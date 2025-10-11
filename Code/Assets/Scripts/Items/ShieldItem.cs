public class ShieldItem : Item
{
    public int m_addShield;
    public override void Pick()
    {
        base.Pick();
        GameManager.GetGameManager().GetPlayer().AddShield(m_addShield);
    }
    public override bool CanPick()
    {
        if (GameManager.GetGameManager().GetPlayer().m_Shield >= GameManager.GetGameManager().GetPlayer().m_maxShield)
        {
            return false;
        }
        return true;
    }
}
