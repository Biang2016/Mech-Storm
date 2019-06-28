using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MetalLifeEnergyManager : MonoBehaviour
{
    internal ClientPlayer ClientPlayer;

    public MetalBarManager MetalBarManager;

    [SerializeField] private TextMeshPro MetalNumberText;

    [SerializeField] private Transform CenterTrans;
    [SerializeField] private Transform LeftTrans;
    [SerializeField] private Transform RightTrans;
    [SerializeField] private Text LifeText;
    [SerializeField] private Text LifeNumber;
    [SerializeField] private Text TotalLifeNumber;
    [SerializeField] private Text EnergyText;
    [SerializeField] private Text EnergyNumber;
    [SerializeField] private Text TotalEnergyNumber;

    [SerializeField] private GameObject PlayerIcon;
    [SerializeField] private Image PlayerIconImage;

    void Awake()
    {
        LanguageManager.Instance.RegisterTextKeys(
            new List<ValueTuple<Text, string>>
            {
                (LifeText, "Ship_LifeText"),
                (EnergyText, "Ship_EnergyText"),
            });
    }

    public void Initialize(ClientPlayer clientPlayer)
    {
        ResetAll();
        ClientPlayer = clientPlayer;
        MetalBarManager.Initialize(ClientPlayer);
    }

    public void ResetAll()
    {
        ClientPlayer = null;
        MetalBarManager.ResetAll();
    }

    public void SetEnemyIconImage()
    {
        if (ClientPlayer.WhichPlayer == Players.Enemy)
        {
            PlayerIcon.SetActive(RoundManager.Instance.M_PlayMode == RoundManager.PlayMode.Single);
            if (RoundManager.Instance.M_PlayMode == RoundManager.PlayMode.Single)
            {
                ClientUtils.ChangeImagePicture(PlayerIconImage, 0);
            }
        }
    }

    public void SetMetal(int value)
    {
        MetalBarManager.SetMetalNumber(value);
        MetalNumberText.text = value.ToString();
        MetalNumberText.transform.localPosition = Vector3.Lerp(MetalNumberMinPos.localPosition, MetalNumberMaxPos.localPosition, (float) value / GamePlaySettings.MaxMetal);
        ClientPlayer.BattlePlayer.HandManager.RefreshAllCardUsable();
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
            FXManager.Instance.PlayFX(LifeIcon.transform, FXManager.FXType.FX_Blade, "#FFFFFF", 0.3f, 1.5f);
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
        ClientPlayer.BattlePlayer.HandManager.RefreshAllCardUsable();
        EnergyNumber.text = value.ToString();
        EnergyBar.fillAmount = (float) value / ClientPlayer.EnergyMax;
        EnergyIconAnim.SetTrigger("Jump");
        EnergyIconAnim.SetTrigger("Reset");
        EnergyNumberAnim.SetTrigger("Jump");
        EnergyTotalNumberAnim.SetTrigger("Jump");
        if (change > 0)
        {
            EnergyNumberFly.SetText("+" + change, "#00D2FF", "#00D2FF", TextFly.FlyDirection.Up);
            FXManager.Instance.PlayFX(RightTrans, FXManager.FXType.FX_ShipAddEnergy, "#FFFFFF", 0.7f, 4);
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
}