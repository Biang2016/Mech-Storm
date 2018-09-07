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

    public void ResetAll()
    {
        ClientPlayer = null;
    }

    public int MetalBarBlockCount;

    private List<MetalBarBlock> MetalBarBlocks = new List<MetalBarBlock>();

    public void SetMetalNumber(int value)
    {
        if (value == MetalBarBlockCount) return;
        if (value > MetalBarBlockCount)
        {
            for (int i = MetalBarBlockCount; i < value; i++)
            {
                MetalBarBlock newMetalBarBlock = GameObjectPoolManager.Instance.Pool_MetalBarBlockPool.AllocateGameObject(transform).GetComponent<MetalBarBlock>();
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
    }

    public void HightlightTopBlocks(int value)
    {
        if (MetalBarBlockCount >= value)
        {
            for (int i = MetalBarBlockCount - 1; i > MetalBarBlockCount - value - 1; i--)
            {
                MetalBarBlocks[i].Shine();
            }
        }
    }

    public void ResetHightlightTopBlocks()
    {
        foreach (MetalBarBlock metalBarBlock in MetalBarBlocks)
        {
            metalBarBlock.ResetColor();
        }
    }

}