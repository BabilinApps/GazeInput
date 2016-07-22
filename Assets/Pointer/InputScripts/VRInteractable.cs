using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class VRInteractable : MonoBehaviour {

    public UnityEvent OnPointerEnter;
    public UnityEvent OnPointerClick;
    public UnityEvent OnPointerExit;

    public void PointerEnter()
    {
        if (OnPointerEnter != null)
            OnPointerEnter.Invoke(); 
    }

    public void PointerClick()
    {
        PointerEnter();
        if (OnPointerClick != null)
            OnPointerClick.Invoke();
    }

    public void PointerExit()
    {
        if (OnPointerExit != null)
            OnPointerExit.Invoke();
    }

    public void AutoClickProgress(float progress)
    {

    }
}
