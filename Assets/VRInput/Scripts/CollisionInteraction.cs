using UnityEngine;
using System.Collections;
namespace BabilinApps.VRInput.Controller {
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    public class CollisionInteraction : VREventSystem {

        [SerializeField]
        float sphereRadius = .1f;
     
        Transform hitTransform;
        Transform selectedTransform;

        // Use this for initialization
        void Start() {
            var r = GetComponent<Rigidbody>();
            r.constraints = RigidbodyConstraints.FreezeAll;
        }

        // Update is called once per frame
        void FixedUpdate() {
            Transform toSelect;
            if (hitTransform == null && VRInteractions.SphereCast(transform.position, sphereRadius, out toSelect)) {
                selectObject(toSelect);
            }
            else if (hitTransform == null && !VRInteractions.SphereCast(transform.position, sphereRadius, out toSelect))
                Deselect();
        }

        void selectObject(Transform toTransform) {
            if (selectedTransform != null && toTransform != selectedTransform) {
                Deselect();
                selectedTransform = toTransform;
            }
            else if (selectedTransform == null)
                selectedTransform = toTransform;

            ObjectSeleted();
            

        }

        void OnTriggerEnter(Collider other) {
            if (hitTransform != null && hitTransform != other.transform) {
                Deselect();
                hitTransform = null;
            }
            else if (hitTransform == null) {
                hitTransform = other.transform;
                ObjectHit();
            }
        }
        void OnTriggerExit(Collider other) {
            if (hitTransform == other.transform) {
                Deselect();
                hitTransform = null;
            }
           
        }
        void OnTriggerStay(Collider other) {
            Debug.Log("Dwelling");
        }

        void OnCollisionEnter(Collision other) {
            if (hitTransform != null && hitTransform != other.transform) {
                Deselect();
                hitTransform = null;
            }
            else if (hitTransform == null) {
                hitTransform = other.transform;
                ObjectHit();
            }
           
        }

        void OnCollisionExit(Collision other) {
            if (hitTransform == other.transform) {
                Deselect();
                hitTransform = null;
            }
        }
        void OnCollisionStay(Collision other) {
           // Debug.Log("Dwelling");
        }

        void ObjectSeleted() {
            VRInteractable interactable = selectedTransform.GetComponent<VRInteractable>();
            if (interactable != null)
                Select(interactable);
        }

        void ObjectHit() {
            VRInteractable interactable = hitTransform.GetComponent<VRInteractable>();
            if (interactable != null)
                ExecuteClick();
        }

        void OnDrawGizmos() {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, sphereRadius);
        }
    }
}
