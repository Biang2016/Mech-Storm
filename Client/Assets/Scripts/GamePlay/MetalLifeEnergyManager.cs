using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MetalLifeEnergyManager : MonoBehaviour
{
    internal ClientPlayer ClientPlayer;

    [SerializeField] private Transform MetalNumberBlock;
    private CardNumberSet NumberSet_MetalNumber;

    [SerializeField] private Text LifeNumber;
    [SerializeField] private Text TotalLifeNumber;
    [SerializeField] private Text EnergyNumber;
    [SerializeField] private Text TotalEnergyNumber;


    void Awake()
    {
        ClientUtils.InitiateNumbers(ref NumberSet_MetalNumber, NumberSize.Big, CardNumberSet.TextAlign.Center, MetalNumberBlock);
    }

    public void ResetAll()
    {
        ClientPlayer = null;
        if (MetalBarManager)
        {
            MetalBarManager.ResetAll();
        }
    }

    public void SetMetal(int value)
    {
        MetalBarManager.ClientPlayer = ClientPlayer;
        MetalBarManager.SetMetalNumber(value);
        NumberSet_MetalNumber.Number = value;
        MetalNumberBlock.transform.localPosition = Vector3.Lerp(MetalNumberMinPos.localPosition, MetalNumberMaxPos.localPosition, (float) value / GamePlaySettings.MaxMetal);
        ClientPlayer.MyHandManager.RefreshAllCardUsable();
    }

    public void SetLife(int value, int change)
    {
        LifeNumber.text = value.ToString();
        LifeBar.fillAmount = (float) value / ClientPlayer.LifeMax;
        LifeIconAnim.SetTrigger("Jump");
        LifeIconAnim.SetTrigger("Reset");
        LifeNumberAnim.SetTrigger("Jump");
        LifeTotalNumberAnim.SetTrigger("Jump");

        if (change > 0)
        {
            LifeNumberFly.SetText("+" + change, "#FFFFFF", "#FFFFFF", TextFly.FlyDirection.Up);
        }
        else if (change < 0)
        {
            LifeNumberFly.SetText(change.ToString(), "#FFFFFF", "#FFFFFF", TextFly.FlyDirection.Down);
            HitManager.Instance.ShowHit(LifeIcon.transform, HitManager.HitType.Blade, "#FFFFFF", 0.2f);
            AudioManager.Instance.SoundPlay("sfx/OnHitShip");
            AudioManager.Instance.SoundPlay("sfx/OnHitShipDuuu");
        }
    }

    public void SetTotalLife(int value)
    {
        TotalLifeNumber.text = "/" + value;
    }

    public void SetEnergy(int value, int change)
    {
        ClientPlayer.MyHandManager.RefreshAllCardUsable();
        EnergyNumber.text = value.ToString();
        EnergyBar.fillAmount = (float) value / ClientPlayer.EnergyMax;
        EnergyIconAnim.SetTrigger("Jump");
        EnergyIconAnim.SetTrigger("Reset");
        EnergyNumberAnim.SetTrigger("Jump");
        EnergyTotalNumberAnim.SetTrigger("Jump");
        if (change > 0)
        {
            EnergyNumberFly.SetText("+" + change, "#00D2FF", "#00D2FF", TextFly.FlyDirection.Up);
        }
        else if (change < 0)
        {
            EnergyNumberFly.SetText(change.ToString(), "#00D2FF", "#00D2FF", TextFly.FlyDirection.Down);
        }
    }

    public void SetTotalEnergy(int value)
    {
        TotalEnergyNumber.text = "/" + value;
    }


    public void OnEnergyChange(int change)
    {
    }

    [SerializeField] private Transform MetalNumberMinPos;
    [SerializeField] private Transform MetalNumberMaxPos;
    [SerializeField] private Image LifeIcon;
    [SerializeField] private TextFlyPile LifeNumberFly;
    [SerializeField] private Image LifeBar;
    [SerializeField] private Image LifeTrough;
    [SerializeField] private Animator LifeIconAnim;
    [SerializeField] private Animator LifeNumberAnim;
    [SerializeField] private Animator LifeTotalNumberAnim;
    [SerializeField] private Image EnergyIcon;
    [SerializeField] private TextFlyPile EnergyNumberFly;
    [SerializeField] private Image EnergyBar;
    [SerializeField] private Image EnergyTrough;
    [SerializeField] private Animator EnergyIconAnim;
    [SerializeField] private Animator EnergyNumberAnim;
    [SerializeField] private Animator EnergyTotalNumberAnim;

    public MetalBarManager MetalBarManager;
}