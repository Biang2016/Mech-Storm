using UnityEngine;
using UnityEngine.UI;

internal class SmallBonusItem : BonusItem_Base
{
    [SerializeField] private Sprite AdjustDeckIcon;
    [SerializeField] private Sprite LifeIcon;
    [SerializeField] private Sprite EnergyIcon;
    [SerializeField] private Sprite BudgetIcon;

    [SerializeField] private GameObject CardImageContainer;
    [SerializeField] private GameObject IconImageContainer;
    [SerializeField] private Image CardImageBorder;
    [SerializeField] private Image CardImage;
    [SerializeField] private Animator CardImageBorderAnim;


    public override void Initialize(Bonus bonus)
    {
        base.Initialize(bonus);
        IconImageContainer.SetActive(true);
        CardImageContainer.SetActive(false);
        switch (bonus.M_BonusType)
        {
            case Bonus.BonusType.AdjustDeck:
            {
                ItemImage.sprite = AdjustDeckIcon;
                ItemImage.preserveAspect = true;
                break;
            }
            case Bonus.BonusType.LifeUpperLimit:
            {
                ItemImage.sprite = LifeIcon;
                ItemImage.color = GameManager.Instance.LifeIconColor;
                ItemImage.preserveAspect = true;
                break;
            }
            case Bonus.BonusType.EnergyUpperLimit:
            {
                ItemImage.sprite = EnergyIcon;
                ItemImage.color = GameManager.Instance.EnergyIconColor;
                ItemImage.preserveAspect = true;
                break;
            }
            case Bonus.BonusType.Budget:
            {
                ItemImage.sprite = BudgetIcon;
                ItemImage.color = Color.white;
                ItemImage.preserveAspect = true;
                break;
            }
            case Bonus.BonusType.UnlockCardByID:
            {
                IconImageContainer.SetActive(false);
                CardImageContainer.SetActive(true);
                ClientUtils.ChangePicture(CardImage, BonusCardInfo.BaseInfo.PictureID);
                CardImage.color = Color.white;
                CardImageBorder.color = ClientUtils.ChangeColorToWhite(ClientUtils.HTMLColorToColor(BonusCardInfo.GetCardColor()), 0.5f);
                CardImage.preserveAspect = true;
                break;
            }
        }
    }

    public override void OnHover()
    {
        base.OnHover();
        if (BonusCardInfo != null)
        {
            CardImageBorderAnim.SetTrigger("Hover");
            WinLostPanelManager.Instance.ShowCardPreview(BonusCardInfo);
            AudioManager.Instance.SoundPlay("sfx/BonusHover");
        }
    }

    public override void OnExit()
    {
        base.OnExit();
        if (BonusCardInfo != null)
        {
            CardImageBorderAnim.SetTrigger("Exit");
            WinLostPanelManager.Instance.HideCardPreview();
        }
    }
}