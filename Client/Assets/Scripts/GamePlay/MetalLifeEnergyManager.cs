using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MetalLifeEnergyManager : MonoBehaviour
{
    internal ClientPlayer ClientPlayer;

    [SerializeField] private GameObject MetalNumberBlock;
    private GameObject GoNumberSet_MetalNumber;
    private CardNumberSet NumberSet_MetalNumber;

    [SerializeField] private Text LifeNumber;
    [SerializeField] private Text TotalLifeNumber;
    [SerializeField] private Text EnergyNumber;
    [SerializeField] private Text TotalEnergyNumber;


    void Awake()
    {
        initiateNumbers(ref GoNumberSet_MetalNumber, ref NumberSet_MetalNumber, NumberSize.Big, CardNumberSet.TextAlign.Center, MetalNumberBlock);
    }

    public void ResetAll()
    {
        ClientPlayer = null;
        if (MetalBarManager)
        {
            MetalBarManager.ResetAll();
        }
    }

    private void initiateNumbers(ref GameObject Number, ref CardNumberSet cardNumberSet, NumberSize numberType, CardNumberSet.TextAlign textAlign, GameObject block)
    {
        if (!Number)
        {
            Number = GameObjectPoolManager.Instance.Pool_CardNumberSetPool.AllocateGameObject(block.transform);
            cardNumberSet = Number.GetComponent<CardNumberSet>();
            cardNumberSet.initiate(0, numberType, textAlign, false);
        }
        else
        {
            cardNumberSet = Number.GetComponent<CardNumberSet>();
            cardNumberSet.initiate(0, numberType, textAlign, false);
        }
    }

    private void initiateNumbers(ref GameObject Number, ref CardNumberSet cardNumberSet, NumberSize numberType, CardNumberSet.TextAlign textAlign, GameObject block, char firstSign)
    {
        if (!Number)
        {
            Number = GameObjectPoolManager.Instance.Pool_CardNumberSetPool.AllocateGameObject(block.transform);
            cardNumberSet = Number.GetComponent<CardNumberSet>();
            cardNumberSet.initiate(firstSign, 0, numberType, textAlign, false);
        }
        else
        {
            cardNumberSet = Number.GetComponent<CardNumberSet>();
            cardNumberSet.initiate(firstSign, 0, numberType, textAlign, false);
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
        BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_OnLifeChange(value, change), "Co_OnLifeChange");
    }

    IEnumerator Co_OnLifeChange(int value, int change)
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
        else
        {
            LifeNumberFly.SetText(change.ToString(), "#FFFFFF", "#FFFFFF", TextFly.FlyDirection.Down);
            HitManager.Instance.ShowHit(LifeIcon.transform, HitManager.HitType.Blade, ClientUtils.HTMLColorToColor("#FFFFFF"), 0.2f);
            AudioManager.Instance.SoundPlay("sfx/OnHitShip");
        }

        yield return null;
        BattleEffectsManager.Instance.Effect_Main.EffectEnd();
    }

    public void SetTotalLife(int value)
    {
        TotalLifeNumber.text = "/" + value;
    }

    public void SetEnergy(int value, int change)
    {
        BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_OnEnergyChange(value, change), "Co_OnEnergyChange");
    }

    public void SetTotalEnergy(int value)
    {
        TotalEnergyNumber.text = "/" + value;
    }


    public void OnEnergyChange(int change)
    {
    }

    IEnumerator Co_OnEnergyChange(int value, int change)
    {
        EnergyNumber.text = value.ToString();
        EnergyBar.fillAmount = (float) value / ClientPlayer.EnergyMax;
        EnergyIconAnim.SetTrigger("Jump");
        EnergyIconAnim.SetTrigger("Reset");
        EnergyNumberAnim.SetTrigger("Jump");
        EnergyTotalNumberAnim.SetTrigger("Jump");
        ClientPlayer.MyHandManager.RefreshAllCardUsable();

        if (change > 0)
        {
            EnergyNumberFly.SetText("+" + change, "#FFFFFF", "#FFFFFF", TextFly.FlyDirection.Up);
        }
        else
        {
            EnergyNumberFly.SetText(change.ToString(), "#FFFFFF", "#FFFFFF", TextFly.FlyDirection.Down);
        }

        yield return null;
        BattleEffectsManager.Instance.Effect_Main.EffectEnd();
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