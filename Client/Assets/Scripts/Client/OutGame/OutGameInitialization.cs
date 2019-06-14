using UnityEngine;

public class OutGameInitialization : OutGameMonoSingleton<OutGameInitialization>
{
    [SerializeField] private GameObject Manager;
    public bool AutoUpdate = true;

    private OutGameInitialization()
    {
    }

    void Awake()
    {
        Instantiate(Manager);
    }
}