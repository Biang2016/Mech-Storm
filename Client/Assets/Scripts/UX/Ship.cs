using UnityEngine;

public class Ship : MonoBehaviour, IMouseHoverComponent
{
    internal ClientPlayer ClientPlayer;
    public Transform[] M_Ship_AttackPoints;

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
        if (Desc) Desc.text = "";
        if (DescBG) DescBG.text = "";
    }

    public Vector3 GetClosestHitPosition(Vector3 from)
    {
        float min_distance = 999;
        Vector3 closestHitPos = Vector3.zero;
        foreach (Transform hitPoint in M_Ship_AttackPoints)
        {
            float dis = Vector3.Magnitude(hitPoint.position - from);
            if (dis < min_distance)
            {
                min_distance = dis;
                closestHitPos = hitPoint.position;
            }
        }

        return closestHitPos;
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

                    string factorText = (int) attackFactor > 1 ? "x" + (int) attackFactor : "";
                    string text = DragManager.Instance.DragOutDamage == 0 ? "" : "-" + DragManager.Instance.DragOutDamage + factorText;
                    if (DamageNumberPreviewTextMesh) DamageNumberPreviewTextMesh.text = text;
                    if (DamageNumberPreviewBGTextMesh) DamageNumberPreviewBGTextMesh.text = text;

                    if (Desc) Desc.text = "";
                    if (DescBG) DescBG.text = "";
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
    /// 当对方场上有机甲，剑及近战攻击无法攻击ship；当对方场上有Defence，枪无法攻击ship；SniperGun随时可以攻击Ship
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
        Sword = 1,
        Gun = 1,
    }

    private bool CheckCardSpellCanTarget(CardSpell card)
    {
        if (card.ClientPlayer == ClientPlayer)
        {
            return (card.CardInfo.TargetInfo.targetRetinueRange & TargetSideEffect.TargetRange.SelfShip) == TargetSideEffect.TargetRange.SelfShip;
        }
        else
        {
            return (card.CardInfo.TargetInfo.targetRetinueRange & TargetSideEffect.TargetRange.EnemyShip) == TargetSideEffect.TargetRange.EnemyShip;
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
        if (Desc) Desc.text = "";
        if (DescBG) DescBG.text = "";
    }
}