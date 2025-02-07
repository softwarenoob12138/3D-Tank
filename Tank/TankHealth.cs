using UnityEngine;
using UnityEngine.UI;

public class TankHealth : MonoBehaviour
{
    public float m_StartingHealth = 100f;          
    public Slider m_Slider;                        
    public Image m_FillImage;                      
    public Color m_FullHealthColor = Color.green;  
    public Color m_ZeroHealthColor = Color.red;    
    public GameObject m_ExplosionPrefab;
    
    
    private AudioSource m_ExplosionAudio;          
    private ParticleSystem m_ExplosionParticles;   
    private float m_CurrentHealth;  
    private bool m_Dead;            


    private void Awake()
    {
        m_ExplosionParticles = Instantiate(m_ExplosionPrefab).GetComponent<ParticleSystem>();

        m_ExplosionAudio = m_ExplosionParticles.GetComponent<AudioSource>();

        m_ExplosionParticles.gameObject.SetActive(false);
    }


    // 当脚本组件变为启用状态时会被调用
    // 1.组件首次启用时会被调用
    // 2.游戏对象从禁用变为启用时会被调用
    // 3.处于激活状态下 场景加载完成后会被调用
    private void OnEnable()
    {
        m_CurrentHealth = m_StartingHealth;
        m_Dead = false;

        SetHealthUI();
    }
    

    public void TakeDamage(float amount)
    {
        m_CurrentHealth -= amount;

        SetHealthUI();

        if(m_CurrentHealth <= 0 && !m_Dead)
        {
            OnDeath(); 
        }
    }


    private void SetHealthUI()
    {
        m_Slider.value = m_CurrentHealth;


        // Color.Lerp 是一个静态方法，用于在两个颜色之间进行线性插值，用于实现颜色渐变的效果 
        // Color.Lerp(a, b, c)
        // a 为 插值的起始颜色
        // b 为 插值的目标颜色
        // c 为 插值因子，介于 0 和 1 之间，返回两者之间的颜色
        m_FillImage.color = Color.Lerp(m_ZeroHealthColor, m_FullHealthColor, m_CurrentHealth / m_StartingHealth);

        //if(m_CurrentHealth > 40)
        //{
        //    m_FillImage.color = m_FullHealthColor;
        //}
        //else
        //{
        //    m_FillImage.color = m_ZeroHealthColor;
        //}
    }


    private void OnDeath()
    {
        m_Dead = true;

        m_ExplosionParticles.transform.position = transform.position;
        m_ExplosionParticles.gameObject.SetActive(true);

        m_ExplosionParticles.Play();

        m_ExplosionAudio.Play();

        gameObject.SetActive(false);
    }
}