using UnityEngine.UI;

/// <summary>
/// 代表已选卡片在右侧卡组中的按钮
/// </summary>
public class BuildButton : PoolObject
{
    public override void PoolRecycle()
    {
        IsSelected = false;
        IsEdit = false;
        base.PoolRecycle();
    }

    void Awake()
    {
    }

    public RawImage SelectedStar;
    public Image ButtonImage;
    public Button Button;
    public Text Text_CardDeckName;
    public Text Text_Count;

    public BuildInfo BuildInfo;

    public void Initialize(BuildInfo buildInfo)
    {
        BuildInfo = buildInfo;
        Text_CardDeckName.text = BuildInfo.BuildName;
        Text_Count.text = BuildInfo.M_BuildCards.GetCardIDs().Count.ToString();
    }

    private bool isSelected;

    public bool IsSelected
    {
        get { return isSelected; }
        set
        {
            if (isSelected && !value)
            {
                OnUnselected();
            }

            else if (!isSelected && value)
            {
                OnSelected();
            }

            isSelected = value;
        }
    }

    private bool isEdit;

    public bool IsEdit
    {
        get { return isEdit; }
        set
        {
            if (isEdit && !value)
            {
                OnUnEdit();
            }

            else if (!isEdit && value)
            {
                OnEdit();
            }

            isEdit = value;
        }
    }

    private void OnSelected()
    {
        SelectedStar.enabled = true;
    }

    private void OnUnselected()
    {
        SelectedStar.enabled = false;
    }

    private void OnEdit()
    {
        ButtonImage.color = GameManager.Instance.BuildButtonEditColor;
    }

    private void OnUnEdit()
    {
        ButtonImage.color = GameManager.Instance.BuildButtonDefaultColor;
    }

    public void AddCard(int cardId)
    {
        BuildInfo.M_BuildCards.CardSelectInfos.TryGetValue(cardId, out BuildInfo.BuildCards.CardSelectInfo csi);
        if (csi != null)
        {
            csi.CardSelectCount += 1;
        }

        Text_Count.text = BuildInfo.CardCount.ToString();
    }

    public void RemoveCard(int cardId)
    {
        BuildInfo.M_BuildCards.CardSelectInfos.TryGetValue(cardId, out BuildInfo.BuildCards.CardSelectInfo csi);
        if (csi != null)
        {
            if (csi.CardSelectCount >= 1)
            {
                csi.CardSelectCount -= 1;
            }
        }

        Text_Count.text = BuildInfo.CardCount.ToString();
    }

    public void RefreshCardCountText()
    {
        Text_Count.text = BuildInfo.CardCount.ToString();
    }
}