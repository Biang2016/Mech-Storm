using UnityEngine;

public class OutGameInitialization : OutGameMonoSingleton<OutGameInitialization>
{
    [SerializeField] private GameObject Manager;

    private OutGameInitialization()
    {
    }

    void Awake()
    {
        Instantiate(Manager);
    }
}