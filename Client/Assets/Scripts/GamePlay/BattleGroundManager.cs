using System.Collections;
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
        previewRetinuePlace = -1;
        Retinues.Clear();
        removeRetinues.Clear();
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
        if (previewRetinuePlace != -1)
        {
            previewRetinuePlace = -1;
        }

        ModuleRetinue retinue = GameObjectPoolManager.GOPM.Pool_ModuleRetinuePool.AllocateGameObject(transform).GetComponent<ModuleRetinue>();
        retinue.Initiate(retinueCardInfo, ClientPlayer);
        retinue.transform.Rotate(Vector3.up, 180);
        Retinues.Insert(retinuePlaceIndex, retinue);
        retinue.M_RetinuePlaceIndex = retinuePlaceIndex;

        foreach (ModuleRetinue moduleRetinue in Retinues)
        {
            moduleRetinue.M_RetinuePlaceIndex = Retinues.IndexOf(moduleRetinue);
        }

        BattleGroundIsFull = Retinues.Count == GamePlaySettings.MaxRetinueNumber;
        SetNewRetinuePlace(retinuePlaceIndex);
        BattleEffectsManager.BEM.EffectsShow(Co_RefreshBattleGroundAnim(), "Co_RefreshBattleGroundAnim");
    }

    private int previewRetinuePlace;

    public void AddRetinuePreview(int placeIndex)
    {
        if (Retinues.Count == 0) return;
        if (previewRetinuePlace == -1 || previewRetinuePlace != placeIndex)
        {
            previewRetinuePlace = placeIndex;
            BattleEffectsManager.BEM.EffectsShow(Co_RefreshBattleGroundAnim(), "Co_RefreshBattleGroundAnim");
        }
    }

    public void RemoveRetinuePreview()
    {
        if (previewRetinuePlace != -1)
        {
            previewRetinuePlace = -1;
            BattleEffectsManager.BEM.EffectsShow(Co_RefreshBattleGroundAnim(), "Co_RefreshBattleGroundAnim");
        }
    }

    public void RemoveRetinue(int retinuePlaceIndex)
    {
        BattleEffectsManager.BEM.EffectsShow(Co_RemoveRetinue(retinuePlaceIndex), "Co_RemoveRetinue");
        BattleEffectsManager.BEM.EffectsShow(Co_RefreshBattleGroundAnim(), "Co_RefreshBattleGroundAnim");
    }

    List<ModuleRetinue> removeRetinues = new List<ModuleRetinue>();

    public void RemoveRetinueTogatherAdd(int retinuePlaceIndex)
    {
        ModuleRetinue retinue = Retinues[retinuePlaceIndex];
        retinue.PoolRecycle();
        removeRetinues.Add(retinue);
    }

    public void RemoveRetinueTogather()
    {
        foreach (ModuleRetinue removeRetinue in removeRetinues)
        {
            Retinues.Remove(removeRetinue);
            ClientLog.CL.Print("remove:" + removeRetinue.M_RetinuePlaceIndex);
        }

        removeRetinues.Clear();

        foreach (ModuleRetinue moduleRetinue in Retinues)
        {
            moduleRetinue.M_RetinuePlaceIndex = Retinues.IndexOf(moduleRetinue);
        }
        BattleGroundIsFull = Retinues.Count == GamePlaySettings.MaxRetinueNumber;
    }

    public void RemoveRetinueTogatherEnd()
    {
        BattleEffectsManager.BEM.EffectsShow(Co_RefreshBattleGroundAnim(), "Co_RefreshBattleGroundAnim");
    }

    IEnumerator Co_RemoveRetinue(int retinuePlaceIndex)
    {
        ModuleRetinue retinue = Retinues[retinuePlaceIndex];
        retinue.PoolRecycle();
        Retinues.Remove(retinue);
        foreach (ModuleRetinue moduleRetinue in Retinues)
        {
            moduleRetinue.M_RetinuePlaceIndex = Retinues.IndexOf(moduleRetinue);
        }

        BattleGroundIsFull = Retinues.Count == GamePlaySettings.MaxRetinueNumber;
        yield return null;
        BattleEffectsManager.BEM.EffectEnd();
    }

    public void EquipWeapon(CardInfo_Weapon cardInfo, int battleGroundIndex)
    {
        ModuleRetinue retinue = GetRetinue(battleGroundIndex);
        ModuleWeapon newModueWeapon = GameObjectPoolManager.GOPM.Pool_ModuleWeaponPool.AllocateGameObject(retinue.transform).GetComponent<ModuleWeapon>();
        newModueWeapon.M_ModuleRetinue = retinue;
        newModueWeapon.Initiate(cardInfo, ClientPlayer);
        retinue.M_Weapon = newModueWeapon;
    }

    public void EquipShield(CardInfo_Shield cardInfo, int battleGroundIndex)
    {
        ModuleRetinue retinue = GetRetinue(battleGroundIndex);
        ModuleShield newModuleShield = GameObjectPoolManager.GOPM.Pool_ModuleShieldPool.AllocateGameObject(retinue.transform).GetComponent<ModuleShield>();
        newModuleShield.M_ModuleRetinue = retinue;
        newModuleShield.Initiate(cardInfo, ClientPlayer);
        retinue.M_Shield = newModuleShield;
    }

    internal void SetNewRetinuePlace(int retinueIndex)
    {
        Retinues[retinueIndex].transform.localPosition = _defaultRetinuePosition;
        Retinues[retinueIndex].transform.transform.Translate(Vector3.left * (retinueIndex - Retinues.Count / 2.0f + 0.5f) * GameManager.GM.RetinueInterval, Space.Self);
    }

    IEnumerator Co_RefreshBattleGroundAnim()
    {
        float duration = 0.05f;
        float tick = 0;

        Vector3[] translations = new Vector3[Retinues.Count];

        int actualPlaceCount = previewRetinuePlace == -1 ? Retinues.Count : Retinues.Count + 1;

        List<ModuleRetinue> movingRetinues = new List<ModuleRetinue>();

        for (int i = 0; i < Retinues.Count; i++)
        {
            movingRetinues.Add(Retinues[i]);

            int actualPlace = i;
            if (previewRetinuePlace != -1 && i >= previewRetinuePlace)
            {
                actualPlace += 1;
            }

            Vector3 ori = Retinues[i].transform.localPosition;
            Vector3 offset = Vector3.left * (actualPlace - actualPlaceCount / 2.0f + 0.5f) * GameManager.GM.RetinueInterval;

            Retinues[i].transform.localPosition = _defaultRetinuePosition;
            Retinues[i].transform.Translate(offset, Space.Self);

            translations[i] = Retinues[i].transform.localPosition - ori;
            Retinues[i].transform.localPosition = ori;
        }


        while (tick <= duration)
        {
            float timeDelta = Mathf.Min(duration - tick, Time.deltaTime);

            for (int i = 0; i < movingRetinues.Count; i++)
            {
                movingRetinues[i].transform.localPosition += translations[i] * timeDelta / duration;
            }

            tick += Time.deltaTime;

            yield return null;
        }

        BattleEffectsManager.BEM.EffectEnd();
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