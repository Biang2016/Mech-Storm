using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 代表已选卡片在右侧卡组中的按钮
/// </summary>
public class BuildButton : MonoBehaviour, IGameObjectPool
{
    private GameObjectPool gameObjectPool;

    public void PoolRecycle()
    {
        IsSelected = false;
        IsEdit = false;
        gameObjectPool.RecycleGameObject(gameObject);
    }

    void Awake()
    {
        gameObjectPool = GameObjectPoolManager.Instance.Pool_BuildButtonPool;
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
        Text_Count.text = BuildInfo.CardIDs.Count.ToString();
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
        BuildInfo.CardIDs.Add(cardId);
        Text_Count.text = BuildInfo.CardCount().ToString();
    }

    public void RemoveCard(int cardId)
    {
        BuildInfo.CardIDs.Remove(cardId);
        Text_Count.text = BuildInfo.CardCount().ToString();
    }
}