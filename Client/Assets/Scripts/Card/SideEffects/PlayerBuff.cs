using System.Collections;
using System.Collections.Generic;
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

    public void Init(SideEffectExecute buff_see, int buffId, int buffValue)
    {
        PlayerBuffSideEffects buff = ((PlayerBuffSideEffects) buff_see.SideEffectBase);
        BuffId = buffId;
        ClientUtils.ChangePicture(Image, buff.BuffPicId);
        BuffValueText.text = buffValue == 0 ? "" : buffValue.ToString();
        Color buffColor = ClientUtils.HTMLColorToColor(((PlayerBuffSideEffects) (AllBuffs.GetBuff((buff.Name)).SideEffectBase)).BuffColor);
        BuffBloom.color = buffColor;
        BuffDescText.color = buffColor;
        BuffValuePanel.enabled = buff.HasNumberShow;
        BuffValueText.enabled = buff.HasNumberShow;
        BuffAnim.SetTrigger("Add");
        BuffDescText.text = buff.GenerateDesc(GameManager.Instance.isEnglish);
    }

    public void UpdateValue(SideEffectExecute buff, int buffValue)
    {
        BuffDescText.text = ((PlayerBuffSideEffects) buff.SideEffectBase).GenerateDesc(GameManager.Instance.isEnglish);
        BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_UpdateValue(buffValue), "Co_UpdateValue");
    }

    IEnumerator Co_UpdateValue(int buffValue)
    {
        if (buffValue >= 0)
        {
            BuffAnim.SetTrigger("Jump");
        }

        yield return new WaitForSeconds(0.4f);
        BuffValueText.text = buffValue == 0 ? "" : buffValue.ToString();
        yield return new WaitForSeconds(0.1f);

        BattleEffectsManager.Instance.Effect_Main.EffectEnd();
        yield return null;
    }

    public void OnRemove()
    {
        BuffAnim.SetTrigger("Remove");
    }

    public void SetRotation(Players whichPlayer)
    {
        RotatePanel.localRotation = Quaternion.Euler(whichPlayer == Players.Self ? 0 : 180, whichPlayer == Players.Self ? 0 : 180, 0);
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