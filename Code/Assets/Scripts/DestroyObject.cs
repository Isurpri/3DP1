using System.Collections;
using UnityEngine;

public class DestroyObject : MonoBehaviour
{
    public float m_DestroyOnTime=0.3f;

    private void Start()
    {
        StartCoroutine(DestroyOnTime());
    }
    IEnumerator DestroyOnTime()
    {
        yield return new WaitForSeconds(m_DestroyOnTime);
        GameObject.Destroy(gameObject);
    }
}
