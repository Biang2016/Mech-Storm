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

    Dictionary<int, PlayerBuff> PlayerBuffs = new Dictionary<int, PlayerBuff>(); //客户端buff按ID进行更改和存储，堆叠等逻辑在服务端处理好分发正确的buffID给客户端，而客户端不考虑堆叠等逻辑。

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

    public void UpdatePlayerBuff(SideEffectExecute buff, int buffId, int buffValue)
    {
        if (PlayerBuffs.ContainsKey(buffId))
        {
            PlayerBuffs[buffId].UpdateValue(buff,buffValue);
        }
        else
        {
            BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_AddBuff(buff, buffId, buffValue), "Co_AddBuff");
        }
    }

    IEnumerator Co_AddBuff(SideEffectExecute buff, int buffId, int buffValue)
    {
        PlayerBuff pb = GameObjectPoolManager.Instance.Pool_PlayerBuffPool.AllocateGameObject<PlayerBuff>(Content);
        pb.SetRotation(ClientPlayer.WhichPlayer);
        pb.Init(buff, buffId, buffValue);
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
            PlayerBuffs[buffId].PoolRecycle();
            PlayerBuffs.Remove(buffId);
            BattleEffectsManager.Instance.Effect_Main.EffectEnd();
            yield return null;
        }
        else
        {
            yield return null;
        }
    }
}