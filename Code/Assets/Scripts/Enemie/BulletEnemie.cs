using UnityEngine;

public class BulletEnemie : MonoBehaviour
{
    public float m_speed;
    public float m_damage;
    public float m_timeToDestroy;
    private Vector3 m_Direction;

    public void Init(Vector3 direction)
    {
        m_Direction = direction.normalized;

        //Destroy(this, m_timeToDestroy);
    }

    private void Update()
    {
        transform.position += m_Direction * m_speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {

            PlayerController player = other.GetComponent<PlayerController>();
            if (player == null)
                player = other.GetComponentInParent<PlayerController>();

            if (player != null)
            {
                player.TakeDamage(m_damage);
            }

            Destroy(gameObject);
        }
        else if (!other.CompareTag("Enemy") && !other.isTrigger)
        {
            Destroy(gameObject);
        }
    }
}
