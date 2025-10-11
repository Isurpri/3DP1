public class AmmoItem : Item
{
    public int m_AmmoCount;
    public override void Pick()
    {
        base.Pick();
        GameManager.GetGameManager().GetPlayer().AddAmmo(m_AmmoCount);

    }
    public override bool CanPick()
    {
        if (GameManager.GetGameManager().GetPlayer().m_totalAmount >= GameManager.GetGameManager().GetPlayer().m_MaxAmount)
        {
            return false;
        }
        return true;
    }
}
