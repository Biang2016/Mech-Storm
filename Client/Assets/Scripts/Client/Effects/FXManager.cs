using System;
using UnityEngine;
using UnityEngine.Events;

internal class FXManager : MonoSingleton<FXManager>
{
    private FXManager()
    {
    }

    public void PlayFX(Transform parentTrans, FXType hitType, string colorText, float duration, float scale = 1)
    {
        PlayFX(parentTrans.position, hitType, colorText, duration, scale);
    }

    public void PlayFX(Vector3 position, FXType hitType, string colorText, float duration, float scale = 1)
    {
        GameObjectPoolManager.PrefabNames prefabName = (GameObjectPoolManager.PrefabNames) Enum.Parse(typeof(GameObjectPoolManager.PrefabNames), hitType.ToString());
        Color color = ClientUtils.HTMLColorToColor(colorText);
        FX_Base fx = GameObjectPoolManager.Instance.PoolDict[prefabName].AllocateGameObject<FX_Base>(transform);
        fx.transform.position = position;
        fx.Play(color, duration, scale);
    }

    public void PlayProjectile(FXType_Projectile fxType_Projectile, Vector3 from, Vector3 to, UnityAction hitAction, string colorText, float duration, float scale = 1)
    {
        GameObjectPoolManager.PrefabNames prefabName = (GameObjectPoolManager.PrefabNames) Enum.Parse(typeof(GameObjectPoolManager.PrefabNames), fxType_Projectile.ToString());
        Color color = ClientUtils.HTMLColorToColor(colorText);
        FX_Projectile fx = GameObjectPoolManager.Instance.PoolDict[prefabName].AllocateGameObject<FX_Projectile>(transform);
        fx.Play(from, to, hitAction, color, duration, scale);
    }

    public enum FXType
    {
        FX_Hit0,
        FX_Hit1,
        FX_Blade1,
        FX_Blade2,
        FX_WeaponEnergyUp,
        FX_ShipAddEnergy,
        FX_Particle,
        FX_GunBulletHit,
        FX_Shield,
        FX_MechAddLife,
    }

    public enum FXType_Projectile
    {
        FX_GunBullet = 0,
    }
}