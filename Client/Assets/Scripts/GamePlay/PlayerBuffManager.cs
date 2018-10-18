using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBuffManager : MonoBehaviour
{
    private PlayerBuffManager()
    {
    }

    internal ClientPlayer ClientPlayer;
    [SerializeField] private Transform Content;

    Dictionary<int, PlayerBuff> PlayerBuffs = new Dictionary<int, PlayerBuff>();

    void Start()
    {
    }

    public void ResetAll()
    {
        foreach (KeyValuePair<int, PlayerBuff> kv in PlayerBuffs)
        {
            PlayerBuffs[kv.Key].PoolRecycle();
        }

        PlayerBuffs.Clear();
    }

    public void UpdatePlayerBuff(int buffId, int buffValue, int buffPicId, bool hasNumberShow)
    {
        if (PlayerBuffs.ContainsKey(buffId))
        {
            PlayerBuffs[buffId].BuffValue = buffValue;
        }
        else
        {
            BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_AddBuff(buffId, buffValue, buffPicId, hasNumberShow), "Co_AddBuff");
        }
    }

    IEnumerator Co_AddBuff(int buffId, int buffValue, int buffPicId, bool hasNumberShow)
    {
        PlayerBuff pb = GameObjectPoolManager.Instance.Pool_PlayerBuffPool.AllocateGameObject<PlayerBuff>(Content);
        pb.UpdateValue(buffPicId, buffId, buffValue, hasNumberShow);
        PlayerBuffs.Add(buffId, pb);
        yield return new WaitForSeconds(0.2f);
        BattleEffectsManager.Instance.Effect_Main.EffectEnd();
        yield return null;
    }

    public void RemovePlayerBuff(int buffId)
    {
        BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_RemoveBuff(buffId), "Co_RemoveBuff");
    }

    IEnumerator Co_RemoveBuff(int buffId)
    {
        if (PlayerBuffs.ContainsKey(buffId))
        {
            PlayerBuffs[buffId].OnRemove();
            yield return new WaitForSeconds(0.3f);
            BattleEffectsManager.Instance.Effect_Main.EffectEnd();
            yield return null;
            PlayerBuffs[buffId].PoolRecycle();
            PlayerBuffs.Remove(buffId);
        }
        else
        {
            yield return null;
        }
    }
}