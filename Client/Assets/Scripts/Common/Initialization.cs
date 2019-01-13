using UnityEngine;

public class Initialization : MonoSingleton<Initialization>
{
    private Initialization()
    {
    }

    [SerializeField] private GameObject Manager;

    void Awake()
    {
        Instantiate(Manager);
        Debug.Log("Start the client...");
    }
}