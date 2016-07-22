using UnityEngine;
using System.Collections;

public class Gaze : VREventSystem {
    [SerializeField]
    bool isClick, isAutoClick;
    [SerializeField]
    VRPointer pointer;
    [SerializeField]
    float AutoClickTime = 2;

    public override IEnumerator StartSelect(VRInteractable interactiveObject)
    {

        pointer.SetState(VRPointer.State.SELECT);
        currentInteractable = interactiveObject;
        UnityPointerSelect(currentInteractable.gameObject);
        UnityPointerEnter(currentInteractable.gameObject);
        currentInteractable.AutoClickProgress(GetAutoClickTimeDelta);
        currentInteractable.PointerEnter();
        if (!isAutoClick)
       yield return ClickInput();
        else
            AutoClick(AutoClickTime);
        


    }

    IEnumerator ClickInput() {
        yield return new WaitUntil(() => isClick == true);

        pointer.SetState(VRPointer.State.CLICK);

        UnityPointerClick(currentInteractable.gameObject);
        currentInteractable.PointerClick();
        UnityPointerDeselect(currentInteractable.gameObject);
        isClick = false;
        Invoke("Deselect", 0.5f);
        yield return 0;
    }


    public override IEnumerator AutoClickAction(float time = 0)
    {
        autoClickTimeDelta = 0;
        yield return new WaitForEndOfFrame();

        autoClickTimeDelta = autoClickTimeDelta + Time.deltaTime;

        currentInteractable.AutoClickProgress(autoClickTimeDelta);
        yield return new WaitForSeconds(time);

        pointer.SetState(VRPointer.State.CLICK);

        UnityPointerClick(currentInteractable.gameObject);
        currentInteractable.PointerClick();
        UnityPointerDeselect(currentInteractable.gameObject);
        isClick = false;
        Invoke("Deselect", 0.5f);
        yield return 0;
    }

    public override IEnumerator StartDeselect()
    {
        pointer.SetState(VRPointer.State.IDLE);
        return base.StartDeselect();

    }

	
	// Update is called once per frame
	void Update () {
        
        GazeUpdate();

        if (pointer == null)
            return;
        if (hit.transform !=null)
            pointer.SetPointerPosition(ray.GetPoint(hit.distance - (hit.distance/100)));
        else
            pointer.SetPointerPosition(ray.GetPoint(.5f));

	}
}
