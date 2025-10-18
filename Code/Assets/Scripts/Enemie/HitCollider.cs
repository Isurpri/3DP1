using UnityEngine;

public class HitCollider : MonoBehaviour
{
    public int m_damage;
    public EnemyController m_Enemy;
    
    public void Hit()
    {
        m_Enemy.Hit(m_damage);
    }
}
