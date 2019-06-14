using UnityEngine;
using UnityEngine.UI;

public class StarsGroup : MonoBehaviour
{
    [SerializeField] private Image[] Stars;
    [SerializeField] private Sprite StarSprite;
    [SerializeField] private Sprite StarEmptySprite;

    public void SetStarNumber(int number, int maxNumber)
    {
        if (maxNumber == 1)
        {
            foreach (Image star in Stars)
            {
                star?.gameObject.SetActive(false);
            }

            return;
        }

        for (int i = 0; i < Stars.Length; i++)
        {
            if (Stars[i] != null)
            {
                Stars[i].sprite = i < number ? StarSprite : StarEmptySprite;
                Stars[i].gameObject.SetActive(i < maxNumber);
            }
        }
    }
}