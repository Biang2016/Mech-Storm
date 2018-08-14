using System.Collections.Generic;
using UnityEngine;

public class CardDeckCard : MonoBehaviour, IGameObjectPool
{
    GameObjectPool gameObjectPool;

    public virtual void PoolRecycle()
    {
        gameObjectPool.RecycleGameObject(gameObject);
        transform.localScale = Vector3.one * 1.4f;
        transform.rotation = Quaternion.Euler(0, 180, 0);
    }


    private Renderer m_Renderer;

    protected virtual void Awake()
    {
        m_Renderer = GetComponent<Renderer>();
    }

    void Start()
    {
    }

    void Update()
    {
    }

    [SerializeField] private Renderer MainBoardRenderer;
    [SerializeField] private GameObject CardBloom;

    public void ChangeColor(Color newColor)
    {
        if (MainBoardRenderer)
        {
            MaterialPropertyBlock mpb = new MaterialPropertyBlock();
            MainBoardRenderer.GetPropertyBlock(mpb);
            mpb.SetColor("_Color", newColor);
            mpb.SetColor("_EmissionColor", newColor);
            MainBoardRenderer.SetPropertyBlock(mpb);
        }
    }

    public void ChangeCardBloomColor(Color color)
    {
        if (CardBloom)
        {
            Renderer rd = CardBloom.GetComponent<Renderer>();
            MaterialPropertyBlock mpb = new MaterialPropertyBlock();
            rd.GetPropertyBlock(mpb);
            mpb.SetColor("_Color", color);
            mpb.SetColor("_EmissionColor", color);
            rd.SetPropertyBlock(mpb);
        }
    }
}