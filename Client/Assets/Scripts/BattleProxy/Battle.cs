using System.Collections.Generic;

public class Battle
{
    public delegate void OnEndGameDelegate(int winnerClientID);

    public ILog DebugLog;
    private GameManager GameManager;
    public BattleProxy ClientA;
    public BattleProxy ClientB;

    public Battle(BattleProxy clientA, BattleProxy clientB, ILog debugLog, OnEndGameDelegate onEndGameDelegate)
    {
        DebugLog = debugLog;
        ClientA = clientA;
        ClientB = clientB;
        GameManager = new GameManager(this, clientA, clientB, onEndGameDelegate);
        clientA.Battle = this;
        clientB.Battle = this;
    }

    public void BattleForceEnd()
    {
        GameManager.OnEndGameByServerError();
    }
}