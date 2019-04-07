using UnityEngine;

internal class HitManager : MonoSingleton<HitManager>
{
    private HitManager()
    {
    }

    [SerializeField] private GameObjectPool[] HitFX_Pool;

    public void ShowHit(Transform transform, HitType hitType, string colorText, float duration, float scale = 1)
    {
        Color color = ClientUtils.HTMLColorToColor(colorText);
        Hit hit = HitFX_Pool[(int) hitType].AllocateGameObject<Hit>(this.transform);
        hit.transform.position = transform.position;
        hit.ShowHit(color, duration, scale);
    }

    public enum HitType
    {
        LineLeftTopToRightButtom = 0,
        LineRightTopToLeftButtom = 1,
        Blade = 2,
        EmitLight = 3,
    }
}