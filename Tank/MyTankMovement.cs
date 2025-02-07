using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyTankMovement : MonoBehaviour
{
    public int PlayerNumber = 1;
    public float Speed;
    public float TurnSpeed;
    public AudioSource MovementAudio;
    public AudioClip EngineIdling;
    public AudioClip EngineDriving;
    public float PitchRange = 0.2f;

    private Rigidbody rb;
    private string MovementAxisName;
    private string TurnAxisName;
    private float MovementInputValue;
    private float TurnInputValue;
    private float OriginalPitch;

    private void Awake()
    {
        Debug.Log("Awake");
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        MovementAxisName = "Horizontal" + PlayerNumber;
        TurnAxisName = "Vertical" + PlayerNumber;

        // ��ȡ������Ĭ��ֵ �� һ�� Unity �г����� ���Ʊ����ķ���
        OriginalPitch = MovementAudio.pitch;
    }
    
    void Update()
    {
        TurnInputValue = Input.GetAxis(MovementAxisName);

        // GetAxis ���й���ֵ�� ���룬GetAxisRaw ��û�й���ֵ�� ����
        MovementInputValue = Input.GetAxis(TurnAxisName);

        PlayAudio();

    }

    // ��ò�Ҫ�� ��ȡֵ�Ĳ������� FixedUpdate ���������Ϊ�̶���֡�ʲ�����ʱ����������ʱ���ȡ���� 
    private void FixedUpdate()
    {
        Move();
        Turn();
    }

    private void PlayAudio()
    {
        // Mathf.Abs() Mathf ��ȡ����ֵ��������ȡ����ֵ�Ķ������������ı���
        if (Mathf.Abs(MovementInputValue) < 0.1f && Mathf.Abs(TurnInputValue) < 0.1f)
        {
            if (MovementAudio.clip == EngineDriving)
            {
                MovementAudio.clip = EngineIdling;  // �˴��� ��Դ����е� Clip ��Ϊ EngineIdling
                MovementAudio.pitch = Random.Range(OriginalPitch - PitchRange, OriginalPitch + PitchRange);
                MovementAudio.Play();
            }
        }
        else
        {
            if (MovementAudio.clip == EngineIdling)
            {
                MovementAudio.clip = EngineDriving;
                MovementAudio.pitch = Random.Range(OriginalPitch - PitchRange, OriginalPitch + PitchRange);
                MovementAudio.Play();
            }

        }
    }




    private void Turn()
    {
        float turn = TurnInputValue * TurnSpeed * Time.deltaTime;

        // Euler ŷ������ Quaternion (��Ԫ��) �� һ����������������һ�� ������ת���� (rb.MoveRotation) �� ��Ԫ��
        // Euler ŷ���� Euler(x, y, z) ���������� �ֱ���� �� x ����ת���� y ����ת�� �� z ����ת 
        Quaternion quaternion = Quaternion.Euler(0, turn, 0);

        // �м��ƶ�����ת�õ� ��ֵ�ֶβ�һ�����ƶ��üӷ�����ת����Ҫ�ó˷�
        rb.MoveRotation(rb.rotation * quaternion);
    }

    private void Move()
    {
        Vector3 movementV3 = transform.forward * MovementInputValue * Speed * Time.deltaTime;

        // MovePosition ���Ǹ�����˶�������������Ҫ rb.position + movementV3������ֱ�� ���� movementV3
        // ��� 2D ��Ϸ���������
        rb.MovePosition(rb.position + movementV3);
    }


}
