using TMPro;
using UnityEngine;

public class CardTypeComponent : MonoBehaviour
{
    [SerializeField] private TextMeshPro CardTypeText;

    public void SetText(string text)
    {
        CardTypeText.text = text;
    }
}