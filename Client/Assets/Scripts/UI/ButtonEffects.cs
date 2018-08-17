using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Animator))]
public class ButtonEffects : MonoBehaviour
{
    private Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void OnPointerEnter()
    {
        anim.SetTrigger("OnMouseEnter");
    }

    public void OnPointerExit()
    {
        anim.SetTrigger("OnMouseLeave");
    }
}