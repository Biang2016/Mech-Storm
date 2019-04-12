using UnityEngine;

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