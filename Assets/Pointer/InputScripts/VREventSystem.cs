using UnityEngine;
using System.Collections;

public class VREventSystem : VRPointerEvents {

    protected VRInteractable currentIntractable;
    private VRInteractable currentSelectedInteractable;

    
    public bool isVerbose;

    protected Ray ray;
    protected RaycastHit hit = new RaycastHit();
    [SerializeField]
    private float maxDistance = 100;

    private IEnumerator selectCoroutine ;
    private IEnumerator AutoClickCoroutine;

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

            if (interactable != null)
                Select(interactable);
          

            

        }
        else {
            Deselect();
        }
	
	}

    

    public  void Select(VRInteractable interactiveObject)
    {
        

        currentSelectedInteractable = interactiveObject;

        if (currentSelectedInteractable == currentIntractable) {

            return;
        }

        

        if (selectCoroutine == null)
        {
            if (isVerbose)
                Debug.Log("start selection...");
            selectCoroutine = StartSelect(interactiveObject);
             StartCoroutine(selectCoroutine);
           
        }
        else {
            if (isVerbose)
                Debug.Log("new selection started before last finished. \n stopping selection.");
                StopCoroutine(selectCoroutine);

            if (isVerbose)
                Debug.Log("new selection started before last finished. \n deselecting object.");
            Deselect();
            if (isVerbose)
                Debug.Log("starting new selection.");
            currentIntractable = interactiveObject;
            selectCoroutine = StartSelect(interactiveObject);
          StartCoroutine(selectCoroutine);

        }
        
    }

    public virtual IEnumerator StartSelect(VRInteractable interactiveObject) {


        if (isVerbose)
            Debug.Log("pointer Selected and entered object. \n current intractable set.");
        UnityPointerSelect(currentIntractable.gameObject);
        UnityPointerEnter(currentIntractable.gameObject);
        currentIntractable = interactiveObject;
        interactiveObject.PointerEnter();


        yield return 0;
    }

    public virtual IEnumerator StartDeselect()
    {

        if (currentIntractable)
        {
            UnityPointerDeselect(currentIntractable.gameObject);
            UnityPointerExit(currentIntractable.gameObject);
            currentIntractable.PointerExit();
            currentIntractable = null;

            selectCoroutine = null;
            AutoClickCoroutine = null;
            if (isVerbose)
                Debug.Log("pointer Deselected and exited.  \n current intractable set to null.");
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
            if (isVerbose)
                Debug.Log("new auto click started before last finished \n stopping auto click.");

            StopCoroutine(AutoClickCoroutine);
        }

        AutoClickCoroutine =AutoClickAction(time);
       StartCoroutine(AutoClickCoroutine);
        if (isVerbose)
            Debug.Log("starting new auto click.");

    }


   
    public virtual IEnumerator AutoClickAction(float time = 0)
    {
        autoClickTimeDelta = 0;
        
        yield return new WaitForEndOfFrame();
        if (isVerbose)
            Debug.Log("autoclick Current time:" + autoClickTimeDelta);

        autoClickTimeDelta = autoClickTimeDelta + Time.deltaTime;
        Debug.Log(autoClickTimeDelta);
        yield return new WaitForSeconds(time);
        if (isVerbose)
            Debug.Log("Auto Clicked!");
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
