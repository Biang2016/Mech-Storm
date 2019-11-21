using UnityEngine;
using UnityEngine.UI;

public class ShopItemButton_Small : ShopItemButton
{
    [SerializeField] private Sprite LifeIcon;
    [SerializeField] private Sprite EnergyIcon;
    [SerializeField] private Sprite BudgetIcon;

    [SerializeField] private Image Icon;
    [SerializeField] private Text Amount;

    public override void Initialize(ShopItem shopItem)
    {
        base.Initialize(shopItem);
        switch (shopItem)
        {
            case ShopItem_Budget si_budget:
            {
                Icon.sprite = BudgetIcon;
                Icon.color = Color.white;
                Icon.preserveAspect = true;
                Amount.text = "x" + si_budget.Budget;
                break;
            }
            case ShopItem_LifeUpperLimit si_life:
            {
                Icon.sprite = LifeIcon;
                Icon.color = ClientUtils.GetColorFromColorDict(AllColors.ColorType.LifeIconColor);
                Icon.preserveAspect = true;
                Amount.text = "x" + si_life.LifeUpperLimit;
                break;
            }
            case ShopItem_EnergyUpperLimit si_energy:
            {
                Icon.sprite = EnergyIcon;
                Icon.color = ClientUtils.GetColorFromColorDict(AllColors.ColorType.EnergyIconColor);
                Icon.preserveAspect = true;
                Amount.text = "x" + si_energy.EnergyUpperLimit;
                break;
            }
        }
    }
}