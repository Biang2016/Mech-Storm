using UnityEngine;

internal class Initialization : MonoSingletion<Initialization>
{
    private Initialization()
    {
    }

    [SerializeField] private GameObject Manager;

    void Awake()
    {
        Instantiate(Manager);
        ClientLog.Instance.PrintClientStates("启动客户端...");
        RoundManager.Instance.StopGame();
    }
}