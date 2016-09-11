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
        bool UseOnlyColliderRaycast = true;
        [SerializeField]
        VRPointer pointer;

        private Vector2 pointerScreenPosition { get
            {
                Vector2 pos = Camera.main.WorldToScreenPoint(pointer.transform.position);
                return new Vector2(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y));

            }

        }
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

        void CameraMovement() {
            if (Input.GetKeyDown(KeyCode.UpArrow))
                Camera.main.transform.Translate(transform.up / 4);

            if (Input.GetKeyDown(KeyCode.DownArrow))
                Camera.main.transform.Translate(-transform.up / 4);

            if (Input.GetKeyDown(KeyCode.RightArrow))
                Camera.main.transform.Translate(transform.right / 4);

            if (Input.GetKeyDown(KeyCode.LeftArrow))
                Camera.main.transform.Translate(-transform.right / 4);
        }


        // Update is called once per frame
        void LateUpdate()
        {
            if (Input.GetButton("Cancel") && !UseOnlyColliderRaycast) {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                return;
            }
            else if(!UseOnlyColliderRaycast)
                Cursor.lockState = CursorLockMode.Locked;

            CameraMovement();
            Input.mousePosition.Set(pointerScreenPosition.x-50, pointerScreenPosition.y - 80, 0);
            gazeRay = new Ray(transform.position, transform.forward);
            if (!Input.GetButton("Cancel") && Physics.Raycast(gazeRay, out objectGazeHit, maxDistance)) {
            
                
                ObjectRaycast();
            }
            else if (!UseOnlyColliderRaycast && !Input.GetButton("Cancel") && RaycastMouse(Input.mousePosition, out mouseGazeHit)) {

                Cursor.lockState = CursorLockMode.Locked;
                MouseRaycast();
            }
            else {
                if (EventSystem.current.firstSelectedGameObject) {
                    VRInteractable interactable = EventSystem.current.firstSelectedGameObject.GetComponent<VRInteractable>();
                    if (interactable != null)
                        Select(interactable);
                    else
                        Deselect();
                }
                else 
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
