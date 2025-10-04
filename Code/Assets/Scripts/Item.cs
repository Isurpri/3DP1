using UnityEngine;
public class Item : MonoBehaviour
{
    public virtual void Pick()
    {
        GameManager.Destroy(gameObject);
    }
}
