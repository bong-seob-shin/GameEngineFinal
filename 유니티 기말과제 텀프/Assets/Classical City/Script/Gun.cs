using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public float range;//사거리
    public float accuracy;//정확도
    public float fireRate;//연사속도
    public float fireDelayTime;//연사 딜레이
    public float reloadTime; //재장전 속도
    
    public int damage;//총의 데미지

    public int reloadBulletCount; //총알 재장전 개수
    public int currentBulletCount;//현재 탄알 개수
    public int maxBulletCount;//최대 탄창에 들어가는 탄알 개수
    public int hasMaxBulletcount; //보유한 최대 탄알 개수
    public float retroActionForce; //반동 세기
    public float retroActionFineSightForce;//정조준시의 반동 세기

    public Vector3 fireSightOriginPos;

    public PlayerMove playerMove;
    public ParticleSystem muzzleFalsh;
    public Animator PlayerAnim;
    public AudioSource fireSound;
   

    private void Update()
    {
        TryFire();
        GunFireRateCalc();
    }

    private void GunFireRateCalc()
    {
        if (fireDelayTime > 0)
        {
            fireDelayTime -= Time.deltaTime;
        }
    }

    void TryFire()
    {

        if (Input.GetButton("Fire1") && fireDelayTime <= 0)
        {
            Fire();
        }
        else if (Input.GetButtonUp("Fire1"))
        {
           
           PlayerAnim.SetBool("SeatGunPlay", false);
           PlayerAnim.SetBool("GunPlay", false);
           fireSound.Stop(); 
        }
    }
    void Fire()
    {
        fireDelayTime = fireRate;
        Shoot();
    }
    void Shoot()
    {
        muzzleFalsh.Play();
        if (playerMove.isCrouch)
        {
            PlayerAnim.SetBool("SeatGunPlay",true);
        }
        else
        {
            PlayerAnim.SetBool("GunPlay", true);
        }
        fireSound.Play();
    }
}
