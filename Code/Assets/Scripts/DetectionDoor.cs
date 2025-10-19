using UnityEngine;

public class DetectionDoor : MonoBehaviour
{
    public Animation m_RightDoorAnimaton;
    public Animation m_LeftDoorAnimaton;
    public AnimationClip m_CloseRightDoor;
    public AnimationClip m_CloseLeftDoor;
    public AnimationClip m_OpenRightDoor;
    public AnimationClip m_OpenLeftDoor;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            m_RightDoorAnimaton.Play(m_OpenRightDoor.name);
            m_LeftDoorAnimaton.Play(m_OpenLeftDoor.name);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            m_RightDoorAnimaton.Play(m_CloseRightDoor.name);
            m_LeftDoorAnimaton.Play(m_CloseLeftDoor.name);
        }
    }
}
