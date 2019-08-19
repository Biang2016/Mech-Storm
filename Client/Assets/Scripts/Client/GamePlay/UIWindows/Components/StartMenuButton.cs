using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class StartMenuButton : PoolObject
{
    [SerializeField] private Button Button;
    [SerializeField] private Text Text;
    [SerializeField] private GameObject Tip;
    [SerializeField] private Text TipText;
    [SerializeField] private Image TipImage;
    [SerializeField] private Text TipImageText;

    public enum TipImageType
    {
        None,
        NewCard,
    }

    [SerializeField] private Sprite NewCardSprite;

    private Dictionary<TipImageType, (Sprite, string)> TipImageSpriteDict = new Dictionary<TipImageType, (Sprite, string)>();

    void Awake()
    {
        TipImageSpriteDict.Add(TipImageType.NewCard, (NewCardSprite, "StartMenu_NewCardTipText"));
        SetTipImageTextShow(false);
    }

    public override void PoolRecycle()
    {
        base.PoolRecycle();
        LanguageManager.Instance.UnregisterText(Text);
        LanguageManager.Instance.UnregisterText(TipText);
        LanguageManager.Instance.UnregisterText(TipImageText);
        SetTipImageTextShow(false);
    }

    public void BindTextKey(string textKey, string tipTextKey, UnityAction buttonClick, TipImageType tipImageType)
    {
        Tip.SetActive(tipTextKey != null);
        LanguageManager.Instance.RegisterTextKey(Text, textKey);
        if (tipTextKey != null)
        {
            LanguageManager.Instance.RegisterTextKey(TipText, tipTextKey);
        }

        if (buttonClick != null) Button.onClick.AddListener(buttonClick);
        if (tipImageType == TipImageType.None)
        {
            SetTipImageTextShow(false);
        }
        else
        {
            TipImage.sprite = TipImageSpriteDict[tipImageType].Item1;
            LanguageManager.Instance.RegisterTextKey(TipImageText, TipImageSpriteDict[tipImageType].Item2);
        }
    }

    public void SetTipImageTextShow(bool isShow)
    {
        TipImage.gameObject.SetActive(isShow);
        TipImageText.gameObject.SetActive(isShow);
    }
}