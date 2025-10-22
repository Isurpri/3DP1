public class KeyCard : Item
{
    public int m_Acces;
    public override void Pick()
    {
        base.Pick();
        base.OpenDoor();
    }
    public override bool CanPick()
    {
        return true;
    }
}
