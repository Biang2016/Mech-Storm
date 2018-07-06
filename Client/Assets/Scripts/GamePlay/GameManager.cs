using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _gm;

    public static GameManager GM
    {
        get
        {
            if (!_gm) _gm = FindObjectOfType<GameManager>();
            return _gm;
        }
    }


    private void Awake()
    {
        initializeColors();
    }

    private void Start()
    {
        AllCard = new AllCards();
        _rbe = Camera.main.GetComponent<ImageEffectBlurBox>();
        RoundManager.RM.GameStart();
    }

    private void Update()
    {
    }


    #region GamePlay

    internal AllCards AllCard;

    internal ClientPlayer SelfClientPlayer;
    internal ClientPlayer EnemyClientPlayer;

    public void InitializePlayers(PlayerResponse r)
    {
        if (r.ClinetId == NetworkManager.NM.SelfClientId)
        {
            SelfClientPlayer = new ClientPlayer(r.CostMax, r.CostLeft, Players.Self);
        }
        else
        {
            EnemyClientPlayer= new ClientPlayer(r.CostMax, r.CostLeft, Players.Enemy);
        }
    }

    public void SetPlayersCost(PlayerCostResponse r)
    {
        if (r.ClinetId == NetworkManager.NM.SelfClientId)
        {
            SelfClientPlayer.DoChangeCost(r);
        }
        else
        {
            EnemyClientPlayer.DoChangeCost(r);
        }
    }

    #endregion

    #region 游戏全局参数

    public bool CanTestEnemyCards = false; //开启敌方卡牌可见、可操作

    public float HandCardSize = 1.0f;
    public float HandCardInterval = 1.0f;
    public float HandCardRotate = 1.0f;
    public float HandCardOffset = 0.6f;
    public float PullOutCardSize = 3.0f;
    public float DetailCardSize = 3.0f;
    public float DetailCardModuleSize = 2.5f;
    public float DetailCardSizeRetinue = 4.0f;

    public float RetinueInterval = 3.5f;
    public float RetinueDetailPreviewDelaySeconds = 0.7f;

    public Color HeroColor;
    public Color RetinueColor;
    public Color WeaponSwordColor;
    public Color WeaponGunColor;
    public Color ShieldShieldColor;
    public Color ShieldArmorColor;
    public Color CardBloomColor;
    public Color CardDragBloomColor;

    private void initializeColors()
    {
        HeroColor = HTMLColorToColor("#787878FF");
        RetinueColor = HTMLColorToColor("#5BAEF4FF");
        WeaponSwordColor = HTMLColorToColor("#FF229DFF");
        WeaponGunColor = HTMLColorToColor("#FF0000FF");
        ShieldShieldColor = HTMLColorToColor("#E6FF00FF");
        ShieldArmorColor = HTMLColorToColor("#FF8E00FF");
        CardBloomColor = HTMLColorToColor("#00FF41FF");
        CardDragBloomColor = HTMLColorToColor("#FF04F8FB");
        Slot1Color = HTMLColorToColor("#FF0000FF");
        Slot2Color = HTMLColorToColor("#FFED00FF");
        Slot3Color = HTMLColorToColor("#00FF6BFF");
        Slot4Color = HTMLColorToColor("#2D37FFFF");
    }

    public Color Slot1Color;
    public Color Slot2Color;
    public Color Slot3Color;
    public Color Slot4Color;

    public static Color HTMLColorToColor(string htmlColor)
    {
        Color cl = new Color();
        ColorUtility.TryParseHtmlString(htmlColor, out cl);
        return cl;
    }

    #endregion

    #region 其他

    public AudioSource MainAudioSource;
    private ImageEffectBlurBox _rbe;

    public void PlayAudioClip(AudioClip ac)
    {
        MainAudioSource.clip = ac;
        MainAudioSource.Play();
    }

    public void StartBlurBackGround()
    {
        _rbe.enabled = true;
    }


    public void StopBlurBackGround()
    {
        _rbe.enabled = false;
    }

    #endregion
}