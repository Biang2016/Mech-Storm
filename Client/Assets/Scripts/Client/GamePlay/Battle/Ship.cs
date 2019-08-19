using TMPro;
using UnityEngine;

public class Ship : MonoBehaviour, IMouseHoverComponent
{
    internal ClientPlayer ClientPlayer;
    public Transform[] M_Ship_AttackPoints;

    [SerializeField] private TextMeshPro DamageNumberPreviewTextMesh;
    [SerializeField] private TextMeshPro Desc;

    [SerializeField] private Transform[] Trans_NeedRotate180ByPlayer;
    [SerializeField] private Transform[] Trans_NeedRotateY180ByPlayer;
    public ShipStyleManager ShipStyleManager;

    public void Initialize(ClientPlayer clientPlayer)
    {
        ResetAll();
        foreach (Transform t in Trans_NeedRotate180ByPlayer)
        {
            if (t != null)
            {
                t.localRotation = Quaternion.Euler(clientPlayer.WhichPlayer == Players.Enemy ? 180 : 0, clientPlayer.WhichPlayer == Players.Enemy ? 180 : 0, t.localRotation.z);
            }
        }

        foreach (Transform t in Trans_NeedRotateY180ByPlayer)
        {
            if (t != null)
            {
                t.localRotation = Quaternion.Euler(0, clientPlayer.WhichPlayer == Players.Enemy ? 180 : 0, t.localRotation.z);
            }
        }

        ClientPlayer = clientPlayer;
        DamageNumberPreviewTextMesh.gameObject.SetActive(clientPlayer.WhichPlayer == Players.Enemy);
        Desc.gameObject.SetActive(clientPlayer.WhichPlayer == Players.Enemy);
        ShipStyleManager.Initialize(ClientPlayer);
    }

    public void ResetAll()
    {
        StopAllCoroutines();
        ClientPlayer = null;
        DamageNumberPreviewTextMesh.text = "";
        Desc.text = "";
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
        if (ClientPlayer.WhichPlayer == Players.Enemy)
        {
            if (DragManager.Instance.CurrentDrag)
            {
                ModuleMech mr = DragManager.Instance.CurrentDrag_ModuleMech;
                CardSpell cs = DragManager.Instance.CurrentDrag_CardSpell;
                if (mr != null)
                {
                    AttackFactor attackFactor = CheckModuleMechCanAttackMe(mr);
                    if (attackFactor > 0)
                    {
                        ShipStyleManager.ShowShipShapeHover(true);
                        if (DragManager.Instance.CurrentArrow && DragManager.Instance.CurrentArrow is ArrowAiming)
                        {
                            ((ArrowAiming) DragManager.Instance.CurrentArrow).IsOnHover = true; //箭头动画
                        }

                        string factorText = (int) attackFactor > 1 ? "x" + (int) attackFactor : "";
                        string text = DragManager.Instance.DragOutDamage == 0 ? "" : "-" + DragManager.Instance.DragOutDamage + factorText;
                        DamageNumberPreviewTextMesh.text = text;
                        Desc.text = "";
                    }
                }
                else if (cs != null && CheckCardSpellCanTarget(cs))
                {
                    ShipStyleManager.ShowShipShapeHover(true);
                    if (DragManager.Instance.CurrentArrow && DragManager.Instance.CurrentArrow is ArrowAiming)
                    {
                        ((ArrowAiming) DragManager.Instance.CurrentArrow).IsOnHover = true; //箭头动画
                    }

                    string text = DragManager.Instance.DragOutDamage == 0 ? "" : "-" + DragManager.Instance.DragOutDamage;
                    DamageNumberPreviewTextMesh.text = text;
                    Desc.text = "";
                }
            }
        }
    }

    /// <summary>
    /// 当对方场上有机甲，剑及近战攻击无法攻击ship；当对方场上有Defence，枪无法攻击ship；SniperGun随时可以攻击Ship
    /// </summary>
    /// <param name="attackMech"></param>
    /// <returns></returns>
    public AttackFactor CheckModuleMechCanAttackMe(ModuleMech attackMech)
    {
        if (attackMech.ClientPlayer == ClientPlayer) return AttackFactor.None;
        if (attackMech.MechEquipSystemComponent.M_Weapon)
        {
            switch (attackMech.MechEquipSystemComponent.M_Weapon.M_WeaponType)
            {
                case WeaponTypes.Sword:
                    if (ClientPlayer.BattlePlayer.BattleGroundManager.BattleGroundIsEmpty) return AttackFactor.Sword;
                    return 0;
                case WeaponTypes.Gun:
                    if (attackMech.M_MechWeaponEnergy != 0)
                    {
                        if (ClientPlayer.BattlePlayer.BattleGroundManager.HasDefenceMech) return 0;
                        return AttackFactor.Gun;
                    }
                    else
                    {
                        if (ClientPlayer.BattlePlayer.BattleGroundManager.BattleGroundIsEmpty) return AttackFactor.Sword;
                        return 0;
                    }
                case WeaponTypes.SniperGun:
                    if (attackMech.M_MechWeaponEnergy != 0) return AttackFactor.Gun;
                    else
                    {
                        if (ClientPlayer.BattlePlayer.BattleGroundManager.BattleGroundIsEmpty) return AttackFactor.Sword;
                        return 0;
                    }
            }
        }
        else
        {
            if (ClientPlayer.BattlePlayer.BattleGroundManager.BattleGroundIsEmpty) return AttackFactor.Sword;
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
            return (card.CardInfo.TargetInfo.targetMechRange & TargetRange.SelfShip) == TargetRange.SelfShip;
        }
        else
        {
            return (card.CardInfo.TargetInfo.targetMechRange & TargetRange.EnemyShip) == TargetRange.EnemyShip;
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
        if (ClientPlayer.WhichPlayer == Players.Enemy)
        {
            ShipStyleManager.ShowShipShapeHover(false);
            if (DragManager.Instance.CurrentArrow && DragManager.Instance.CurrentArrow is ArrowAiming)
            {
                ((ArrowAiming) DragManager.Instance.CurrentArrow).IsOnHover = false; //箭头动画
            }

            DamageNumberPreviewTextMesh.text = "";
            Desc.text = "";
        }
    }
}