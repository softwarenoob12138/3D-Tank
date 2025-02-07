using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float m_DampTime = 0.2f;                 
    public float m_ScreenEdgeBuffer = 4f;           
    public float m_MinSize = 6.5f;                  
    [HideInInspector] public Transform[] m_Targets; 


    private Camera m_Camera;                        
    private float m_ZoomSpeed;                      
    private Vector3 m_MoveVelocity;                 
    private Vector3 m_DesiredPosition;              


    private void Awake()
    {
        m_Camera = GetComponentInChildren<Camera>();
    }


    private void FixedUpdate()
    {
        Move();
        Zoom(); // 缩放摄像机的视野
    }


    private void Move()
    {
        FindAveragePosition();

        // Vector3.SmoothDamp 方法用于实现平滑的移动或过渡效果
        // Vector3.SmoothDamp(a, b, c, d);
        // a 是当前对象的位置
        // b 是希望到达的位置
        // c 是函数会根据 当前位置和目标位置 更新的速度值
        // d 是阻尼时间，也就是从当前值到目标值平滑过渡所需的时间，值越大，过渡越慢，反之越小
        transform.position = Vector3.SmoothDamp(transform.position, m_DesiredPosition, ref m_MoveVelocity, m_DampTime);
    }


    private void FindAveragePosition()  // 寻找 MainCamera 的更新位置
    {
        Vector3 averagePos = new Vector3();
        int numTargets = 0;

        for (int i = 0; i < m_Targets.Length; i++)
        {
            if (!m_Targets[i].gameObject.activeSelf)
                continue;

            averagePos += m_Targets[i].position;
            numTargets++;
        }

        if (numTargets > 0)
            averagePos /= numTargets; // 找到这些目标位置相距的 中点

        averagePos.y = transform.position.y;

        m_DesiredPosition = averagePos;
    }


    private void Zoom()
    {
        float requiredSize = FindRequiredSize();

        // 这个方法与 Vector3.SmoothDamp 类似，只是把位置变化变成了 浮点数变化
        m_Camera.orthographicSize = Mathf.SmoothDamp(m_Camera.orthographicSize, requiredSize, ref m_ZoomSpeed, m_DampTime);
    }


    private float FindRequiredSize() // 找到需要 的尺寸
    {
        // InverseTransformPoint 方法将 m_DesiredPosition 从世界坐标转换为相对于 transform 的局部坐标
        // 也就是说 在 当前的 transform.position 为 (0, 0, 0) 的情况下  m_DesiredPosition 相对于 当前的 transform.position 的位置
        Vector3 desiredLocalPos = transform.InverseTransformPoint(m_DesiredPosition);

        float size = 0f;

        for (int i = 0; i < m_Targets.Length; i++)
        {
            if (!m_Targets[i].gameObject.activeSelf)
                continue;

            Vector3 targetLocalPos = transform.InverseTransformPoint(m_Targets[i].position);

            Vector3 desiredPosToTarget = targetLocalPos - desiredLocalPos;

            size = Mathf.Max (size, Mathf.Abs (desiredPosToTarget.y)); // Abs 是绝对值，一定为正

            // m_Camera.aspect 是屏幕比值
            // 以屏幕中心点为坐标原点，y是 从中心点往上 到屏幕边缘的值， x是 从中心点往右 到屏幕边缘的值
            // 假设屏幕是 16:9 那么 m_Camera.aspect 的值就为 1.7
            size = Mathf.Max (size, Mathf.Abs (desiredPosToTarget.x) / m_Camera.aspect);

            // size = Mathf.Max(size, Mathf.Abs(desiredPosToTarget.y))
            // 和 size = Mathf.Max (size, Mathf.Abs (desiredPosToTarget.x) / m_Camera.aspect)
            // 计算的是同一个量 都是以屏幕中心点为坐标原点，从中心点往上 到屏幕边缘也就是 y 的值 ，没什么区别
            // size = Mathf.Max(size, Mathf.Abs(desiredPosToTarget.x) / m_Camera.aspect) 是用 x 来表示 y
        }

        size += m_ScreenEdgeBuffer; // 再给由 坦克 规定的最大屏幕边缘 + 一点距离

        size = Mathf.Max(size, m_MinSize); // 坦克距离过近时，让  m_MinSize 赋予屏幕边缘一个最小值

        return size;
    }


    public void SetStartPositionAndSize()
    {
        FindAveragePosition();

        transform.position = m_DesiredPosition;

        m_Camera.orthographicSize = FindRequiredSize();
    }
}