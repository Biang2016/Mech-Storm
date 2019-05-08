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
                Ray ray = GameManager.Instance.BattleGroundCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit[] raycasts = Physics.RaycastAll(ray, 10f, UXLayer);

                PreviewPanelBG positivePreviewPanelBG = null;
                BoxCollider positiveCutOut = null;

                foreach (RaycastHit raycastHit in raycasts)
                {
                    if (positivePreviewPanelBG == null)
                    {
                        positivePreviewPanelBG = raycastHit.collider.GetComponent<PreviewPanelBG>();
                        if (positivePreviewPanelBG) continue;
                    }

                    if (positiveCutOut == null) positiveCutOut = raycastHit.collider.GetComponent<BoxCollider>();
                }

                if (positivePreviewPanelBG == this)
                {
                    if (positiveCutOut != null)
                    {
                    }
                    else
                    {
                        UIManager.Instance.CloseUIForm<CardPreviewPanel>();
                    }
                }
            }

            if (Input.GetMouseButton(1))
            {
                UIManager.Instance.CloseUIForm<CardPreviewPanel>();
            }
        }
    }
}