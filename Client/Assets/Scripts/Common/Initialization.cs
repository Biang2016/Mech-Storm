using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Initialization : MonoBehaviour
{
    [SerializeField] private GameObject Manager;
    private GameManager GameManager;
    private GameBoardManager GameBoardManager;

    void Awake()
    {
        GameObject manager = Instantiate(Manager);

        GameManager gameManager = manager.GetComponentInChildren<GameManager>();
        GameBoardManager gameBoardManager= manager.GetComponentInChildren<GameBoardManager>();
        gameBoardManager.enabled = true;
        gameManager.enabled = true;
    }

    void Start()
    {
    }

    void Update()
    {
    }
}