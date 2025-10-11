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
        return true;
    }
}
