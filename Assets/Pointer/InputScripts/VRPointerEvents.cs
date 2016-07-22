using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class VRPointerEvents : MonoBehaviour {

    private PointerEventData currentEventData;

  

    // Use this for initialization
    protected void SetPointerEvents()
    {
        currentEventData = new PointerEventData(EventSystem.current);

    }


    protected void UnityPointerEnter(GameObject activeItem)
    {
        if (currentEventData != null)
             ExecuteEvents.Execute(activeItem, currentEventData, ExecuteEvents.pointerEnterHandler);
    }

    protected void UnityPointerSelect(GameObject activeItem)
    {
        if (currentEventData != null)
            ExecuteEvents.Execute(activeItem, currentEventData, ExecuteEvents.selectHandler);
    }

    protected void UnityPointerDeselect(GameObject activeItem)
    {
        if (currentEventData != null)
            ExecuteEvents.Execute(activeItem, currentEventData, ExecuteEvents.deselectHandler);
    }

    protected void UnityPointerExit(GameObject activeItem)
    {
        if (currentEventData != null)
            ExecuteEvents.Execute(activeItem, currentEventData, ExecuteEvents.pointerExitHandler);
    }

    protected void UnityPointerClick(GameObject activeItem)
    {
        if (currentEventData != null)
            ExecuteEvents.Execute(activeItem, currentEventData, ExecuteEvents.pointerClickHandler);
    }
}
