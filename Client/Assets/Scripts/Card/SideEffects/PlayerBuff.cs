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

    public void Init(PlayerBuffSideEffects buffInfo, int buffId, int buffValue)
    {
        BuffId = buffId;
        ClientUtils.ChangePicture(Image, buffInfo.BuffPicId);
        BuffValueText.text = buffValue == 0 ? "" : buffValue.ToString();
        BuffBloom.color = ClientUtils.HTMLColorToColor(buffInfo.BuffColor);
        BuffValuePanel.enabled = buffInfo.HasNumberShow;
        BuffValueText.enabled = buffInfo.HasNumberShow;
        BuffAnim.SetTrigger("Add");
    }

    public void UpdateValue(int buffValue)
    {
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