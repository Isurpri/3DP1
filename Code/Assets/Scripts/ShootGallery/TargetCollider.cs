using UnityEngine;

public class TargetCollider : MonoBehaviour
{
    public float m_score;
    public ParticleSystem m_smokeParticle;
    
    public float Hit()
    {
        ParticleSystem smoke = Instantiate(m_smokeParticle,transform.position,Quaternion.identity);

        smoke.Play();
        m_smokeParticle.Play();
        gameObject.SetActive(false);
        return m_score;
    }
}
