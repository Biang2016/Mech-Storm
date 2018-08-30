using System.Collections.Generic;
using UnityEngine;

public class CostBarManager : MonoBehaviour
{
    internal ClientPlayer ClientPlayer;

    [SerializeField] private Transform FirstPos;
    [SerializeField] private Transform SecondPos;

    private Vector3 offset;

    void Awake()
    {
        offset = SecondPos.localPosition - FirstPos.localPosition;
    }

    public int CostBarBlockCount;

    private List<CostBarBlock> CostBarBlocks = new List<CostBarBlock>();

    public void SetCostNumber(int value)
    {
        if (value == CostBarBlockCount) return;
        if (value > CostBarBlockCount)
        {
            for (int i = CostBarBlockCount; i < value; i++)
            {
                CostBarBlock newCostBarBlock = GameObjectPoolManager.Instance.Pool_CostBarBlockPool.AllocateGameObject(transform).GetComponent<CostBarBlock>();
                newCostBarBlock.ClientPlayer = ClientPlayer;
                newCostBarBlock.ResetColor();
                newCostBarBlock.transform.localPosition = i * offset;
                newCostBarBlock.transform.localRotation = Quaternion.Euler(0, 0, 0);

                CostBarBlocks.Add(newCostBarBlock);
            }

            CostBarBlockCount = value;
        }
        else if (value < CostBarBlockCount)
        {
            while (value < CostBarBlockCount)
            {
                CostBarBlock newCostBarBlock = CostBarBlocks[value];
                CostBarBlocks.Remove(newCostBarBlock);
                newCostBarBlock.PoolRecycle();
                CostBarBlockCount--;
            }
        }
    }

    public void HightlightTopBlocks(int value)
    {
        if (CostBarBlockCount >= value)
        {
            for (int i = CostBarBlockCount - 1; i > CostBarBlockCount - value - 1; i--)
            {
                CostBarBlocks[i].Shine();
            }
        }
    }

    public void ResetHightlightTopBlocks()
    {
        foreach (CostBarBlock costBarBlock in CostBarBlocks)
        {
            costBarBlock.ResetColor();
        }
    }

}