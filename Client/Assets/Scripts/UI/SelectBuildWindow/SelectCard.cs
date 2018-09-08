using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 代表已选卡片在右侧卡组中的按钮
/// </summary>
public class SelectCard : MonoBehaviour, IGameObjectPool
{
    private GameObjectPool gameObjectPool;

    public void PoolRecycle()
    {
        Count = 0;
        gameObjectPool.RecycleGameObject(gameObject);
    }

    void Awake()
    {
        gameObjectPool = GameObjectPoolManager.Instance.Pool_SelectCardPool;
    }



    public Transform Tran_Metal;
    public Transform Tran_Energy;
    public Transform Tran_Metal_s;
    public Transform Tran_Energy_s;
    public Transform Tran_NOCOST;

    public Text Text_Metal;
    public Text Text_Energy;
    public Text Text_Metal_s;
    public Text Text_Energy_s;
    public Text Text_NOCOST;

    public Text Text_MetalBG;
    public Text Text_EnergyBG;
    public Text Text_Metal_sBG;
    public Text Text_Energy_sBG;
    public Text Text_NOCOSTBG;


    public Text Text_Count;
    public Button CardButton;
    public Text Text_CardName;

    private int count;

    public int Count
    {
        get { return count; }

        set
        {
            count = value;
            Text_Count.text = "×" + count.ToString();
        }
    }

    private int metal;

    public int Metal
    {
        get { return metal; }

        set
        {
            metal = value;
            RefreshText();
        }
    }


    private int energy;

    public int Energy
    {
        get { return energy; }

        set
        {
            energy = value;
            RefreshText();
        }
    }

    private void RefreshText()
    {
        if (Metal != 0 && Energy != 0)
        {
            Tran_Metal.gameObject.SetActive(false);
            Tran_Energy.gameObject.SetActive(false);
            Tran_Metal_s.gameObject.SetActive(true);
            Tran_Energy_s.gameObject.SetActive(true);
            Tran_NOCOST.gameObject.SetActive(false);

            Text_Metal_s.text = Metal.ToString();
            Text_Energy_s.text = Energy.ToString();
            Text_Metal_sBG.text = Metal.ToString();
            Text_Energy_sBG.text = Energy.ToString();
        }
        else if (Metal == 0 && Energy != 0)
        {
            Tran_Metal.gameObject.SetActive(false);
            Tran_Energy.gameObject.SetActive(true);
            Tran_Metal_s.gameObject.SetActive(false);
            Tran_Energy_s.gameObject.SetActive(false);
            Tran_NOCOST.gameObject.SetActive(false);

            Text_Energy.text = Energy.ToString();
            Text_EnergyBG.text = Energy.ToString();
        }
        else if (Metal != 0 && Energy == 0)
        {
            Tran_Metal.gameObject.SetActive(true);
            Tran_Energy.gameObject.SetActive(false);
            Tran_Metal_s.gameObject.SetActive(false);
            Tran_Energy_s.gameObject.SetActive(false);
            Tran_NOCOST.gameObject.SetActive(false);

            Text_Metal.text = Metal.ToString();
            Text_MetalBG.text = Metal.ToString();
        }
        else
        {
            Tran_Metal.gameObject.SetActive(false);
            Tran_Energy.gameObject.SetActive(false);
            Tran_Metal_s.gameObject.SetActive(false);
            Tran_Energy_s.gameObject.SetActive(false);
            Tran_NOCOST.gameObject.SetActive(true);

            Text_NOCOST.text = "-";
            Text_NOCOSTBG.text = "-";
        }
    }
}