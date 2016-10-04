using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;
namespace BabilinApps.VRInput {
    public partial class VRInteractions : MonoBehaviour {

        public static PointerEventData pointer = new PointerEventData(EventSystem.current);
        private static List<VRInteractable> resultList = new List<VRInteractable>();
        private static List<VRInteractable> newResultList = new List<VRInteractable>();
        private static GameObject LastSelected;
        private static Vector2 lastLocation;

        /// <summary>
        /// selects a UI object from a given point.
        /// </summary>
        /// <param name="Location"> the screen point where to select the object</param>
        /// <param name="hit"> the object that gets selected</param>
        /// <returns></returns>
        public static bool RaycastMouse(Vector2 Location, out GameObject hit) {

            Cursor.lockState = CursorLockMode.Locked;
            List<RaycastResult> raycastResultCache = new List<RaycastResult>();




            pointer.position = Location;

            EventSystem.current.RaycastAll(pointer, raycastResultCache);

            foreach (RaycastResult result in raycastResultCache) {
                VRInteractable item = result.gameObject.GetComponent<VRInteractable>();
                if (item != null)
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
    }
}