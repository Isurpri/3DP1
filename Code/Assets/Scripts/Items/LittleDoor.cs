using UnityEngine;

public class LittleDoor : MonoBehaviour
{
    
    public Animation m_OpenDoor;
    public AnimationClip m_OpenLittleDoor;

    public void Acces()
    {
        m_OpenDoor.Play(m_OpenLittleDoor.name);
    }
}
