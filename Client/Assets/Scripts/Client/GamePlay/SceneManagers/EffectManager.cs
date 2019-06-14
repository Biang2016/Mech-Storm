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
    }

    public void OnCardEditorButtonClick()
    {
        SceneManager.LoadScene("CardEditorScene");
    }
}