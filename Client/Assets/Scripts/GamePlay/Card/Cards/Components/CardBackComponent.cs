using UnityEngine;

public class CardBackComponent : MonoBehaviour
{
    [SerializeField] private MeshRenderer CardBack;
    [SerializeField] private MeshRenderer CardBloom;


    public void SetCardBackColor(Color color, float intensity)
    {
        ClientUtils.ChangeColor(CardBack, color, intensity);
    }

    public void SetCardBloomColor(Color color, float intensity)
    {
        ClientUtils.ChangeColor(CardBloom, color, intensity);
    }

    public void SetBloomShow(bool isShow)
    {
        CardBloom.gameObject.SetActive(isShow);
    }
}