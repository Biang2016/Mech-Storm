using System;
using TMPro;
using UnityEngine;

public class RetinueBattleInfoComponent : RetinueComponentBase
{
    private RetinueBattleInfoIcon[] BattleInfoIcons;
    [SerializeField] private Transform[] PosPivots;

    void Awake()
    {
        BattleInfoIcons = new RetinueBattleInfoIcon[Enum.GetNames(typeof(RetinueBattleInfoIcon.IconTypes)).Length];
        for (int i = 0; i < BattleInfoIcons.Length; i++)
        {
            BattleInfoIcons[i] = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.RetinueBattleInfoIcon].AllocateGameObject<RetinueBattleInfoIcon>(transform);
            BattleInfoIcons[i].Initialize((RetinueBattleInfoIcon.IconTypes) i);
        }

        ReplaceAllIcons();
    }

    public void SetValue(RetinueBattleInfoIcon.IconTypes iconType, int value)
    {
        BattleInfoIcons[(int) iconType].Set_Value(value);
        ReplaceAllIcons();
    }

    private void ReplaceAllIcons()
    {
        int index = 0;
        for (int i = 0; i < BattleInfoIcons.Length; i++)
        {
            if (BattleInfoIcons[i].Value != 0)
            {
                BattleInfoIcons[i].transform.position = PosPivots[index].position;
                index++;
            }
        }
    }

    protected override void SetSortingIndexOfCard(int cardSortingIndex)
    {
        foreach (RetinueBattleInfoIcon rbii in BattleInfoIcons)
        {
            rbii.SetSortingIndexOfCard(cardSortingIndex);
        }
    }
}