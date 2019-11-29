using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LevelEditorPanel_CardComboList : PropertyFormRow
{
    [SerializeField] private Button AddCardComboButton;
    [SerializeField] private Button ClearAllCardComboButton;
    [SerializeField] private Button RefreshButton;

    [SerializeField] private Transform LevelEditorPanel_CardComboContainer;

    public List<CardCombo> Cur_CardComboList = new List<CardCombo>();
    public List<LevelEditorPanel_CardCombo> LevelEditorPanel_CardCombos = new List<LevelEditorPanel_CardCombo>();

    public override void PoolRecycle()
    {
        base.PoolRecycle();
        Clear();
        AddCardComboButton.onClick.RemoveAllListeners();
        ClearAllCardComboButton.onClick.RemoveAllListeners();
        RefreshButton.onClick.RemoveAllListeners();
        LanguageManager.Instance.UnregisterText(Label);
    }

    public void Initialize(List<CardCombo> cardComboList, UnityAction initializeParentPanel)
    {
        Clear();
        Cur_CardComboList = cardComboList;
        LanguageManager.Instance.RegisterTextKey(Label, "LevelEditorPanel_CardComboList");
        AddCardComboButton.onClick.RemoveAllListeners();
        AddCardComboButton.onClick.AddListener(delegate
        {
            CardCombo new_CardCombo = new CardCombo(new List<int>());
            Cur_CardComboList.Add(new_CardCombo);
            LevelEditorPanel_CardCombo cc = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.LevelEditorPanel_CardCombo].AllocateGameObject<LevelEditorPanel_CardCombo>(LevelEditorPanel_CardComboContainer);
            cc.Initialize(new_CardCombo,
                deleteAction: delegate(CardCombo _cc)
                {
                    Cur_CardComboList.Remove(_cc);
                    Initialize(Cur_CardComboList, initializeParentPanel);
                },
                moveUpButton: delegate(CardCombo _cc)
                {
                    int index = Cur_CardComboList.IndexOf(_cc);
                    if (index > 0)
                    {
                        Cur_CardComboList.Remove(_cc);
                        Cur_CardComboList.Insert(index - 1, _cc);
                        Initialize(Cur_CardComboList, initializeParentPanel);
                    }
                },
                moveDownButton: delegate(CardCombo _cc)
                {
                    int index = Cur_CardComboList.IndexOf(_cc);
                    if (index >= 0 && index < Cur_CardComboList.Count - 1)
                    {
                        Cur_CardComboList.Remove(_cc);
                        Cur_CardComboList.Insert(index + 1, _cc);
                        Initialize(Cur_CardComboList, initializeParentPanel);
                    }
                }
            );
            LevelEditorPanel_CardCombos.Add(cc);
            initializeParentPanel();
            StartCoroutine(ClientUtils.UpdateLayout(UIManager.Instance.GetBaseUIForm<LevelEditorPanel>().RightPanel));
        });
        ClearAllCardComboButton.onClick.RemoveAllListeners();
        ClearAllCardComboButton.onClick.AddListener(delegate
        {
            Cur_CardComboList.Clear();
            Clear();
        });
        RefreshButton.onClick.RemoveAllListeners();
        RefreshButton.onClick.AddListener(delegate { StartCoroutine(ClientUtils.UpdateLayout(UIManager.Instance.GetBaseUIForm<LevelEditorPanel>().RightPanel)); });

        foreach (CardCombo cardCombo in cardComboList)
        {
            LevelEditorPanel_CardCombo cc = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.LevelEditorPanel_CardCombo].AllocateGameObject<LevelEditorPanel_CardCombo>(LevelEditorPanel_CardComboContainer);
            cc.Initialize(cardCombo,
                deleteAction: delegate(CardCombo _cc)
                {
                    Cur_CardComboList.Remove(_cc);
                    Initialize(Cur_CardComboList, initializeParentPanel);
                }, moveUpButton: delegate(CardCombo _cc)
                {
                    int index = Cur_CardComboList.IndexOf(_cc);
                    if (index > 0)
                    {
                        Cur_CardComboList.Remove(_cc);
                        Cur_CardComboList.Insert(index - 1, _cc);
                        Initialize(Cur_CardComboList, initializeParentPanel);
                    }
                },
                moveDownButton: delegate(CardCombo _cc)
                {
                    int index = Cur_CardComboList.IndexOf(_cc);
                    if (index >= 0 && index < Cur_CardComboList.Count - 1)
                    {
                        Cur_CardComboList.Remove(_cc);
                        Cur_CardComboList.Insert(index + 1, _cc);
                        Initialize(Cur_CardComboList, initializeParentPanel);
                    }
                });
            LevelEditorPanel_CardCombos.Add(cc);
        }

        initializeParentPanel();

        StartCoroutine(ClientUtils.UpdateLayout(UIManager.Instance.GetBaseUIForm<LevelEditorPanel>().RightPanel));
    }

    public void Clear()
    {
        foreach (LevelEditorPanel_CardCombo cr in LevelEditorPanel_CardCombos)
        {
            cr.PoolRecycle();
        }

        LevelEditorPanel_CardCombos.Clear();
        StartCoroutine(ClientUtils.UpdateLayout(UIManager.Instance.GetBaseUIForm<LevelEditorPanel>().RightPanel));
    }

    protected override void SetValue(string value_str, bool forceChange = false)
    {
    }

    public void OnLanguageChange()
    {
        foreach (LevelEditorPanel_CardCombo c in LevelEditorPanel_CardCombos)
        {
            c.OnLanguageChange();
        }
    }
}