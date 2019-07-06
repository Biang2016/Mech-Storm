using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class LevelPropertyForm_Bonus : PoolObject
{
    [SerializeField] private Button DeleteButton;
    [SerializeField] private Button EditButton;
    [SerializeField] private Image Pic;
    [SerializeField] private Image EditBG;
    [SerializeField] private Text TypeLabel;
    [SerializeField] private Text TypeText;
    [SerializeField] private Text NameLabel;
    [SerializeField] private Text NameText;

    public override void PoolRecycle()
    {
        base.PoolRecycle();
        Cur_Bonus = null;
        EditButton.onClick.RemoveAllListeners();
        DeleteButton.onClick.RemoveAllListeners();
    }

    void Awake()
    {
        LanguageManager.Instance.RegisterTextKey(TypeLabel, "LevelEditorPanel_ShopItemTypeLabel");
        LanguageManager.Instance.RegisterTextKey(NameLabel, "LevelEditorPanel_ShopItemNameLabel");
    }

    public Bonus Cur_Bonus;

    private bool isEdit;

    public bool IsEdit
    {
        get { return isEdit; }
        set
        {
            isEdit = value;
            EditBG.enabled = isEdit;
        }
    }

    public void Initialize(Bonus bonus, UnityAction onEditButtonClick, UnityAction onDeleteButtonClick)
    {
        Cur_Bonus = bonus;
        EditButton.onClick.RemoveAllListeners();
        EditButton.onClick.AddListener(onEditButtonClick);
        DeleteButton.onClick.RemoveAllListeners();
        DeleteButton.onClick.AddListener(onDeleteButtonClick);

        ClientUtils.ChangeImagePicture(Pic, Cur_Bonus.PicID);
        TypeText.text = Cur_Bonus.BonusType.ToString();
        NameText.text = Utils.TextMeshProColorStringConvertToText(Cur_Bonus.GetDesc());
        if (Cur_Bonus is Bonus_UnlockCardByID b_UnlockCardByID)
        {
            ClientUtils.ChangeImagePicture(Pic, AllCards.GetPicIDByCardID(b_UnlockCardByID.CardID));
        }
    }

    public void Refresh()
    {
        TypeText.text = Cur_Bonus.BonusType.ToString();
        NameText.text = Utils.TextMeshProColorStringConvertToText(Cur_Bonus.GetDesc());
        if (Cur_Bonus is Bonus_UnlockCardByID b_UnlockCardByID)
        {
            ClientUtils.ChangeImagePicture(Pic, AllCards.GetPicIDByCardID(b_UnlockCardByID.CardID));
        }
    }

    public void OnLanguageChange()
    {
        Refresh();
    }
}