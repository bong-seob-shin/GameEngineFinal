using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    


    [Tooltip("스피드 조정")]
    [SerializeField]
    private float walkSpeed;
    [SerializeField]
    private float runSpeed;
    [SerializeField]
    private float crouchSpeed;
    private float applySpeed;//걷기와 뛰기 함수를 두개만들지말고 이동하는것에 속도를 적용한다.

    [Tooltip("점프 조정")]
    [SerializeField]
    private float jumpForce;

    [Tooltip("상태 변수")]
    private bool isRun = false;
    private bool isGround = true;
    public bool isCrouch = false;

    [Tooltip("얼마나 앉을건지")]
    [SerializeField]
    private float crouchPosY;
    private float originPosY;
    private float applyCouchPosY;

    [Tooltip("카메라 민감도")]
    [SerializeField]
    private float lookSensitivity;


    [SerializeField]
    private Camera theCamera;
    public Transform CameraArm;


    public Animator animator; 

    [SerializeField]
    private Transform player;

    //땅 착지여부
    private CapsuleCollider playerCollider;


    private Rigidbody playerRb;
    
    // Start is called before the first frame update
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        playerCollider = GetComponent<CapsuleCollider>();
        applySpeed = walkSpeed;
        originPosY = CameraArm.transform.localPosition.y;
        applyCouchPosY = originPosY;
    }

    // Update is called once per frame
    void Update()
    {

        IsGround();
        TryCrouch();
        TryRun();
        TryJump();
        Move();
        LookAround();
        //CameraRotation();
        //CharacterRotation();
    }

    private void Move()
    {
        Vector2 moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        bool isMove = moveInput.magnitude != 0;

        if (isMove)
        {
            animator.SetBool("GunWalk", true);

            Vector3 lookForward = new Vector3(CameraArm.forward.x, 0f, CameraArm.forward.z).normalized;
            Vector3 lookRight = new Vector3(CameraArm.right.x, 0f, CameraArm.right.z).normalized;
            Vector3 moveDir = lookForward * moveInput.y + lookRight * moveInput.x;

            player.forward = moveDir;
            transform.position += moveDir * Time.deltaTime * applySpeed;
        }
        else
        {
            animator.SetBool("GunWalk", false);

        }


    }

  

    private void LookAround()
    {
        Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        Vector3 camAngle = CameraArm.rotation.eulerAngles;
        float x = camAngle.x - mouseDelta.y;

        if (x < 180f)
        {
            x = Mathf.Clamp(x, -1f, 70f);
        }
        else
        {
            x = Mathf.Clamp(x, 358f, 361f);
        }
        CameraArm.rotation = Quaternion.Euler(x, camAngle.y+mouseDelta.x, camAngle.z);
    }

    private void TryRun()
    {
        if (Input.GetKey(KeyCode.LeftShift)&&!isCrouch)
        {
            Running();
            animator.SetBool("GunRun", true);
        }
        if (Input.GetKeyUp(KeyCode.LeftShift)&&!isCrouch)
        {
            RunningCancle();
            animator.SetBool("GunRun", false);

        }
    }

    private void Running()
    {
        isRun = true;
        applySpeed = runSpeed;
    }

    private void RunningCancle()
    {
        isRun = false;
        applySpeed = walkSpeed;
    }


    private void TryJump()
    {
        if(Input.GetKeyDown(KeyCode.Space)&& isGround)
        {
            animator.SetBool("Jump", true);
            Jump();
        }
    }

    private void Jump()
    {
        if (isCrouch)
        {
            Crouch();
        }
        playerRb.velocity = player.transform.up * jumpForce;
    }

    private void IsGround()
    {
        isGround = Physics.Raycast(player.transform.position, Vector3.down, playerCollider.bounds.extents.y+0.1f);//고정적인 아래좌표를 위해서 벡터.다운사용 플레이어콜라이더의크기의y의반, 정확히 반을주면 오차때문에 문제생겨서 조금더준다.
        if (isGround)
        {
            animator.SetBool("Jump", false);
        }
    }


    private void TryCrouch()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            RunningCancle();
            Crouch();
        }
    }

    private void Crouch()
    {
        isCrouch = !isCrouch;

        if (isCrouch)
        {
            applySpeed = crouchSpeed;
            applyCouchPosY = crouchPosY;
            animator.SetBool("Crouch", true);

        }
        else
        {
            applySpeed = walkSpeed;
            applyCouchPosY = originPosY;
            animator.SetBool("Crouch", false);

        }

        StartCoroutine(CrouchCoRoutine());
    }
   
   IEnumerator CrouchCoRoutine()
    {
        float posY = CameraArm.transform.localPosition.y;
        int count = 0;

        while(posY != applyCouchPosY)
        {
            count++;
            posY = Mathf.Lerp(posY, applyCouchPosY, 0.3f);
            CameraArm.transform.localPosition = new Vector3(0, posY, 0);
            if (count > 15)
            {
                break;
            }
            yield return null;
        }
        CameraArm.transform.localPosition = new Vector3(0, applyCouchPosY, 0f);
    }
}
