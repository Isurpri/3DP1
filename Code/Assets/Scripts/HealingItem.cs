public class HealingItem : Item
{
    public int m_Health;
    public override void Pick()
    {
        base.Pick();
        GameManager.GetGameManager().GetPlayer().AddHealing(m_Health);
    }
}
