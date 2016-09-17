using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace BabilinApps.VRInput
{
    public class VREventSystem : VRPointerEvents {

        protected VRInteractable currentIntractable;
        private VRInteractable currentSelectedInteractable;

        [Tooltip("Use this to see exactly what is happining inside of the VR Input System")]
        public bool isVerbose = false;
        [Tooltip ("Normalizes the fill value")]
        [SerializeField]
        bool isNormalizedFillValue = true;

        [SerializeField]
      protected  bool UseOnlyColliderRaycast = true;

        private IEnumerator selectCoroutine;
        private IEnumerator AutoClickCoroutine;

        protected float autoClickTimeDelta = 0;
        public float GetAutoClickTimeDelta {
            get
            {
                return autoClickTimeDelta;

            }
        }
        // Use this for initialization
        void Awake() {

            SetPointerEvents();
            pointer = new PointerEventData(EventSystem.current);
        }

        private static PointerEventData pointer = new PointerEventData(EventSystem.current);
        private static List<VRInteractable> resultList = new List<VRInteractable> ();
        private static List<VRInteractable> newResultList = new List<VRInteractable>();
        private static GameObject LastSelected;
        private static Vector2  lastLocation;
        public static bool RaycastMouse(Vector2 Location, out GameObject hit) {
            if (Input.GetButton("Cancel")) {
                hit = null;
                return false;
            }

            Cursor.lockState = CursorLockMode.Locked;
            List<RaycastResult> raycastResultCache = new List<RaycastResult>();
           
         
          

            pointer.position = Location;
          
            EventSystem.current.RaycastAll(pointer, raycastResultCache);

            foreach (RaycastResult result in raycastResultCache) {
                VRInteractable item = result.gameObject.GetComponent<VRInteractable>();
                if (item !=null)
                    newResultList.Add(item);
            }
            if (newResultList.Count <= 0) {
             
                LastSelected = null;
                hit = LastSelected;
                return false;
            }
              

            newResultList.Sort(delegate (VRInteractable c1, VRInteractable c2) {
                Vector2 offset = pointer.position - new Vector2(c1.transform.position.x, c1.transform.position.y);
                Vector2 offset2 = pointer.position - new Vector2(c2.transform.position.x, c2.transform.position.y);
                return offset.sqrMagnitude.CompareTo(offset2.sqrMagnitude);
            }
            );


            if (resultList.Count > 0) {
                if (resultList[0].transform != newResultList[0].transform)
                    resultList = newResultList;
                LastSelected = resultList[0].gameObject;
                hit = LastSelected;
                newResultList.Clear();
                return true;
            }
            else if (newResultList.Count > 0) {
                resultList = newResultList;
                LastSelected = resultList[0].gameObject;
                hit = LastSelected;
                return true;
            }
            else {
                hit = null;
                return false;
            }
        }

        //2 lists with foreach

        public void Select(VRInteractable interactiveObject)
        {
    
          
            currentSelectedInteractable = interactiveObject;
            if (currentSelectedInteractable == null) 
                return;
            
          if (currentIntractable != null) {
                if (currentSelectedInteractable == currentIntractable || currentSelectedInteractable.gameObject== currentIntractable.gameObject)
                    return;
            
               
            }
       



                if (selectCoroutine == null)
            {
                if (isVerbose)
                    Debug.Log("start selection...");
                Debug.Log(( selectCoroutine == null ));
                selectCoroutine = StartSelect(interactiveObject);
                StartCoroutine(selectCoroutine);
                return;

            }
            else
            {
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

        public void Deselect()
        {
            //override input to VR Gaze and try again
            if (EventSystem.current.firstSelectedGameObject) {
                VRInteractable interactable = EventSystem.current.firstSelectedGameObject.GetComponent<VRInteractable>();
                if (interactable != null)
                    Select(interactable);
                else
                    Deselect();
            }

            if (currentIntractable == null)
                return;

            if (isVerbose)
                Debug.Log("Deselect started");
        
            StartCoroutine(StartDeselect());
        }

        public void AutoClick(float time = 0)
        {
            if (isVerbose)
                Debug.Log("start auto click...");

            if (AutoClickCoroutine != null)
            {
                if (isVerbose)
                    Debug.Log("stopping last auto click.");

                StopCoroutine(AutoClickCoroutine);
            }
            autoClickTimeDelta = 0;
            StartCoroutine(SetAutoClickTime(time));
            AutoClickCoroutine = AutoClickAction(time);
            StartCoroutine(AutoClickCoroutine);
            if (isVerbose)
                Debug.Log("starting new auto click.");

        }

        /// <summary>
        /// action for clicking than deselecting the current object;
        /// </summary>
        public void ExecuteClick()
        {
            if (currentIntractable == null || currentIntractable.gameObject == null)
                return;

            if (isVerbose)
                Debug.Log("execute basic click");

            UnityPointerClick(currentIntractable.gameObject);
            currentIntractable.PointerClick();
            UnityPointerDeselect(currentIntractable.gameObject);
        }

        private IEnumerator SetAutoClickTime(float time)
        {
       
            while (GetAutoClickTimeDelta < time)
            {
                autoClickTimeDelta = autoClickTimeDelta + Time.deltaTime;
                //public version of 'autoClickTimeDelta' is 'GetAutoClickTimeDelta'

                if (currentIntractable == null)
                    yield break;
                if(isNormalizedFillValue)
                currentIntractable.AutoClickProgress(autoClickTimeDelta/time);
                else
                currentIntractable.AutoClickProgress(autoClickTimeDelta);

                yield return 0;
            }

        }

        public virtual IEnumerator AutoClickAction(float time = 0)
        {
            if (isVerbose)
                Debug.Log(string.Format("waiting [0] seconds", time));

            yield return new WaitUntil(() => GetAutoClickTimeDelta >= time);


            if (isVerbose)
                Debug.Log("autoclick Current time:" + autoClickTimeDelta);

            autoClickTimeDelta = autoClickTimeDelta + Time.deltaTime;
            Debug.Log(autoClickTimeDelta);
            yield return new WaitForSeconds(time);
            if (isVerbose)
                Debug.Log("Auto Click flished");
            autoClickTimeDelta = 0;
            yield return 0;
        }

        public virtual IEnumerator StartSelect(VRInteractable interactiveObject)
        {


            if (isVerbose)
                Debug.Log("pointer Selected and entered object. \n current intractable set.");

            
           
            UnityPointerSelect(currentIntractable.gameObject);
            UnityPointerEnter(currentIntractable.gameObject);
         
            interactiveObject.PointerEnter();
            currentIntractable = interactiveObject;

            yield return 0;
        }

        public virtual IEnumerator StartDeselect()
        {


            UnityPointerDeselect(currentIntractable.gameObject);
            UnityPointerExit(currentIntractable.gameObject);
            currentIntractable.PointerExit();
            currentIntractable = null;

            selectCoroutine = null;
            AutoClickCoroutine = null;
            if (isVerbose)
                Debug.Log("Deselect Finished");



            yield return 0;
        }



    }
}





