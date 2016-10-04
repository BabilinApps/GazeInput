using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;
namespace BabilinApps.VRInput.Controller
{
    public class Gaze : VREventSystem
    {
        //Call this to activate a click without a timer;
        public bool isClick { get; set; }
        [Tooltip("Set true to have the pointer click the object after a given amount of time")]
        [SerializeField]
        bool isAutoClick = false;
        [Tooltip("Set true to allow auto click to repeat clicks on the same object even after it has been pressed")]
        [SerializeField]
        bool isRepeatable = false;
        [SerializeField]
        protected bool UseOnlyColliderRaycast = true;
        // pointer to use as the cross hair for gaze
        [SerializeField]
        VRPointer pointer;
       // Is the gaze pointer selecting something
        public bool isSelecting { get; private set; }
        //the pointers pixel position on the screen 
        private Vector2 pointerScreenPosition {
            get{
                Vector2 pos = Camera.main.WorldToScreenPoint(pointer.transform.position);
                return new Vector2(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y));
            }
        }
        [Tooltip("The amount of time required for the auto click to activate a press")]
        [SerializeField]
        float autoClickWaitTime = 2;
      
       
        //Ray that is used and set by the interactions 
        private Ray gazeRay;
        //The object that is stored during a selection that does not use colliders
        private GameObject mouseGazeHit;
        // the raycast hit that is stored during a selection using physics
        private RaycastHit objectGazeHit = new RaycastHit();
        [Tooltip("The distance at which the gaze pointer can select")]
        [SerializeField]
        private float maxDistance = 100;

        void Start()
        {
            if (pointer == null) {
                if (isVerbose)
                    Debug.Log("Could not finder pointer...");

                pointer = FindObjectOfType<VRPointer>();

                if (isVerbose && pointer != null)
                    Debug.Log("Found pointer automatically.");
                else if (isVerbose)
                    Debug.Log("Could not find pointer.");

            }
            else {
                pointer.transform.SetParent(Camera.main.transform);
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

        /// <summary>
        /// Used to press the button when 'isClicked' is set to true
        /// </summary>
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
                Debug.Log("Waiting 'waitForDeselect' value [" + deselectWaitTime + "] to deselect");

            if (!isRepeatable)
                Deselect();
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


            if (isRepeatable)
                Deselect();
            yield return 0;
        }

        public override IEnumerator StartDeselect()
        {
            if (pointer != null)
                pointer.SetState(VRPointer.State.IDLE);
            return base.StartDeselect();

        }


        /// <summary>
        /// Users input such as the escape key and the button press that is used without auto click
        /// </summary>
        void UserInput() {
            if (Input.GetMouseButtonDown(0))
                isClick = true;

            
            if (Input.GetButton("Cancel") && !UseOnlyColliderRaycast) {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                return;
            }
            else if (!UseOnlyColliderRaycast) {
                Cursor.lockState = CursorLockMode.Locked;
                Input.mousePosition.Set(pointerScreenPosition.x - 50, pointerScreenPosition.y - 80, 0);
            }


            // debug with mouse
            if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)) {
                gazeRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            }
            else
                gazeRay = new Ray(transform.position, transform.forward);
        }


        // Update is called once per frame
        void Update()
        {
            UserInput();
            UpdatePointer();
            GazeRaycast();
        }

        /// <summary>
        /// The function that casts a physics and mouse ray to select an intractable object
        /// </summary>
        void GazeRaycast() {


            if (!Input.GetButton("Cancel") && Physics.Raycast(gazeRay, out objectGazeHit, maxDistance))
                ObjectHit();

            else if (!UseOnlyColliderRaycast && !Input.GetButton("Cancel") && VRInteractions.RaycastMouse(Input.mousePosition, out mouseGazeHit))
                MouseHit();

            else
                Deselect();
        }

        /// <summary>
        /// called when the mouse ray hits an object
        /// </summary>
        void MouseHit() {
                VRInteractable interactable = mouseGazeHit.GetComponent<VRInteractable>();
                if (interactable != null)
                    Select(interactable);
    }

        /// <summary>
        /// Sets the pointer position and rotation
        /// </summary>
        void UpdatePointer() {

            if (pointer == null)
                return;
            Transform cameraTransform = Camera.main.transform;
            pointer.transform.LookAt(cameraTransform);
            if (objectGazeHit.transform != null) {
                pointer.SetPointerPosition(gazeRay.GetPoint(objectGazeHit.distance) *.95f);
                isSelecting = true;
            }
            else if (isSelecting) {
                pointer.SetPointerPosition(gazeRay.GetPoint(3));
                isSelecting = false;
            }
        }

        /// <summary>
        /// Called when the physics ray hits an object
        /// </summary>
        void ObjectHit() {
            VRInteractable interactable = objectGazeHit.transform.GetComponent<VRInteractable>();
                if (interactable != null)
                    Select(interactable);
        }


        /// <summary>
        /// draws a ray to the pointer from the gaze object
        /// </summary>
        void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            if (objectGazeHit.point != Vector3.zero)
                Gizmos.DrawLine(transform.position, pointer.transform.position);
        }
    }


}
