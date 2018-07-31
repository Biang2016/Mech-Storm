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

    public Renderer Renderer;

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

    public Material[] NumberMaterial;

    public Material[] NumberSignMaterial;

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
                Renderer.material = NumberMaterial[value];
            }
        }
    }

    [SerializeField]
    private NumberSize MyNumberSize;

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

        Renderer.material = NumberSignMaterial[signIndex];
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

