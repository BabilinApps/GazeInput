using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace BabilinApps.VRInput
{
    public abstract class VREventSystem : VRPointerEvents {

        protected VRInteractable currentIntractable;
        private VRInteractable currentSelectedInteractable;

        [Tooltip("The amount of time waited before the transition from clicked to deselected")]
       public float deselectWaitTime = .5f;
        [Tooltip("Use this to see exactly what is happining inside of the VR Input System")]
        public bool isVerbose = false;
        [Tooltip ("Normalizes the fill value")]
        [SerializeField]
        bool isNormalizedFillValue = true;

      

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
            VRInteractions.pointer = new PointerEventData(EventSystem.current);
        }


        /// <summary>
        /// calls the event on a interactive object
        /// </summary>
        /// <param name="interactiveObject"> the object to select</param>
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

        /// <summary>
        /// deselects the current object
        /// </summary>
        public void Deselect()
        {
            if (EventSystem.current.firstSelectedGameObject) {
                VRInteractable interactable = EventSystem.current.firstSelectedGameObject.GetComponent<VRInteractable>();
                if (interactable != null) {
                    Select(interactable);
                    return;
                }
            }

            if (currentIntractable == null)
                return;

            if (isVerbose)
                Debug.Log("Deselect started");
        
            StartCoroutine(StartDeselect());
        }

        /// <summary>
        /// Calls for a click on the current interactive object after a given time
        /// </summary>
        /// <param name="time"> time to wait before click</param>
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

        /// <summary>
        /// sends the auto click progress to the current interactive object
        /// </summary>
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

        /// <summary>
        /// Started when AutoClick is called
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
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

        /// <summary>
        /// starts when an object is selected or enters the gaze ray
        /// </summary>
        public virtual IEnumerator StartSelect(VRInteractable interactiveObject)
        {


            if (isVerbose)
                Debug.Log("pointer Selected and entered object. \n current intractable set.");


            currentIntractable = interactiveObject;
            UnityPointerSelect(currentIntractable.gameObject);
            UnityPointerEnter(currentIntractable.gameObject);
         
            interactiveObject.PointerEnter();
            

            yield return 0;
        }

        /// <summary>
        /// Started when Deselect is called
        /// </summary>
        public virtual IEnumerator StartDeselect()
        {
            var ci = currentIntractable;
            yield return new WaitForSeconds(deselectWaitTime);

            UnityPointerDeselect(ci.gameObject);
            UnityPointerExit(ci.gameObject);
            ci.PointerExit();
            currentIntractable = null;

            selectCoroutine = null;
            AutoClickCoroutine = null;
            if (isVerbose)
                Debug.Log("Deselect Finished");



            yield return 0;
        }



    }
}





