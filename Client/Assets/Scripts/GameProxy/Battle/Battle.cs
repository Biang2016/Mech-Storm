public class Battle
{
    internal GameManager GameManager;
    public BattleProxy ClientA;
    public BattleProxy ClientB;

    public Battle(BattleProxy clientA, BattleProxy clientB)
    {
        ClientA = clientA;
        ClientB = clientB;
        GameManager = new GameManager(clientA, clientB);
    }

    public void BattleForceEnd()
    {
        GameManager.OnEndGameByServerError();
    }
}