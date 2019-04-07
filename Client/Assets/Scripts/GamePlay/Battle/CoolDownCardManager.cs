using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoolDownCardManager : MonoBehaviour
{
    private CoolDownCardManager()
    {
    }

    internal ClientPlayer ClientPlayer;
    [SerializeField] private Transform Content;

    Dictionary<int, CoolDownCardIcon> CoolDownCardIcons = new Dictionary<int, CoolDownCardIcon>();
    HashSet<int> CoolDownCardIcons_Prepass = new HashSet<int>();

    void Start()
    {
    }

    public void ResetAll()
    {
        foreach (KeyValuePair<int, CoolDownCardIcon> kv in CoolDownCardIcons)
        {
            kv.Value.PoolRecycle();
        }

        CoolDownCardIcons.Clear();
        CoolDownCardIcons_Prepass.Clear();
    }

    public void UpdateCoolDownCard(CardDeck.CoolingDownCard cdc)
    {
        if (CoolDownCardIcons_Prepass.Contains(cdc.CardInstanceID))
        {
            BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_UpdateCoolDownCard(cdc), "Co_UpdateCoolDownCard");
        }
        else
        {
            CoolDownCardIcons_Prepass.Add(cdc.CardInstanceID);
            BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_AddCoolDownCard(cdc), "Co_AddCoolDownCard");
        }
    }

    IEnumerator Co_UpdateCoolDownCard(CardDeck.CoolingDownCard cdc)
    {
        if (CoolDownCardIcons.ContainsKey(cdc.CardInstanceID))
        {
            yield return CoolDownCardIcons[cdc.CardInstanceID].Co_UpdateValue(cdc);
        }

        yield return null;
        BattleEffectsManager.Instance.Effect_Main.EffectEnd();
    }

    IEnumerator Co_AddCoolDownCard(CardDeck.CoolingDownCard cdc)
    {
        CoolDownCardIcon cdci = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.CoolDownCard].AllocateGameObject<CoolDownCardIcon>(Content);
        cdci.transform.SetAsFirstSibling();
        cdci.SetRotation(ClientPlayer.WhichPlayer);
        cdci.Init(cdc, ClientPlayer);
        CoolDownCardIcons.Add(cdc.CardInstanceID, cdci);
        yield return new WaitForSeconds(0.2f);
        BattleEffectsManager.Instance.Effect_Main.EffectEnd();
        yield return null;
    }

    public void RemoveCoolDownCard(int cardInstanceID)
    {
        BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_RemoveCoolDownCard(cardInstanceID), "Co_RemoveCoolDownCard");
    }

    IEnumerator Co_RemoveCoolDownCard(int cardInstanceID)
    {
        if (CoolDownCardIcons.ContainsKey(cardInstanceID))
        {
            CoolDownCardIcons[cardInstanceID].OnRemove();
            yield return new WaitForSeconds(0.3f);
            CoolDownCardIcons[cardInstanceID].PoolRecycle();
            CoolDownCardIcons.Remove(cardInstanceID);
            BattleEffectsManager.Instance.Effect_Main.EffectEnd();
            yield return null;
        }
        else
        {
            yield return null;
        }
    }
}