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
        // MaxLifeTime ��ݻ�����
        Destroy(transform.gameObject, MaxLifeTime);
    }

    private void OnTriggerEnter(Collider other)  // �ñ��������Ĵ��� Collider ������ �����ķ���
    {
        // Physics.OverlapSphere ���������� 3D ��Ϸ�в���Χ�ƿռ���ĳһ��һ���뾶�ڵ�������ײ�� Physics.OverlapSphere(x, y, z)
        // x Ϊ�������������ĵ�
        // y Ϊ����İ뾶
        // z Ϊ������(ͼ��)������ɸѡӦ�ÿ�����Щͼ���ϵ���ײ��
        Collider[] targetCollider = Physics.OverlapSphere(transform.position, ExplosionRadius, TankMask);  // ��ȡ���������е� TankMask

        for (int i = 0; i < targetCollider.Length; i++)
        {
            Rigidbody target = targetCollider[i].GetComponent<Rigidbody>();

            if (target == null)
            {
                continue; // ��� target Ϊ�� �ͽ�����ת����һ��ѭ������ִ������Ĵ�����
            }

            // AddExplosionForce �� Rigidbody ��һ������ ����ʩ�ӱ�ը��
            // AddExplosionForce(a, b, c)
            // a Ϊ��ը���Ĵ�С
            // b Ϊ��ը��������λ��
            // c Ϊ��ը���İ뾶
            target.AddExplosionForce(ExplosionForce, transform.position, ExplosionRadius);

            TankHealth targetTankHealth = targetCollider[i].GetComponent<TankHealth>();

            float damage = CalculateDamage(target.gameObject.transform.position);

            targetTankHealth.TakeDamage(damage);

        }

        //TankHealth tankHealth = other.gameObject.GetComponent<TankHealth>();

        //if (tankHealth != null) // ����ж��������ʶ�� Tank
        //{
            //tankHealth.TakeDamage(20);
        //}


        // ��������� ���� ��ô����ʱ�ͻ� ��������һͬ������
        ExplosionParticles.transform.parent = null;
        ExplosionParticles.Play();

        ExplosionAudio.Play();

        // ExplosionParticles.main.duration �ɻ�ȡ��������Ч�Ĳ���ʱ��
        Destroy(ExplosionParticles.gameObject, ExplosionParticles.main.duration);

        Destroy(gameObject);

    }

    private float CalculateDamage(Vector3 targetPosition)
    {
        // ����̹���뱬ը��ľ���
        Vector3 v3 = targetPosition - transform.position;

        // v3.magnitude ������ v3 �ĳ���(������ģ) 
        // ע�ⵥ���� Vector3 �������͵�ֵ �ǲ��ܸ�ֵ�� float ���͵�ֵ�ģ���Ҫת��Ϊģ
        float distance = v3.magnitude;

        float damage = ((ExplosionRadius - distance) / ExplosionRadius) * MaxDamage; // ���ð뾶ռ�� �����㷶Χ��ը���˺�

        // �������ĳЩ������ͻ�����ʹ damage ��ɸ�ֵ
        damage = Mathf.Max(0, damage);

        return damage;
    }

}
