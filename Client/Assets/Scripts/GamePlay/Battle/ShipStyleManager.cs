using UnityEngine;

public class ShipStyleManager : MonoBehaviour
{
    [SerializeField] private Material[] ShipMaterials;
    [SerializeField] private MeshRenderer ShipShape;
    [SerializeField] private MeshRenderer ShipShapeHover;

    private ClientPlayer ClientPlayer;

    public void Initialize(ClientPlayer clientPlayer)
    {
        ClientPlayer = clientPlayer;
        if (ClientPlayer.WhichPlayer == Players.Self)
        {
            SetShipStyle(ShipStyles.BlueShip);
        }
        else if (ClientPlayer.WhichPlayer == Players.Enemy)
        {
            SetShipStyle(ShipStyles.RedShip);
        }

        ShowShipShapeHover(false);
    }

    private void SetShipStyle(ShipStyles shipStyle)
    {
        ShipShape.material = ShipMaterials[(int) shipStyle];
        ShipShapeHover.material = ShipMaterials[(int) shipStyle];
    }

    public void ShowShipShapeHover(bool isShow)
    {
        ShipShapeHover.gameObject.SetActive(isShow);
    }

    public void ResetAll()
    {
        ClientPlayer = null;
    }

    enum ShipStyles
    {
        BlueShip = 0,
        RedShip = 1,
    }
}