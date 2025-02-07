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

    // ������� �� ��С������
    // �� 0.75 ���� ����С�����ٵ������ (����ʱ��)
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
        ChargeSpeed = (MaxLaunchForce - MinLaunchForce) / ChargeTime; // �����ٶ� : ��/ʱ��
    }

    private void Update()
    {
        if (Input.GetButtonDown(FireButton)) // ����
        {
            CurrentLaunchForce = MinLaunchForce;
            
            ShootingAudio.clip = ChargingClip;
            ShootingAudio.Play();

            Fired = false;
        }

        else if(Input.GetButton(FireButton) && !Fired)  // ��ס
        {
            CurrentLaunchForce += ChargeSpeed * Time.deltaTime;

            AimSlider.value = CurrentLaunchForce;

            //if(ShootingAudio.clip == FireClip)    ��Ϊ��ס���ǳ�����⣬��������������������Ļ������ͻ�һֱ�ظ�������Ƶ�Ŀ�ͷ
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

        else if(Input.GetButtonUp(FireButton) && !Fired) // �ɿ�
        {
            Fire();
        }


        
    }

    private void Fire()
    {
        AimSlider.value = MinLaunchForce;

        Fired = true;

        // �ڵ����� �̳з���ڵ�λ�ú���ת��Ϣ
        GameObject gameObjectInstance = Instantiate(Shell, FireTransform.transform.position, FireTransform.transform.rotation);

        Rigidbody rigidbody = gameObjectInstance.GetComponent<Rigidbody>();

        rigidbody.velocity = FireTransform.transform.forward * CurrentLaunchForce;

        CurrentLaunchForce = MinLaunchForce;

        ShootingAudio.clip = FireClip;
        ShootingAudio.Play();
    }
}
