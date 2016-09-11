using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;
namespace BabilinApps.VRInput.Controller
{
    public class Gaze : VREventSystem
    {
        
        public bool isClick, isAutoClick, isRepeatable = false;
        [SerializeField]
        VRPointer pointer;
        [SerializeField]
        float autoClickWaitTime = 2;
        [SerializeField]
        float waitForDeselect = .5f;

        private Ray gazeRay;
        private GameObject mouseGazeHit;
        private RaycastHit objectGazeHit = new RaycastHit();
        [SerializeField]
        private float maxDistance = 100;

        void Start()
        {
            if (pointer == null)
            {
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

            if (isVerbose)
                Debug.Log("pointer Selected and entered object. \n current intractable set.");
            if (pointer != null)
                pointer.SetState(VRPointer.State.SELECT);

            currentIntractable = interactiveObject;
            UnityPointerSelect(currentIntractable.gameObject);
            UnityPointerEnter(currentIntractable.gameObject);
            currentIntractable.PointerEnter();
            if (!isAutoClick)
                yield return ClickInput();
            else
                AutoClick(autoClickWaitTime);



        }

        IEnumerator ClickInput()
        {
            yield return new WaitUntil(() => isClick == true);
            if (isVerbose)
                Debug.Log("isClicked has been set to true");
            if (pointer != null)
                pointer.SetState(VRPointer.State.CLICK);

            ExecuteClick();

            isClick = false;
            if (isVerbose)
                Debug.Log("Waiting 'waitForDeselect' value [" + waitForDeselect + "] to deselect");

            if (!isRepeatable)
                Invoke("Deselect", 0.5f);
            yield return 0;
        }



        public override IEnumerator AutoClickAction(float time = 0)
        {
            if (isVerbose)
                Debug.Log(string.Format("waiting {0} seconds", time.ToString()));

            yield return new WaitUntil(() => GetAutoClickTimeDelta >= time);

            if (isVerbose)
                Debug.Log("auto clicked finished, Execute click...");

            if (pointer != null)
                pointer.SetState(VRPointer.State.CLICK);

            ExecuteClick();
            isClick = false;


            if(isRepeatable)
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
        void LateUpdate()
        {
            gazeRay = new Ray(transform.position, transform.forward);
            if (Physics.Raycast(gazeRay, out objectGazeHit, maxDistance)) {
                ObjectRaycast();
            }
            else if (RaycastMouse(Camera.main.WorldToScreenPoint(pointer.transform.position), out mouseGazeHit)) {
                MouseRaycast();
            }
            else {
                Deselect();
            }
            UpdatePointer();




        }

        void MouseRaycast() {
            

           
                VRInteractable interactable = mouseGazeHit.GetComponent<VRInteractable>();
                if (interactable != null)
                    Select(interactable);

            
        

       
        
    }

        void UpdatePointer() {

            if (pointer == null)
                return;

            if (objectGazeHit.transform != null)
                pointer.SetPointerPosition(gazeRay.GetPoint(objectGazeHit.distance - ( objectGazeHit.distance / 100 )));
            else
                pointer.SetPointerPosition(gazeRay.GetPoint(.5f));
        }

        void ObjectRaycast() {


            VRInteractable interactable = objectGazeHit.transform.GetComponent<VRInteractable>();

                if (interactable != null)
                    Select(interactable);

          

        }



        void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            if (objectGazeHit.point != Vector3.zero)
                Gizmos.DrawLine(transform.position, objectGazeHit.point);
        }
    }


}
