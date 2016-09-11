using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

namespace BabilinApps.VRInput
{

    /// <summary>
    /// links VRInput events to Unity events
    /// </summary>
    public class VRPointerEvents : MonoBehaviour
    {
        //Unity UI pointer data 
        protected PointerEventData currentEventData;



        // Use this for initialization
        protected void SetPointerEvents()
        {
            currentEventData = new PointerEventData(EventSystem.current);
            
            if (currentEventData == null)
                Debug.LogWarning("There is no  Unity UI Event System in the scene");

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
            {
                ExecuteEvents.Execute(activeItem, currentEventData, ExecuteEvents.pointerUpHandler);
                ExecuteEvents.Execute(activeItem, currentEventData, ExecuteEvents.pointerExitHandler);
            }
        }

        protected void UnityPointerClick(GameObject activeItem)
        {
            if (currentEventData != null)
            {
                ExecuteEvents.Execute(activeItem, currentEventData, ExecuteEvents.pointerDownHandler);
                ExecuteEvents.Execute(activeItem, currentEventData, ExecuteEvents.pointerClickHandler);
            }
        }
    }
}
