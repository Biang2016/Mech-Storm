using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class PreviewPanelBG : MonoBehaviour
{
    private int UXLayer;

    public BoxCollider CutOutPanel;

    void Awake()
    {
        UXLayer = 1 << LayerMask.NameToLayer("UX");
    }

    void Update()
    {
        if (gameObject.activeSelf)
        {
            if (Input.GetMouseButton(0))
            {
                Ray ray = GameManager.Instance.SelectCardWindowForeCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit[] raycasts = Physics.RaycastAll(ray, 10f, UXLayer);

                PreviewPanelBG positivePreviewPanelBG = null;
                BoxCollider positiveCutOut = null;

                foreach (RaycastHit raycastHit in raycasts)
                {
                    if (positivePreviewPanelBG == null) positivePreviewPanelBG = raycastHit.collider.GetComponent<PreviewPanelBG>();
                    if (positivePreviewPanelBG == null && positiveCutOut == null) positiveCutOut = raycastHit.collider.GetComponent<BoxCollider>();
                }

                if (positivePreviewPanelBG == this)
                {
                    if (positiveCutOut != null)
                    {

                    }
                    else
                    {
                        SelectBuildManager.Instance.HidePreviewCardPanel();
                    }
                }
            }

            if (Input.GetMouseButton(1))
            {
                SelectBuildManager.Instance.HidePreviewCardPanel();
            }
        }
    }
}