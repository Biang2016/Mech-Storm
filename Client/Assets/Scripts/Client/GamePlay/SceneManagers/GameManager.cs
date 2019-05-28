using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoSingleton<GameManager>
{
    internal int Layer_Cards;
    internal int Layer_UI;
    internal int Layer_Modules;
    internal int Layer_Mechs;
    internal int Layer_Ships;
    internal int Layer_Slots;
    internal int Layer_BoardAreas;
    internal int Layer_UX;

    void Awake()
    {
        Layer_Cards = 1 << LayerMask.NameToLayer("Cards");
        Layer_UI = 1 << LayerMask.NameToLayer("UI");
        Layer_Modules = 1 << LayerMask.NameToLayer("Modules");
        Layer_Mechs = 1 << LayerMask.NameToLayer("Mechs");
        Layer_Ships = 1 << LayerMask.NameToLayer("Ships");
        Layer_Slots = 1 << LayerMask.NameToLayer("Slots");
        Layer_BoardAreas = 1 << LayerMask.NameToLayer("BoardAreas");
        Layer_UX = 1 << LayerMask.NameToLayer("UX");
    }

    void Start()
    {
       
    }

    public void OnCardEditorButtonClick()
    {
        SceneManager.LoadScene("CardEditorScene");
    }
}