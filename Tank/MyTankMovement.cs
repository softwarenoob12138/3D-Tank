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

        // 获取和设置默认值 是 一种 Unity 中常见的 控制变量的方法
        OriginalPitch = MovementAudio.pitch;
    }
    
    void Update()
    {
        TurnInputValue = Input.GetAxis(MovementAxisName);

        // GetAxis 是有过渡值的 输入，GetAxisRaw 是没有过渡值的 输入
        MovementInputValue = Input.GetAxis(TurnAxisName);

        PlayAudio();

    }

    // 最好不要把 获取值的操作放在 FixedUpdate 否则可能因为固定的帧率产生的时间间隔导致有时候获取不到 
    private void FixedUpdate()
    {
        Move();
        Turn();
    }

    private void PlayAudio()
    {
        // Mathf.Abs() Mathf 的取绝对值方法，被取绝对值的对象就是括号里的变量
        if (Mathf.Abs(MovementInputValue) < 0.1f && Mathf.Abs(TurnInputValue) < 0.1f)
        {
            if (MovementAudio.clip == EngineDriving)
            {
                MovementAudio.clip = EngineIdling;  // 此处将 音源组件中的 Clip 变为 EngineIdling
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

        // Euler 欧拉角是 Quaternion (四元数) 的 一个方法，用来返回一个 用于旋转方法 (rb.MoveRotation) 的 四元数
        // Euler 欧拉角 Euler(x, y, z) 中三个变量 分别代表， 绕 x 轴旋转，绕 y 轴旋转， 绕 z 轴旋转 
        Quaternion quaternion = Quaternion.Euler(0, turn, 0);

        // 切记移动和旋转用的 赋值手段不一样，移动用加法，旋转必须要用乘法
        rb.MoveRotation(rb.rotation * quaternion);
    }

    private void Move()
    {
        Vector3 movementV3 = transform.forward * MovementInputValue * Speed * Time.deltaTime;

        // MovePosition 这是个相对运动方法，所以需要 rb.position + movementV3，不能直接 输入 movementV3
        // 这和 2D 游戏是有区别的
        rb.MovePosition(rb.position + movementV3);
    }


}
