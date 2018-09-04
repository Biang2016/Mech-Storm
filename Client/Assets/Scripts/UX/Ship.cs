using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Ship : MonoBehaviour,IMouseHoverComponent
{
    internal ClientPlayer ClientPlayer;

    public MeshCollider Collider;
    public GameObject shield;

    public void MouseHoverComponent_OnMousePressEnterImmediately(Vector3 mousePosition)
    {
        shield.SetActive(true);
    }

    public void MouseHoverComponent_OnMouseEnterImmediately(Vector3 mousePosition)
    {
    }

    public void MouseHoverComponent_OnMouseEnter(Vector3 mousePosition)
    {
    }

    public void MouseHoverComponent_OnMouseOver()
    {
    }

    public void MouseHoverComponent_OnMouseLeave()
    {
    }

    public void MouseHoverComponent_OnMouseLeaveImmediately()
    {
    }

    public void MouseHoverComponent_OnMousePressLeaveImmediately()
    {
        shield.SetActive(false);
    }
}