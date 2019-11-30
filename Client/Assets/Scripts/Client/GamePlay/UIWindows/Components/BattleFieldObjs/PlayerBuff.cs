using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

internal class PlayerBuff : PoolObject
{
    public override void PoolRecycle()
    {
        BuffValueText.text = "";
        base.PoolRecycle();
    }

    [SerializeField] private int BuffId;

    private PlayerBuffSideEffects Cur_Buff;

    public void Init(SideEffectExecute buffSee, int buffId)
    {
        Cur_Buff = ((PlayerBuffSideEffects) buffSee.SideEffectBases[0]);
        int buffValue = GetBuffValue(buffSee, Cur_Buff);

        BuffValueText.text = buffValue == 0 ? "" : buffValue.ToString();
        BuffId = buffId;
        ClientUtils.ChangeImagePicture(Image, Cur_Buff.M_SideEffectParam.GetParam_ConstInt("BuffPicId"));
        Color buffColor = ClientUtils.HTMLColorToColor(AllBuffs.GetBuff((Cur_Buff.Name)).M_SideEffectParam.GetParam_String("BuffColor"));
        BuffBloom.color = buffColor;
        BuffDescText.color = buffColor;
        BuffValuePanel.enabled = Cur_Buff.M_SideEffectParam.GetParam_Bool("HasNumberShow");
        BuffValueText.enabled = Cur_Buff.M_SideEffectParam.GetParam_Bool("HasNumberShow");
        BuffAnim.SetTrigger("Add");
        BuffDescText.text = Utils.TextMeshProColorStringConvertToText(Cur_Buff.GenerateDesc());
    }

    public IEnumerator Co_UpdateValue(SideEffectExecute buffSee, PlayerBuffUpdateRequest.UpdateTypes updateType)
    {
        BuffDescText.text = Utils.TextMeshProColorStringConvertToText(((PlayerBuffSideEffects) buffSee.SideEffectBases[0]).GenerateDesc());
        PlayerBuffSideEffects buff = (PlayerBuffSideEffects) buffSee.SideEffectBases[0];
        int buffValue = GetBuffValue(buffSee, buff);

        if (buffValue >= 0)
        {
            if (updateType == PlayerBuffUpdateRequest.UpdateTypes.Trigger)
            {
                BuffAnim.SetTrigger("Jump");
                AudioManager.Instance.SoundPlay("sfx/OnBuffTrigger", 0.7f);
            }

            else if (updateType == PlayerBuffUpdateRequest.UpdateTypes.Refresh)
            {
                BuffAnim.SetTrigger("Rotate");
                AudioManager.Instance.SoundPlay("sfx/OnBuffTrigger", 0.7f);
            }
        }

        yield return new WaitForSeconds(0.2f);
        BuffValueText.text = buffValue == 0 ? "" : buffValue.ToString();
        yield return new WaitForSeconds(0.1f);
        yield return null;
    }

    private int GetBuffValue(SideEffectExecute buffSee, PlayerBuffSideEffects buff)
    {
        int buffValue = 0;
        if (buff.M_SideEffectParam.GetParam_Bool("CanPiled"))
        {
            switch ((PlayerBuffSideEffects.BuffPiledBy) buff.M_SideEffectParam.GetParam_ConstInt("PiledBy"))
            {
                case PlayerBuffSideEffects.BuffPiledBy.RemoveTriggerTimes:
                {
                    buffValue = buffSee.M_ExecuteSetting.RemoveTriggerTimes;
                    break;
                }
                case PlayerBuffSideEffects.BuffPiledBy.RemoveTriggerDelayTimes:
                {
                    buffValue = buffSee.M_ExecuteSetting.RemoveTriggerDelayTimes;
                    break;
                }
                case PlayerBuffSideEffects.BuffPiledBy.Value:
                {
                    //TODO
                    buffValue = 0;
//                    buffValue =  ((IEffectFactor) buff.Sub_SideEffect[0]).Values[0].Value;
                    break;
                }
            }
        }
        else
        {
            buffValue = buffSee.M_ExecuteSetting.RemoveTriggerTimes;
        }

        return buffValue;
    }

    public void OnRemove()
    {
        BuffAnim.SetTrigger("Remove");
    }

    public void SetRotation(Players whichPlayer)
    {
        RotatePanel.localRotation = Quaternion.Euler(whichPlayer == Players.Self ? 0 : 180, whichPlayer == Players.Self ? 0 : 180, 0);
        BuffDescText.transform.localRotation = Quaternion.Euler(whichPlayer == Players.Self ? 0 : 180, whichPlayer == Players.Self ? 0 : 180, 0);
    }

    [SerializeField] private Transform RotatePanel;
    [SerializeField] private Image BuffBloom;
    [SerializeField] private Image BuffValuePanel;
    [SerializeField] private Text BuffValueText;
    [SerializeField] private Text BuffDescText;
    [SerializeField] private Animator BuffDescAnim;
    [SerializeField] private Animator BuffAnim;
    [SerializeField] private Image Image;

    public void OnLanguageChange()
    {
        BuffDescText.text = Utils.TextMeshProColorStringConvertToText(Cur_Buff.GenerateDesc());
    }
}