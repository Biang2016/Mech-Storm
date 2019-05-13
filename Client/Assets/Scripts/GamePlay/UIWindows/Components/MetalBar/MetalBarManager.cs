using System.Collections.Generic;
using UnityEngine;

public class MetalBarManager : MonoBehaviour
{
    internal ClientPlayer ClientPlayer;

    [SerializeField] private Transform FirstPos;
    [SerializeField] private Transform SecondPos;

    private Vector3 offset;

    void Awake()
    {
        offset = SecondPos.localPosition - FirstPos.localPosition;
    }

    public void Initialize(ClientPlayer clientPlayer)
    {
        ResetAll();
        ClientPlayer = clientPlayer;
    }

    public void ResetAll()
    {
        foreach (MetalBarBlock mbb in MetalBarBlocks)
        {
            mbb.PoolRecycle();
        }

        MetalBarBlocks.Clear();
        MetalBarBlockCount = 0;
        ClientPlayer = null;
    }

    public int MetalBarBlockCount;

    private List<MetalBarBlock> MetalBarBlocks = new List<MetalBarBlock>();

    public void SetMetalNumber(int value)
    {
        if (value == MetalBarBlockCount || value < 0) return;
        if (value > MetalBarBlockCount)
        {
            for (int i = MetalBarBlockCount; i < value; i++)
            {
                MetalBarBlock newMetalBarBlock = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.MetalBarBlock].AllocateGameObject<MetalBarBlock>(transform);
                newMetalBarBlock.ClientPlayer = ClientPlayer;
                newMetalBarBlock.ResetColor();
                newMetalBarBlock.transform.localPosition = i * offset;
                newMetalBarBlock.transform.localRotation = Quaternion.Euler(0, 0, 0);

                MetalBarBlocks.Add(newMetalBarBlock);
            }

            MetalBarBlockCount = value;
        }
        else if (value < MetalBarBlockCount)
        {
            while (value < MetalBarBlockCount)
            {
                MetalBarBlock newMetalBarBlock = MetalBarBlocks[value];
                MetalBarBlocks.Remove(newMetalBarBlock);
                newMetalBarBlock.PoolRecycle();
                MetalBarBlockCount--;
            }
        }

        ResetHighlightTopBlocks();
    }

    public void HighlightTopBlocks(int value)
    {
        if (MetalBarBlockCount >= value)
        {
            for (int i = MetalBarBlockCount - 1; i > MetalBarBlockCount - value - 1; i--)
            {
                MetalBarBlocks[i].Shine();
            }
        }
    }

    public void ResetHighlightTopBlocks()
    {
        foreach (MetalBarBlock metalBarBlock in MetalBarBlocks)
        {
            metalBarBlock.ResetColor();
        }
    }
}