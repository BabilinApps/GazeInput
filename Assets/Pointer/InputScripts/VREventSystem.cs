using UnityEngine;
using System.Collections;

public class VREventSystem : VRPointerEvents {

    protected VRInteractable currentInteractable;
    private VRInteractable currentSelectedInteractable;

    protected Ray ray;
    protected RaycastHit hit = new RaycastHit();
    private float maxDistance = 100;

    private Coroutine selectCoroutine ;
    private Coroutine AutoClickCoroutine;

   protected float autoClickTimeDelta = 0;
    public float GetAutoClickTimeDelta
    {
        get
        {
            return autoClickTimeDelta;

        }
    }
    // Use this for initialization
    void Awake () {

        SetPointerEvents();
    }

    // Update is called once per frame
    public void  GazeUpdate() {
        ray = new Ray(transform.position, Vector3.forward);

        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            VRInteractable interactable = hit.transform.GetComponent<VRInteractable>();

            if(interactable != null)
                Select(interactable);
            

        }
        else {
            Deselect();
        }
	
	}

    

    public  void Select(VRInteractable interactiveObject)
    {
        currentSelectedInteractable = interactiveObject;
        if (currentSelectedInteractable == currentInteractable)
            return;

        if (selectCoroutine == null)
        {
           
            selectCoroutine = StartCoroutine(StartSelect(interactiveObject));
        }
        else {
            StopCoroutine(selectCoroutine);
            Deselect();
            currentInteractable = interactiveObject;
            selectCoroutine = StartCoroutine(StartSelect(interactiveObject));

        }
        
    }

    public virtual IEnumerator StartSelect(VRInteractable interactiveObject) {
        UnityPointerSelect(currentInteractable.gameObject);
        UnityPointerEnter(currentInteractable.gameObject);
        currentInteractable = interactiveObject;
        interactiveObject.PointerEnter();
        yield return 0;
    }

    public virtual IEnumerator StartDeselect()
    {
        if (currentInteractable)
        {
            UnityPointerDeselect(currentInteractable.gameObject);
            UnityPointerExit(currentInteractable.gameObject);
            currentInteractable.PointerExit();
            currentInteractable = null;
        }

        yield return 0;
    }

    public void Deselect()
    {
        StartCoroutine(StartDeselect());
    }

    public void AutoClick(float time = 0)
    {
        if (AutoClickCoroutine != null)
        {
            StopCoroutine(AutoClickCoroutine);
        }
        AutoClickCoroutine = StartCoroutine(AutoClickAction(time));

    }


   
    public virtual IEnumerator AutoClickAction(float time = 0)
    {
        autoClickTimeDelta = 0;
         yield return new WaitForEndOfFrame();

        autoClickTimeDelta = autoClickTimeDelta + Time.deltaTime;
        Debug.Log(autoClickTimeDelta);
        yield return new WaitForSeconds(time);
        Debug.Log("Clicked");
        autoClickTimeDelta = 0;
        yield return 0;
    }


        


    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        if(hit.point != Vector3.zero)
        Gizmos.DrawLine(transform.position, hit.point);
    }








}
