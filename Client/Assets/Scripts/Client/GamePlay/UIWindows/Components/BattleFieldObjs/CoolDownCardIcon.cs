using System.Collections;
using UnityEngine;
using UnityEngine.UI;

internal class CoolDownCardIcon : PoolObject
{
    public override void PoolRecycle()
    {
        CoolDownRoundText.text = "";
        base.PoolRecycle();
    }

    public ClientPlayer ClientPlayer;

    public CardDeck.CoolingDownCard M_CoolingDownCard;

    public void Init(CardDeck.CoolingDownCard cdc, ClientPlayer clientPlayer)
    {
        ClientPlayer = clientPlayer;
        M_CoolingDownCard = cdc;
        CoolDownRoundText.text = cdc.LeftRounds.ToString();
        ClientUtils.ChangeImagePicture(Image, AllCards.GetCard(cdc.CardID).BaseInfo.PictureID);
        CooldDownIconAnim.SetTrigger("Add");
    }

    public IEnumerator Co_UpdateValue(CardDeck.CoolingDownCard cdc)
    {
        M_CoolingDownCard = cdc;
        if (cdc.LeftRounds >= 0)
        {
            CooldDownIconAnim.SetTrigger("Jump");
        }

        yield return new WaitForSeconds(0.2f);
        CoolDownRoundText.text = cdc.LeftRounds.ToString();
        yield return new WaitForSeconds(0.1f);
        yield return null;
    }

    public void OnRemove()
    {
        CooldDownIconAnim.SetTrigger("Remove");
    }

    public void SetRotation(Players whichPlayer)
    {
        RotatePanel.localRotation = Quaternion.Euler(whichPlayer == Players.Self ? 0 : 180, whichPlayer == Players.Self ? 0 : 180, 0);
    }

    [SerializeField] private Transform RotatePanel;
    [SerializeField] private Image CoolDownBloom;
    [SerializeField] private Image CoolDownPanel;
    [SerializeField] private Text CoolDownRoundText;
    [SerializeField] private Animator CooldDownIconAnim;
    [SerializeField] private Image Image;

    public void OnHover()
    {
        BattleManager.Instance.ShowCardDetailInBattleManager.ShowCardDetail(AllCards.GetCard(M_CoolingDownCard.CardID), ClientPlayer.WhichPlayer == Players.Self ? ShowCardDetailInBattleManager.ShowPlaces.LeftLower : ShowCardDetailInBattleManager.ShowPlaces.RightUpper);
    }

    public void OnExit()
    {
        BattleManager.Instance.ShowCardDetailInBattleManager.HideCardDetail();
    }
}