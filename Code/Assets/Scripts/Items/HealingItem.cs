public class HealingItem : Item
{
    public int m_Health;
    public override void Pick()
    {
        base.Pick();
        GameManager.GetGameManager().GetPlayer().AddHealing(m_Health);
    }
    public override bool CanPick()
    {
        if (GameManager.GetGameManager().GetPlayer().m_Health >= GameManager.GetGameManager().GetPlayer().m_maxHealth)
        {
            return false;
        }

        return true;
    }
}
