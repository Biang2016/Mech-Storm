using System;
using UnityEngine;

public class MechTriggerIconComponent : MechComponentBase
{
    private MechTriggerIcon[] MechTriggerIcons;
    [SerializeField] private Transform[] PosPivots;

    void Awake()
    {
        MechTriggerIcons = new MechTriggerIcon[Enum.GetNames(typeof(MechTriggerIcon.IconTypes)).Length];

        for (int i = 0; i < MechTriggerIcons.Length; i++)
        {
            MechTriggerIcon temp = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.MechTriggerIcon].AllocateGameObject<MechTriggerIcon>(transform);
            MechTriggerIcons[i] = temp;
            MechTriggerIcons[i].Initialize((MechTriggerIcon.IconTypes) i);
        }

        Reset();
    }

    protected override void Child_Initialize()
    {
        if (Mech.CardInfo.SideEffectBundle.GetSideEffectExecutes(SideEffectExecute.TriggerTime.OnMechDie, SideEffectExecute.TriggerRange.Self).Count != 0)
        {
            ShowIcon(MechTriggerIcon.IconTypes.Die, true);
        }

        foreach (SideEffectExecute see in Mech.CardInfo.SideEffectBundle_BattleGroundAura.SideEffectExecutes)
        {
            if (!(see.M_ExecuteSetting.TriggerTime == SideEffectExecute.TriggerTime.OnMechDie && see.M_ExecuteSetting.TriggerRange == SideEffectExecute.TriggerRange.Self)
                && !(see.M_ExecuteSetting.TriggerTime == SideEffectExecute.TriggerTime.OnMechSummon && see.M_ExecuteSetting.TriggerRange == SideEffectExecute.TriggerRange.Self))
            {
                ShowIcon(MechTriggerIcon.IconTypes.Trigger, true);
                break;
            }
        }
    }

    protected override void Reset()
    {
        foreach (MechTriggerIcon mti in MechTriggerIcons)
        {
            mti.ShowIcon(false);
        }

        ReplaceAllIcons();
    }

    protected override void SetSortingIndexOfCard(int cardSortingIndex)
    {
        foreach (MechTriggerIcon rti in MechTriggerIcons)
        {
            rti.SetSortingIndexOfCard(cardSortingIndex);
        }
    }

    public void ShowIcon(MechTriggerIcon.IconTypes iconType, bool isShow)
    {
        MechTriggerIcons[(int) iconType].ShowIcon(isShow);
        ReplaceAllIcons();
    }

    public void IconJump(MechTriggerIcon.IconTypes iconType)
    {
        MechTriggerIcons[(int) iconType].IconJump();
    }

    private void ReplaceAllIcons()
    {
        int index = 0;
        for (int i = 0; i < MechTriggerIcons.Length; i++)
        {
            if (MechTriggerIcons[i].IsShow)
            {
                MechTriggerIcons[i].transform.position = PosPivots[index].position;
                index++;
            }
        }
    }
}