using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class ButtonEffects : MonoBehaviour
{
    private Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void OnMouseEnter()
    {
        anim.SetTrigger("OnMouseEnter");
    }   

    public void OnMouseExit()
    {
        anim.SetTrigger("OnMouseLeave");
    }
}