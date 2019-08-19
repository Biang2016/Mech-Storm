using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class StoryPlayerInformationPanel : BaseUIForm
{
    [SerializeField] private Text CrystalText;

    void Awake()
    {
        UIType.InitUIType(
            isClearStack: false,
            isESCClose: false,
            isClickElsewhereClose: false,
            uiForms_Type: UIFormTypes.Fixed,
            uiForms_ShowMode: UIFormShowModes.Normal,
            uiForm_LucencyType: UIFormLucencyTypes.Penetrable);
    }

    public void SetCrystal(int crystal)
    {
        CrystalText.text = crystal.ToString();
    }
}