using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LevelPropertyForm_BonusGroups : PropertyFormRow
{
    [SerializeField] private Transform BonusGroupContainer;
    [SerializeField] private Button AddButton;
    [SerializeField] private Button ClearButton;
    private UnityAction<bool, int, int, LevelEditorPanel.SelectCardContents> OnStartSelectCard;

    public override void PoolRecycle()
    {
        base.PoolRecycle();
        Clear();
        AddButton.onClick.RemoveAllListeners();
        ClearButton.onClick.RemoveAllListeners();
        OnStartSelectCard = null;
    }

    void Awake()
    {
        LanguageManager.Instance.RegisterTextKey(Label, "LevelEditorPanel_BonusGroups");
    }

    protected override void SetValue(string value_str)
    {
    }

    private List<BonusGroup> BonusGroups;
    private List<LevelPropertyForm_BonusGroup> M_BonusGroups = new List<LevelPropertyForm_BonusGroup>();
    private IEnumerator Co_refresh;

    public void OnCurEditBonusUnlockCardChangeCard(int cardID)
    {
        Cur_BonusGroup.OnCurEditBonusUnlockCardChangeCard(cardID);
    }

    public void Initialize(List<BonusGroup> bonusGroups, IEnumerator co_refresh, UnityAction addAction, UnityAction clearAction, UnityAction<bool, int, int, LevelEditorPanel.SelectCardContents> onStartSelectCard)
    {
        Co_refresh = co_refresh;
        AddButton.onClick.RemoveAllListeners();
        AddButton.onClick.AddListener(addAction);
        ClearButton.onClick.RemoveAllListeners();
        ClearButton.onClick.AddListener(clearAction);
        OnStartSelectCard = onStartSelectCard;

        BonusGroups = bonusGroups;
        Refresh();
    }

    private void Clear()
    {
        foreach (LevelPropertyForm_BonusGroup bg in M_BonusGroups)
        {
            bg.PoolRecycle();
        }

        Cur_BonusGroup = null;
        M_BonusGroups.Clear();
        OnStartSelectCard?.Invoke(false, (int) AllCards.EmptyCardTypes.NoCard, 0, LevelEditorPanel.SelectCardContents.SelectBonusCards);
    }

    public LevelPropertyForm_BonusGroup Cur_BonusGroup;

    public void Refresh()
    {
        Clear();
        foreach (BonusGroup bg in BonusGroups)
        {
            LevelPropertyForm_BonusGroup bg_row = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.LevelPropertyForm_BonusGroup].AllocateGameObject<LevelPropertyForm_BonusGroup>(BonusGroupContainer);
            bg_row.Initialize(bg, Co_refresh,
                deleteAction: delegate
                {
                    BonusGroups.Remove(bg);
                    Refresh();
                }, clearAction: delegate
                {
                    bg.Bonuses.Clear();
                    bg_row.Refresh();
                }, onEditAction: delegate { Cur_BonusGroup = bg_row; },
                onStartSelectCard: OnStartSelectCard);
            M_BonusGroups.Add(bg_row);
        }

        UIManager.Instance.GetBaseUIForm<LevelEditorPanel>().StartCoroutine(Co_refresh);
    }

    public void OnLanguageChange()
    {
        foreach (LevelPropertyForm_BonusGroup bg in M_BonusGroups)
        {
            bg.OnLanguageChange();
        }
    }
}