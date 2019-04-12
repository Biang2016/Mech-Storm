using System.Collections;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    public Camera BattleGroundCamera;

    internal Vector3 UseCardShowPosition = new Vector3(10, 3, 0);
    internal Vector3 UseCardShowPosition_Overlay = new Vector3(10, 3, 0.2f);

    public bool ShowBEMMessages = false;

    public float HandCardSize = 1.5f;
    public float HandCardInterval = 1.0f;
    public float HandCardRotate = 1.0f;
    public float HandCardOffset = 0.4f;

    public float PullOutCardSize = 3.0f;
    public float PullOutCardDistanceThreshold = 0f;

    public float DetailSingleCardSize = 3.0f;
    public float DetailEquipmentCardSize = 2.5f;
    public float DetailRetinueCardSize = 4.0f;

    public float RetinueDefaultSize = 1.75f;
    public float RetinueInterval = 3.5f;
    public float RetinueDetailPreviewDelaySeconds = 0.7f;

    public float CardShowScale = 3f;

    public float ShowCardDuration = 1.2f;
    public float ShowCardFlyDuration = 0.4f;

    public int CardDeckCardNum = 10;

    public float CardDeckCardSize = 1.4f;
    public Vector3 Self_CardDeckCardInterval = new Vector3(0.05f, 0.01f, 0.1f);
    public Vector3 Enemy_CardDeckCardInterval = new Vector3(-0.05f, 0.01f, 0.1f);
}