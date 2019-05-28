using System.Collections;
using DG.Tweening;
using UnityEngine;

public class LensFlareIdle : PoolObject
{
    public override void PoolRecycle()
    {
        base.PoolRecycle();
        LensFlare.transform.DOPause();
        StopAllCoroutines();
        transform.position = paths[0].position;
    }

    [SerializeField] private LensFlare LensFlare;
    [SerializeField] private Transform[] paths;

    void Start()
    {
        Flicker();

        Vector3[] pathVector3 = new Vector3[paths.Length];
        for (int i = 0; i < paths.Length; i++)
        {
            pathVector3[i] = paths[i].position;
        }

        LensFlare.transform.DOPath(pathVector3, 30f, PathType.CatmullRom, PathMode.Full3D, 10, Color.white).SetLoops(-1);
    }

    void Update()
    {
    }

    private void Flicker()
    {
        StartCoroutine(Co_Flicker());
    }

    IEnumerator Co_Flicker()
    {
        bool isBrighter = false;
        while (true)
        {
            if (isBrighter)
            {
                while (LensFlare.brightness < 0.5f)
                {
                    LensFlare.brightness += 0.01f;
                    yield return new WaitForSeconds(0.1f);
                }

                isBrighter = false;
            }
            else
            {
                while (LensFlare.brightness > 0.2f)
                {
                    LensFlare.brightness -= 0.01f;
                    yield return new WaitForSeconds(0.1f);
                }

                isBrighter = true;
            }
        }
    }
}