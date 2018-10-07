using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Ship : MonoBehaviour, IMouseHoverComponent
{
    internal ClientPlayer ClientPlayer;

    [SerializeField] private GameObject ShipBG;

    [SerializeField] private TextMesh DamageNumberPreviewTextMesh;
    [SerializeField] private TextMesh DamageNumberPreviewBGTextMesh;

    [SerializeField] private TextMesh Desc;
    [SerializeField] private TextMesh DescBG;

    void Awake()
    {
        ShipBG.SetActive(false);
        if (DamageNumberPreviewTextMesh) DamageNumberPreviewTextMesh.text = "";
        if (DamageNumberPreviewBGTextMesh) DamageNumberPreviewBGTextMesh.text = "";
    }

    public void MouseHoverComponent_OnMousePressEnterImmediately(Vector3 mousePosition)
    {
        if (DragManager.Instance.CurrentDrag)
        {
            ModuleRetinue mr = DragManager.Instance.CurrentDrag_ModuleRetinue;
            CardSpell cs = DragManager.Instance.CurrentDrag_CardSpell;
            if (mr != null)
            {
                AttackFactor attackFactor = CheckModuleRetinueCanAttackMe(mr);
                if (attackFactor > 0)
                {
                    ShipBG.SetActive(true);
                    if (DragManager.Instance.CurrentArrow && DragManager.Instance.CurrentArrow is ArrowAiming)
                    {
                        ((ArrowAiming) DragManager.Instance.CurrentArrow).IsOnHover = true; //箭头动画
                    }

                    string factorText = (int) attackFactor > 1 ? "x" + attackFactor : "";
                    string text = DragManager.Instance.DragOutDamage == 0 ? "" : "-" + DragManager.Instance.DragOutDamage + factorText;
                    if (DamageNumberPreviewTextMesh) DamageNumberPreviewTextMesh.text = text;
                    if (DamageNumberPreviewBGTextMesh) DamageNumberPreviewBGTextMesh.text = text;

                    string desc = GameManager.Instance.isEnglish ? AttackFactorDesc_en[attackFactor] : AttackFactorDesc[attackFactor];
                    if (Desc) Desc.text = desc;
                    if (DescBG) DescBG.text = desc;
                }
            }
            else if (cs != null && CheckCardSpellCanTarget(cs))
            {
                ShipBG.SetActive(true);
                if (DragManager.Instance.CurrentArrow && DragManager.Instance.CurrentArrow is ArrowAiming)
                {
                    ((ArrowAiming) DragManager.Instance.CurrentArrow).IsOnHover = true; //箭头动画
                }

                string text = DragManager.Instance.DragOutDamage == 0 ? "" : "-" + DragManager.Instance.DragOutDamage;
                if (DamageNumberPreviewTextMesh) DamageNumberPreviewTextMesh.text = text;
                if (DamageNumberPreviewBGTextMesh) DamageNumberPreviewBGTextMesh.text = text;
                if (Desc) Desc.text = "";
                if (DescBG) DescBG.text = "";
            }
        }
    }

    /// <summary>
    /// 当对方场上有随从，剑及近战攻击无法攻击ship；当对方场上有Defence，枪无法攻击ship；SniperGun随时可以攻击Ship
    /// </summary>
    /// <param name="attackRetinue"></param>
    /// <returns></returns>
    public AttackFactor CheckModuleRetinueCanAttackMe(ModuleRetinue attackRetinue)
    {
        if (attackRetinue.ClientPlayer == ClientPlayer) return AttackFactor.None;
        if (attackRetinue.M_Weapon)
        {
            switch (attackRetinue.M_Weapon.M_WeaponType)
            {
                case WeaponTypes.Sword:
                    if (ClientPlayer.MyBattleGroundManager.BattleGroundIsEmpty) return AttackFactor.Sword;
                    return 0;
                case WeaponTypes.Gun:
                    if (attackRetinue.M_RetinueWeaponEnergy != 0)
                    {
                        if (ClientPlayer.MyBattleGroundManager.HasDefenceRetinue) return 0;
                        return AttackFactor.Gun;
                    }
                    else
                    {
                        if (ClientPlayer.MyBattleGroundManager.BattleGroundIsEmpty) return AttackFactor.Sword;
                        return 0;
                    }
                case WeaponTypes.SniperGun:
                    if (attackRetinue.M_RetinueWeaponEnergy != 0) return AttackFactor.Gun;
                    else
                    {
                        if (ClientPlayer.MyBattleGroundManager.BattleGroundIsEmpty) return AttackFactor.Sword;
                        return 0;
                    }
            }
        }
        else
        {
            if (ClientPlayer.MyBattleGroundManager.BattleGroundIsEmpty) return AttackFactor.Sword;
        }

        return AttackFactor.None;
    }

    public enum AttackFactor
    {
        None = 0,
        Sword = 2,
        Gun = 1,
    }

    public static Dictionary<AttackFactor, string> AttackFactorDesc_en = new Dictionary<AttackFactor, string>
    {
        {AttackFactor.None, ""},
        {AttackFactor.Sword, "Sword Double"},
        {AttackFactor.Gun, ""},
    };

    public static Dictionary<AttackFactor, string> AttackFactorDesc = new Dictionary<AttackFactor, string>
    {
        {AttackFactor.None, ""},
        {AttackFactor.Sword, "近战翻倍"},
        {AttackFactor.Gun, ""},
    };

    private bool CheckCardSpellCanTarget(CardSpell card)
    {
        if (card.ClientPlayer == ClientPlayer)
        {
            return card.targetRetinueRange == TargetSideEffect.TargetRange.All ||
                   card.targetRetinueRange == TargetSideEffect.TargetRange.Ships ||
                   card.targetRetinueRange == TargetSideEffect.TargetRange.SelfShip;
        }
        else
        {
            return card.targetRetinueRange == TargetSideEffect.TargetRange.All ||
                   card.targetRetinueRange == TargetSideEffect.TargetRange.Ships ||
                   card.targetRetinueRange == TargetSideEffect.TargetRange.EnemyShip;
        }
    }

    public void MouseHoverComponent_OnHover1Begin(Vector3 mousePosition)
    {
    }

    public void MouseHoverComponent_OnHover1End()
    {
    }

    public void MouseHoverComponent_OnHover2Begin(Vector3 mousePosition)
    {
    }

    public void MouseHoverComponent_OnHover2End()
    {
    }

    public void MouseHoverComponent_OnFocusBegin(Vector3 mousePosition)
    {
    }

    public void MouseHoverComponent_OnFocusEnd()
    {
    }


    public void MouseHoverComponent_OnMousePressLeaveImmediately()
    {
        ShipBG.SetActive(false);
        if (DragManager.Instance.CurrentArrow && DragManager.Instance.CurrentArrow is ArrowAiming)
        {
            ((ArrowAiming) DragManager.Instance.CurrentArrow).IsOnHover = false; //箭头动画
        }

        if (DamageNumberPreviewTextMesh) DamageNumberPreviewTextMesh.text = "";
        if (DamageNumberPreviewBGTextMesh) DamageNumberPreviewBGTextMesh.text = "";
    }
}