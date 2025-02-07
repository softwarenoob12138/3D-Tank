using UnityEngine;
using UnityEngine.UI;

public class MyTankShooting : MonoBehaviour
{
    public int PlayerNumber = 1;
    public GameObject FireTransform;
    public GameObject Shell;
    public Slider AimSlider;
    public AudioSource ShootingAudio;
    public AudioClip ChargingClip;
    public AudioClip FireClip;

    // 最大发射力 和 最小发射力
    // 在 0.75 秒内 从最小力加速到最大力 (充能时间)
    public float MinLaunchForce = 15f;
    public float MaxLaunchForce = 30f;
    public float ChargeTime = 0.75f;  
     
    private string FireButton;
    private float CurrentLaunchForce;
    private float ChargeSpeed;

    private bool Fired;

    private void Start()
    {
        FireButton = "Fire" + PlayerNumber;
        ChargeSpeed = (MaxLaunchForce - MinLaunchForce) / ChargeTime; // 充能速度 : 力/时间
    }

    private void Update()
    {
        if (Input.GetButtonDown(FireButton)) // 按下
        {
            CurrentLaunchForce = MinLaunchForce;
            
            ShootingAudio.clip = ChargingClip;
            ShootingAudio.Play();

            Fired = false;
        }

        else if(Input.GetButton(FireButton) && !Fired)  // 按住
        {
            CurrentLaunchForce += ChargeSpeed * Time.deltaTime;

            AimSlider.value = CurrentLaunchForce;

            //if(ShootingAudio.clip == FireClip)    因为按住它是持续检测，所以如果不加限制条件的话，它就会一直重复播放音频的开头
            //{
               // ShootingAudio.clip = ChargingClip;
               //ShootingAudio.Play();
           //}


            if(CurrentLaunchForce >= MaxLaunchForce)
            {
                CurrentLaunchForce = MaxLaunchForce;

                Fire();
            }
        }

        else if(Input.GetButtonUp(FireButton) && !Fired) // 松开
        {
            Fire();
        }


        
    }

    private void Fire()
    {
        AimSlider.value = MinLaunchForce;

        Fired = true;

        // 炮弹生成 继承发射口的位置和旋转信息
        GameObject gameObjectInstance = Instantiate(Shell, FireTransform.transform.position, FireTransform.transform.rotation);

        Rigidbody rigidbody = gameObjectInstance.GetComponent<Rigidbody>();

        rigidbody.velocity = FireTransform.transform.forward * CurrentLaunchForce;

        CurrentLaunchForce = MinLaunchForce;

        ShootingAudio.clip = FireClip;
        ShootingAudio.Play();
    }
}
