using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBuffManager : MonoBehaviour
{
    private PlayerBuffManager()
    {
    }

    internal ClientPlayer ClientPlayer;
    [SerializeField] private Transform Content;

    Dictionary<int, PlayerBuff> PlayerBuffs = new Dictionary<int, PlayerBuff>(); //客户端buff按ID进行更改和存储，堆叠等逻辑在服务端处理好分发正确的buffID给客户端，而客户端不考虑堆叠等逻辑。
    HashSet<int> PlayerBuffID_PrepPass = new HashSet<int>(); //buffID 接到协议后预存

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

    public void UpdatePlayerBuff(SideEffectExecute buffSee, int buffId)
    {
        if (PlayerBuffID_PrepPass.Contains(buffId))
        {
            BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_UpdateBuff(buffSee, buffId), "Co_UpdateBuff");
        }
        else
        {
            PlayerBuffID_PrepPass.Add(buffId);
            BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_AddBuff(buffSee, buffId), "Co_AddBuff");
        }
    }

    IEnumerator Co_UpdateBuff(SideEffectExecute buffSee, int buffId)
    {
        if (PlayerBuffs.ContainsKey(buffId))
        {
            yield return PlayerBuffs[buffId].Co_UpdateValue(buffSee);
        }

        yield return null;
        BattleEffectsManager.Instance.Effect_Main.EffectEnd();
    }


    IEnumerator Co_AddBuff(SideEffectExecute buffSee, int buffId)
    {
        PlayerBuff pb = GameObjectPoolManager.Instance.Pool_PlayerBuffPool.AllocateGameObject<PlayerBuff>(Content);
        AudioManager.Instance.SoundPlay("sfx/OnBuffAdd", 0.5f);
        pb.SetRotation(ClientPlayer.WhichPlayer);
        pb.Init(buffSee, buffId);
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
            AudioManager.Instance.SoundPlay("sfx/OnBuffRemove", 0.5f);
            BattleEffectsManager.Instance.Effect_Main.EffectEnd();
            yield return null;
        }
        else
        {
            yield return null;
        }
    }
}