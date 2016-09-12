﻿using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.Events;
[RequireComponent(typeof(BoxCollider))]
public class VRInteractable : MonoBehaviour {

    public UnityEvent OnPointerEnter;
    public UnityEvent OnPointerClick;
    public UnityEvent OnPointerExit;
    [ExecuteInEditMode]
    void Start() {
        var c = GetComponent<BoxCollider>();
        var r =GetComponent<RectTransform>();
        if (r == null)
            return;

        c.size = new Vector3(r.sizeDelta.x, r.sizeDelta.y, 1);


    }
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
