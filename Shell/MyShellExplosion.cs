using UnityEngine;

public class MyShellExplosion : MonoBehaviour
{
    public LayerMask TankMask;
    public ParticleSystem ExplosionParticles;
    public AudioSource ExplosionAudio;
    public float MaxDamage = 100f;
    public float ExplosionForce = 1000f;
    public float MaxLifeTime = 2f;
    public float ExplosionRadius = 5f;


    private void Start()
    {
        // MaxLifeTime 后摧毁物体
        Destroy(transform.gameObject, MaxLifeTime);
    }

    private void OnTriggerEnter(Collider other)  // 让被它碰到的带有 Collider 的物体 触发的方法
    {
        // Physics.OverlapSphere 方法用于在 3D 游戏中查找围绕空间中某一点一定半径内的所有碰撞器 Physics.OverlapSphere(x, y, z)
        // x 为球体检测器的中心点
        // y 为球体的半径
        // z 为层掩码(图层)，用于筛选应该考虑哪些图层上的碰撞器
        Collider[] targetCollider = Physics.OverlapSphere(transform.position, ExplosionRadius, TankMask);  // 获取球体里所有的 TankMask

        for (int i = 0; i < targetCollider.Length; i++)
        {
            Rigidbody target = targetCollider[i].GetComponent<Rigidbody>();

            if (target == null)
            {
                continue; // 如果 target 为空 就进行跳转到下一个循环，不执行下面的代码了
            }

            // AddExplosionForce 是 Rigidbody 的一个方法 用来施加爆炸力
            // AddExplosionForce(a, b, c)
            // a 为爆炸力的大小
            // b 为爆炸力的中心位置
            // c 为爆炸力的半径
            target.AddExplosionForce(ExplosionForce, transform.position, ExplosionRadius);

            TankHealth targetTankHealth = targetCollider[i].GetComponent<TankHealth>();

            float damage = CalculateDamage(target.gameObject.transform.position);

            targetTankHealth.TakeDamage(damage);

        }

        //TankHealth tankHealth = other.gameObject.GetComponent<TankHealth>();

        //if (tankHealth != null) // 这个判断语句用来识别 Tank
        //{
            //tankHealth.TakeDamage(20);
        //}


        // 如果不脱离 父级 那么销毁时就会 连带父级一同被销毁
        ExplosionParticles.transform.parent = null;
        ExplosionParticles.Play();

        ExplosionAudio.Play();

        // ExplosionParticles.main.duration 可获取到粒子特效的播放时间
        Destroy(ExplosionParticles.gameObject, ExplosionParticles.main.duration);

        Destroy(gameObject);

    }

    private float CalculateDamage(Vector3 targetPosition)
    {
        // 计算坦克与爆炸点的距离
        Vector3 v3 = targetPosition - transform.position;

        // v3.magnitude 是向量 v3 的长度(向量的模) 
        // 注意单个的 Vector3 向量类型的值 是不能赋值给 float 类型的值的，需要转化为模
        float distance = v3.magnitude;

        float damage = ((ExplosionRadius - distance) / ExplosionRadius) * MaxDamage; // 利用半径占比 来计算范围爆炸的伤害

        // 避免更改某些变量和突发情况使 damage 变成负值
        damage = Mathf.Max(0, damage);

        return damage;
    }

}
