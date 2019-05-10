using System;
using UnityEngine;

public class RetinueTriggerIconComponent : RetinueComponentBase
{
    private RetinueTriggerIcon[] RetinueTriggerIcons;
    [SerializeField] private Transform[] PosPivots;

    void Awake()
    {
        RetinueTriggerIcons = new RetinueTriggerIcon[Enum.GetNames(typeof(RetinueTriggerIcon.IconTypes)).Length];

        for (int i = 0; i < RetinueTriggerIcons.Length; i++)
        {
            RetinueTriggerIcons[i] = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.RetinueBattleInfoIcon].AllocateGameObject<RetinueTriggerIcon>(transform);
            RetinueTriggerIcons[i].Initialize((RetinueTriggerIcon.IconTypes) i);
        }

        ReplaceAllIcons();
    }

    protected override void SetSortingIndexOfCard(int cardSortingIndex)
    {
        foreach (RetinueTriggerIcon rti in RetinueTriggerIcons)
        {
            rti.SetSortingIndexOfCard(cardSortingIndex);
        }
    }

    public void ShowIcon(RetinueTriggerIcon.IconTypes iconType, bool isShow)
    {
        RetinueTriggerIcons[(int) iconType].ShowIcon(isShow);
        ReplaceAllIcons();
    }

    public void IconJump(RetinueTriggerIcon.IconTypes iconType)
    {
        RetinueTriggerIcons[(int) iconType].IconJump();
    }

    private void ReplaceAllIcons()
    {
        int index = 0;
        for (int i = 0; i < RetinueTriggerIcons.Length; i++)
        {
            if (RetinueTriggerIcons[i].IsShow)
            {
                RetinueTriggerIcons[i].transform.position = PosPivots[index].position;
                index++;
            }
        }
    }
}