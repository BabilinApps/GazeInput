using UnityEngine;
using System.Collections;
namespace BabilinApps.VRInput
{
    public class VREventSystem : VRPointerEvents
    {

        protected VRInteractable currentIntractable;
        private VRInteractable currentSelectedInteractable;

        [Tooltip("Use this to see exactly what is happining inside of the VR Input System")]
        public bool isVerbose;

        protected Ray ray;
        protected RaycastHit hit = new RaycastHit();
        [SerializeField]
        private float maxDistance = 100;

        private IEnumerator selectCoroutine;
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
        void Awake()
        {

            SetPointerEvents();
        }

        // Update is called once per frame
        public void GazeUpdate()
        {
            ray = new Ray(transform.position, Vector3.forward);

            if (Physics.Raycast(ray, out hit, maxDistance))
            {
                VRInteractable interactable = hit.transform.GetComponent<VRInteractable>();

                if (interactable != null)
                    Select(interactable);

            }
            else
            {
                Deselect();
            }

        }

        public void Select(VRInteractable interactiveObject)
        {


            currentSelectedInteractable = interactiveObject;

            if (currentSelectedInteractable == currentIntractable)
            {

                return;
            }



            if (selectCoroutine == null)
            {
                if (isVerbose)
                    Debug.Log("start selection...");
                selectCoroutine = StartSelect(interactiveObject);
                StartCoroutine(selectCoroutine);

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
            currentIntractable = interactiveObject;
            interactiveObject.PointerEnter();


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

        void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            if (hit.point != Vector3.zero)
                Gizmos.DrawLine(transform.position, hit.point);
        }


    }
}





