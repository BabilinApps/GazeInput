using UnityEngine;
using System.Collections;

public class Gaze : VREventSystem {
    [SerializeField]
    bool isClick, isAutoClick;
    [SerializeField]
    VRPointer pointer;
    [SerializeField]
    float AutoClickTime = 2;


    void Start() {
        if (pointer == null) {
            if (isVerbose)
                Debug.Log("Could not finder pointer...");

            pointer = FindObjectOfType<VRPointer>();

            if (isVerbose && pointer != null)
                Debug.Log("Found pointer automatically.");
            else if (isVerbose)
                Debug.Log("Could not find pointer.");

        }
    }

    public override IEnumerator StartSelect(VRInteractable interactiveObject)
    {


        if (pointer != null)
            pointer.SetState(VRPointer.State.SELECT);

        currentIntractable = interactiveObject;
        UnityPointerSelect(currentIntractable.gameObject);
        UnityPointerEnter(currentIntractable.gameObject);
        currentIntractable.AutoClickProgress(GetAutoClickTimeDelta);
        currentIntractable.PointerEnter();
        if (!isAutoClick)
       yield return ClickInput();
        else
            AutoClick(AutoClickTime);
        


    }

    IEnumerator ClickInput() {
        yield return new WaitUntil(() => isClick == true);
        if (pointer != null)
            pointer.SetState(VRPointer.State.CLICK);

        UnityPointerClick(currentIntractable.gameObject);
        currentIntractable.PointerClick();
        UnityPointerDeselect(currentIntractable.gameObject);
        isClick = false;
        Invoke("Deselect", 0.5f);
        yield return 0;
    }


    public override IEnumerator AutoClickAction(float time = 0)
    {
        autoClickTimeDelta = 0;
        yield return new WaitForEndOfFrame();

        autoClickTimeDelta = autoClickTimeDelta + Time.deltaTime;

        currentIntractable.AutoClickProgress(autoClickTimeDelta);
        yield return new WaitForSeconds(time);
        if (pointer != null)
            pointer.SetState(VRPointer.State.CLICK);

        UnityPointerClick(currentIntractable.gameObject);
        currentIntractable.PointerClick();
        UnityPointerDeselect(currentIntractable.gameObject);
        isClick = false;
        Invoke("Deselect", 0.5f);
        yield return 0;
    }

    public override IEnumerator StartDeselect()
    {
        if (pointer != null)
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
