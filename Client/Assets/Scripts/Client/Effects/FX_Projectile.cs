using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class FX_Projectile : FX_Sprite
{
    Quaternion defaultRotation;

    void Awake()
    {
        defaultRotation = transform.localRotation;
    }

    public void Play(Vector3 from, Vector3 to, UnityAction hitAction, Color color, float duration, float scale = 1)
    {
        transform.localRotation = defaultRotation;

        Vector3 diff = to - from;
        diff = new Vector3(diff.x, 0, diff.z);
        Vector3 rotateAxis = Vector3.Cross(Vector3.right, diff);
        float rotateAngle = Vector3.Angle(Vector3.right, diff);
        transform.Rotate(rotateAxis, rotateAngle - 90);

        transform.position = from;
        transform.DOMove(to, duration).SetEase(Ease.Linear).OnComplete(delegate { hitAction(); });
        Play(color, duration, scale);
    }
}