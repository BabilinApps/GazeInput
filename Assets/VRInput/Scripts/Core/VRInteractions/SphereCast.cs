using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace BabilinApps.VRInput {
    public partial class VRInteractions : MonoBehaviour {
        
        /// <summary>
        /// Allows user to cast a sphere and get the closest item to the location given.
        /// </summary>
        /// <param name="location"> Center of the sphere cast</param>
        /// <param name="radius"> radius of the sphere cast</param>
        /// <param name="hit"> the closest item returned</param>
        public static bool SphereCast(Vector3 location, float radius, out VRInteractions hit) {
            hit = null;
            var castObjects = Physics.OverlapSphere(location, radius);
            List<VRInteractions> hitItems = new List<VRInteractions>();
            foreach (var item in castObjects) {
                var interactionObject = item.GetComponent<VRInteractions>();
                if (interactionObject != null)
                    hitItems.Add(interactionObject);
            }
            hitItems.Sort((a, b) => Vector3.Distance(a.transform.position, b.transform.position).CompareTo(Vector3.Distance(a.transform.position, b.transform.position)));

            if (hitItems.Count > 0) {
                hit = hitItems[0];
                return true;
            }
            else {
                return false;
            }

        }

        /// <summary>
        /// Allows user to cast a sphere and get the closest item to the location given.
        /// </summary>
        /// <param name="location"> Center of the sphere cast</param>
        /// <param name="radius"> radius of the sphere cast</param>
        /// <param name="hit"> the closest item returned</param>
        public static bool SphereCast(Vector3 location, float radius, out Transform hit) {
            hit = null;
            var castObjects = Physics.OverlapSphere(location, radius);
            List<VRInteractable> hitItems = new List<VRInteractable>();
            foreach (var item in castObjects) {
                var interactionObject = item.GetComponent<VRInteractable>();
                if (interactionObject != null)
                    hitItems.Add(interactionObject);
            }
            hitItems.Sort((a, b) => Vector3.Distance(a.transform.position, b.transform.position).CompareTo(Vector3.Distance(a.transform.position, b.transform.position)));

            if (hitItems.Count > 0) {
                hit = hitItems[0].transform;
                return true;
            }
            else {
                return false;
            }

        }
    }
}