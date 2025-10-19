public class KeyCard : Item
{
    public int m_Acces;
    public override void Pick()
    {
        base.Pick();
        GameManager.GetGameManager().GetPlayer().OpenDoor(m_Acces);
    }
    public override bool CanPick()
    {
        return true;
    }
}
