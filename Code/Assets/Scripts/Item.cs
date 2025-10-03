using UnityEngine;
public class Item : MonoBehaviour
{
    public void Pick()
    {
        GameManager.Destroy(gameObject);
    }
}
