using UnityEngine;

public class TargetCollider : MonoBehaviour
{
    public float m_score;
    
    public float Hit()
    {
        gameObject.SetActive(false);
        return m_score;
    }
}
