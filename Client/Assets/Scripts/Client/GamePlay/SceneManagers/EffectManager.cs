using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EffectManager : MonoSingleton<EffectManager>
{
    [SerializeField] private Transform EffectRoot;

    void Awake()
    {
    }

    void Start()
    {
        GenerateLensFlareIdle(0);
        GenerateLensFlareIdle(90);
    }

    private List<LensFlareIdle> LensFlareIdles = new List<LensFlareIdle>();

    public void GenerateLensFlareIdle(float rotate)
    {
        LensFlareIdle lfi = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.LensFlareIdle].AllocateGameObject<LensFlareIdle>(EffectRoot);
        LensFlareIdles.Add(lfi);
        lfi.transform.Rotate(Vector3.up, rotate, Space.World);
    }

    public void OnCardEditorButtonClick()
    {
        SceneManager.LoadScene("CardEditorScene");
    }
}