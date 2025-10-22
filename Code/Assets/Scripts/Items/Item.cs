using UnityEngine;
public abstract class Item : MonoBehaviour
{
    public LittleDoor m_LittleDoor;
    public virtual void Pick()
    {
        GameObject.Destroy(gameObject);
    }
    public void OpenDoor()
    {
        m_LittleDoor.Acces();
    }
    public abstract bool CanPick();
}
