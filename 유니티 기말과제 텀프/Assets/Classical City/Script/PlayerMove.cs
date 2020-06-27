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
    private bool isCrouch = false;

    [Tooltip("얼마나 앉을건지")]
    [SerializeField]
    private float crouchPosY;
    private float originPosY;
    private float applyCouchPosY;

    [Tooltip("카메라 민감도")]
    [SerializeField]
    private float lookSensitivity;


    [Tooltip("카메라 한계")]
    [SerializeField]
    private float cameraRotationLimit = 45.0f;
    private float currentCameraRotationX = 0;

    [SerializeField]
    private Camera theCamera;
    
    //땅 착지여부
    private CapsuleCollider playerCollider;


    private Rigidbody playerRb;
    // Start is called before the first frame update
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        playerCollider = GetComponent<CapsuleCollider>();
        applySpeed = walkSpeed;
        originPosY = theCamera.transform.localPosition.y;
        applyCouchPosY = originPosY;
    }

    // Update is called once per frame
    void Update()
    {

        IsGround();
        TryCrouch();
        TryJump();
        TryRun();
        Move();
        CameraRotation();
        CharacterRotation();
    }

    private void Move()
    {
        float moveDirX = Input.GetAxisRaw("Horizontal");
        float moveDirZ = Input.GetAxisRaw("Vertical");

        Vector3 moveHorizontal = transform.right * moveDirX;
        Vector3 moveVertical = transform.forward * moveDirZ;

        Vector3 veclocity = (moveHorizontal + moveVertical).normalized* applySpeed;

        playerRb.MovePosition(transform.position + veclocity * Time.deltaTime);
    }

    private void CameraRotation()
    {
        float xRotation = Input.GetAxisRaw("Mouse Y");//카메라의 시야는 2d이므로 2차원이다 그래서 x축은 마우스Y이다.
        float cameraRotationX = xRotation * lookSensitivity;
        currentCameraRotationX -= cameraRotationX;//+를하면 마우스올리면 밑으로내려가고 이런식
        currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);

        theCamera.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
    }

    private void CharacterRotation()
    {
        float yRotaion = Input.GetAxisRaw("Mouse X");
        Vector3 characterRotationY = new Vector3(0f, yRotaion, 0f) * lookSensitivity;
        playerRb.MoveRotation(playerRb.rotation * Quaternion.Euler(characterRotationY));
    }

    private void TryRun()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            Running();
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            RunningCancle();
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
            Jump();
        }
    }

    private void Jump()
    {
        if (isCrouch)
        {
            Crouch();
        }
        playerRb.velocity = transform.up * jumpForce;
    }

    private void IsGround()
    {
        isGround = Physics.Raycast(transform.position, Vector3.down, playerCollider.bounds.extents.y+0.1f);//고정적인 아래좌표를 위해서 벡터.다운사용 플레이어콜라이더의크기의y의반, 정확히 반을주면 오차때문에 문제생겨서 조금더준다.

    }


    private void TryCrouch()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
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
        }
        else
        {
            applySpeed = walkSpeed;
            applyCouchPosY = originPosY;

        }

        StartCoroutine(CrouchCoRoutine());
    }

   IEnumerator CrouchCoRoutine()
    {
        float posY = theCamera.transform.localPosition.y;
        int count = 0;

        while(posY != applyCouchPosY)
        {
            count++;
            posY = Mathf.Lerp(posY, applyCouchPosY, 0.3f);
            theCamera.transform.localPosition = new Vector3(0, posY, 0);
            if (count > 15)
            {
                break;
            }
            yield return null;
        }
        theCamera.transform.localPosition = new Vector3(0, applyCouchPosY, 0f);
    }
}
