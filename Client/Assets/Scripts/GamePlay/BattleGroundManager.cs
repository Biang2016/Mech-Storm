using System.Collections.Generic;
using UnityEngine;

internal class BattleGroundManager : MonoBehaviour
{
    internal bool BattleGroundIsFull;
    private Vector3 _defaultRetinuePosition = Vector3.zero;

    internal ClientPlayer ClientPlayer;
    private List<ModuleRetinue> Retinues = new List<ModuleRetinue>();

    private void Awake()
    {
    }

    private void Start()
    {
    }

    private void Update()
    {
    }

    public void Reset()
    {
        foreach (ModuleRetinue moduleRetinue in Retinues)
        {
            moduleRetinue.PoolRecycle();
        }

        ClientPlayer = null;
        Retinues.Clear();
    }


    internal int ComputePosition(Vector3 dragLastPosition)
    {
        int index = Mathf.RoundToInt(Mathf.Floor(dragLastPosition.x / GameManager.GM.RetinueInterval - (Retinues.Count + 1) % 2 * 0.5f) + (Retinues.Count / 2 + 1));
        if (index < 0) index = 0;
        if (index >= Retinues.Count) index = Retinues.Count;
        return index;
    }

    internal ModuleRetinue CheckRetinueOnPosition(Vector3 dragLastPosition)
    {
        var index = Mathf.RoundToInt(Mathf.Floor(dragLastPosition.x / GameManager.GM.RetinueInterval - (Retinues.Count + 1) % 2 * 0.5f) + (Retinues.Count / 2 + 1));
        if (index < 0 || index >= Retinues.Count)
            return null;
        return Retinues[index];
    }

    public void AddRetinue(CardInfo_Retinue retinueCardInfo, int retinuePlaceIndex)
    {
        if (ClientPlayer == null) return;
        ModuleRetinue retinue = GameObjectPoolManager.GOPM.Pool_ModuleRetinuePool.AllocateGameObject(transform).GetComponent<ModuleRetinue>();
        retinue.Initiate(retinueCardInfo, ClientPlayer);
        retinue.transform.Rotate(Vector3.up, 180);
        Retinues.Insert(retinuePlaceIndex, retinue);
        retinue.M_RetinuePlaceIndex = retinuePlaceIndex;
        BattleGroundIsFull = Retinues.Count == GamePlaySettings.MaxRetinueNumber;
        RefreshBattleGround();
    }


    public void RemoveRetinue(int retinuePlaceIndex)
    {
        ModuleRetinue retinue = Retinues[retinuePlaceIndex];
        retinue.PoolRecycle();
        Retinues.RemoveAt(retinuePlaceIndex);
        BattleGroundIsFull = Retinues.Count == GamePlaySettings.MaxRetinueNumber;
        RefreshBattleGround();
    }

    public void EquipWeapon(CardInfo_Weapon cardInfo, int battleGroundIndex)
    {
        ModuleRetinue retinue = GetRetinue(battleGroundIndex);
        ModuleWeapon newModueWeapon = GameObjectPoolManager.GOPM.Pool_ModuleWeaponPool.AllocateGameObject(retinue.transform).GetComponent<ModuleWeapon>();
        newModueWeapon.M_ModuleRetinue = retinue;
        newModueWeapon.M_RetinuePlaceIndex = battleGroundIndex;
        newModueWeapon.Initiate(cardInfo, ClientPlayer);
        retinue.M_Weapon = newModueWeapon;
    }

    public void EquipShield(CardInfo_Shield cardInfo, int battleGroundIndex)
    {
        ModuleRetinue retinue = GetRetinue(battleGroundIndex);
        ModuleShield newModuleShield = GameObjectPoolManager.GOPM.Pool_ModuleShieldPool.AllocateGameObject(retinue.transform).GetComponent<ModuleShield>();
        newModuleShield.M_ModuleRetinue = retinue;
        newModuleShield.M_RetinuePlaceIndex = battleGroundIndex;
        newModuleShield.Initiate(cardInfo, ClientPlayer);
        retinue.M_Shield = newModuleShield;
    }

    internal void RefreshBattleGround()
    {
        var count = 0;
        foreach (var retinue in Retinues)
        {
            retinue.transform.localPosition = _defaultRetinuePosition;
            retinue.transform.Translate(Vector3.left * (count - Retinues.Count / 2.0f + 0.5f) * GameManager.GM.RetinueInterval);
            count++;
        }
    }


    #region Utils

    public ModuleRetinue GetRetinue(int retinuePlaceIndex)
    {
        return Retinues[retinuePlaceIndex];
    }

    public int GetRetinuePlaceIndex(ModuleRetinue moduleRetinue)
    {
        return Retinues.IndexOf(moduleRetinue);
    }

    #endregion

    #region GameProcess

    internal void BeginRound()
    {
        foreach (var mr in Retinues) mr.OnBeginRound();
    }

    internal void EndRound()
    {
        foreach (var mr in Retinues) mr.OnEndRound();
    }

    #endregion
}