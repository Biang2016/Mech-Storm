using System;
using TMPro;
using UnityEngine;

public class MechBattleInfoComponent : MechComponentBase
{
    private MechBattleInfoIcon[] BattleInfoIcons;
    [SerializeField] private Transform[] PosPivots;

    void Awake()
    {
        BattleInfoIcons = new MechBattleInfoIcon[Enum.GetNames(typeof(MechBattleInfoIcon.IconTypes)).Length];
        for (int i = 0; i < BattleInfoIcons.Length; i++)
        {
            BattleInfoIcons[i] = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.MechBattleInfoIcon].AllocateGameObject<MechBattleInfoIcon>(transform);
            BattleInfoIcons[i].Initialize((MechBattleInfoIcon.IconTypes) i);
        }

        Reset();
    }

    protected override void Child_Initialize()
    {
        
    }

    protected override void Reset()
    {
        foreach (MechBattleInfoIcon mbii in BattleInfoIcons)
        {
            mbii.Set_Value(0);
        }

        ReplaceAllIcons();
    }

    public void SetValue(MechBattleInfoIcon.IconTypes iconType, int value)
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
        foreach (MechBattleInfoIcon rbii in BattleInfoIcons)
        {
            rbii.SetSortingIndexOfCard(cardSortingIndex);
        }
    }
}