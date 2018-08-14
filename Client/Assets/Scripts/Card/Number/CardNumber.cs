using UnityEngine;
using System.Collections;

internal class CardNumber : MonoBehaviour,IGameObjectPool
{
    internal GameObjectPool gameObjectPool;

    public void PoolRecycle()
    {
        hasSign = false;
        SetNumberColor(GameManager.GM.DefaultLifeNumberColor);
        gameObjectPool.RecycleGameObject(gameObject);
    }

    [SerializeField] private Renderer Renderer;

    void Awake()
    {
        switch (MyNumberSize)
        {
            case NumberSize.Small:
                gameObjectPool = GameObjectPoolManager.GOPM.Pool_CardSmallNumberPool;
                break;
            case NumberSize.Medium:
                gameObjectPool = GameObjectPoolManager.GOPM.Pool_CardMediumNumberPool;
                break;
            case NumberSize.Big:
                gameObjectPool = GameObjectPoolManager.GOPM.Pool_CardBigNumberPool;
                break;
        }
    }

    void Start()
    {
        if (!hasSign) {
            Number = Number;
        }
    }

    [SerializeField] private Material[] NumberMaterial;

    [SerializeField] private Material[] NumberSignMaterial;

    [SerializeField] private Material[] NumberMaterial_Select;

    [SerializeField] private Material[] NumberSignMaterial_Select;

    int number;

    public int Number
    {
        get
        {
            return number;
        }

        set
        {
            if (value < 10 && value >= 0)
            {
                number = value;
                if (!IsSelect) Renderer.material = NumberMaterial[value];
                else Renderer.material = NumberMaterial_Select[value];
            }
        }
    }

    [SerializeField]private NumberSize MyNumberSize;

    internal bool IsSelect;//是否是选择卡牌面板上的数字（需换材质）

    private bool hasSign=false;
    public void SetSign(char sign)
    {
        hasSign = true;
        int signIndex = 0;
        switch (sign)
        {
            case '+': signIndex = 0; break;
            case '-': signIndex = 1; break;
            case 'x': signIndex = 2; break;
            case '/': signIndex = 3; break;
            case '=': signIndex = 4; break;
            case '.': signIndex = 5; break;
            default:
                ClientLog.CL.Print("无此符号");
                signIndex = 0; break;
        }

        if (!IsSelect) Renderer.material = NumberSignMaterial[signIndex];
        else Renderer.material = NumberSignMaterial_Select[signIndex];
    }

    public void SetNumberColor(Color color)
    {
        MaterialPropertyBlock mpb=new MaterialPropertyBlock();
        Renderer.GetPropertyBlock(mpb);
        mpb.SetColor("_MainTex",color);
        mpb.SetColor("_EmissionColor", color);
        Renderer.SetPropertyBlock(mpb);
    }
}

public enum NumberSize
{
    Small = 0,
    Medium = 1,
    Big = 2
}

