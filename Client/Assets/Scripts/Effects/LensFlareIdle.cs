using System.Collections;
using UnityEngine;

public class LensFlareIdle : MonoBehaviour
{
    [SerializeField] private LensFlare LensFlare;
    [SerializeField] private Transform[] paths;

    void Start()
    {
        Flicker();
        Hashtable args = new Hashtable();
        args.Add("path", paths);
        args.Add("speed", 0.5f);
        args.Add("movetopath", true);
        args.Add("looptype", "loop");
        iTween.MoveTo(gameObject, args);
    }

    //void OnDrawGizmos()
    //{
    //    //在scene视图中绘制出路径与线
    //    iTween.DrawLine(paths, Color.yellow);

    //    iTween.DrawPath(paths, Color.red);
    //}

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