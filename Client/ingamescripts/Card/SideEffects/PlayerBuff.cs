using System.Collections;
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

    public void Init(SideEffectExecute buffSee, int buffId)
    {
        PlayerBuffSideEffects buff = ((PlayerBuffSideEffects) buffSee.SideEffectBase);
        int buffValue = GetBuffValue(buffSee, buff);

        BuffValueText.text = buffValue == 0 ? "" : buffValue.ToString();
        BuffId = buffId;
        ClientUtils.ChangeCardPicture(Image, buff.BuffPicId);
        Color buffColor = ClientUtils.HTMLColorToColor(((PlayerBuffSideEffects) (AllBuffs.GetBuff((buff.Name)).SideEffectBase)).BuffColor);
        BuffBloom.color = buffColor;
        BuffDescText.color = buffColor;
        BuffValuePanel.enabled = buff.HasNumberShow;
        BuffValueText.enabled = buff.HasNumberShow;
        BuffAnim.SetTrigger("Add");
        BuffDescText.text = buff.GenerateDesc(GameManager.Instance.IsEnglish);
    }

    public IEnumerator Co_UpdateValue(SideEffectExecute buffSee)
    {
        BuffDescText.text = ((PlayerBuffSideEffects) buffSee.SideEffectBase).GenerateDesc(GameManager.Instance.IsEnglish);
        PlayerBuffSideEffects buff = (PlayerBuffSideEffects) buffSee.SideEffectBase;
        int buffValue = GetBuffValue(buffSee, buff);

        if (buffValue >= 0)
        {
            BuffAnim.SetTrigger("Jump");
            AudioManager.Instance.SoundPlay("sfx/OnBuffTrigger", 0.7f);
        }

        yield return new WaitForSeconds(0.2f);
        BuffValueText.text = buffValue == 0 ? "" : buffValue.ToString();
        yield return new WaitForSeconds(0.1f);
        yield return null;
    }

    private int GetBuffValue(SideEffectExecute buffSee, PlayerBuffSideEffects buff)
    {
        int buffValue = 0;
        if (buff.CanPiled)
        {
            switch (buff.PiledBy)
            {
                case PlayerBuffSideEffects.BuffPiledBy.RemoveTriggerTimes:
                {
                    buffValue = buffSee.RemoveTriggerTimes;
                    break;
                }
                case PlayerBuffSideEffects.BuffPiledBy.Value:
                {
                    buffValue = ((IEffectFactor) buff.Sub_SideEffect[0]).Values[0].Value;
                    break;
                }
            }
        }
        else
        {
            buffValue = buffSee.RemoveTriggerTimes;
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
}