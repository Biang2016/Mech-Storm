using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardEditorPanel : BaseUIForm
{
    private CardEditorPanel()
    {
    }

    void Awake()
    {
        UIType.IsClearStack = false;
        UIType.IsESCClose = false;
        UIType.IsClickElsewhereClose = false;
        UIType.UIForms_Type = UIFormTypes.Fixed;
        UIType.UIForm_LucencyType = UIFormLucencyTypes.ImPenetrable;
        UIType.UIForms_ShowMode = UIFormShowModes.HideOther;
        UIType.IsClearStack = true;

        LanguageManager.Instance.RegisterTextKeys(
            new List<(Text, string)>
            {
            });
    }

    [SerializeField] private Transform CardPreviewContainer;

    public void OnButtonClick()
    {
        CardBase.InstantiateCardByCardInfo(AllCards.GetCard(0), CardPreviewContainer, CardBase.CardShowMode.CardSelect);
    }

    void Start()
    {
    }

    void Update()
    {
    }
}