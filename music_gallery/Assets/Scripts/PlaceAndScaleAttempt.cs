// taken from arfoundation-samples

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.Samples
{
    /// <summary>
    /// Listens for touch events and performs an AR raycast from the screen touch point.
    /// AR raycasts will only hit detected trackables like feature points and planes.
    ///
    /// If a raycast hits a trackable, the <see cref="placedPrefab"/> is instantiated
    /// and moved to the hit position.
    /// </summary>

    /// I am going to attept to add a scaling feature to this based on the scaling functionality found in
    /// https://docs.google.com/document/d/1z7xG20s7_jM7ftizfXexMd9b3ycYDfkBCPxUyRG3UNs/edit
    [RequireComponent(typeof(ARRaycastManager))]
    public class PlaceAndScaleAttempt : MonoBehaviour
    {
        // the prefab that will be the placeable object
        [SerializeField]
        [Tooltip("Instantiates this prefab on a plane at the touch location.")]
        GameObject m_PlacedPrefab;

        // ARRaycastManager stuff
        static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();
        ARRaycastManager m_RaycastManager;

        // Values to check to see if things can be done
        float initDist = 0;
        Vector3 initScale;
        bool canPlace, canScale, canMove;

        // The prefab to instantiate on touch.
        public GameObject placedPrefab
        {
            get { return m_PlacedPrefab; }
            set { m_PlacedPrefab = value; }
        }

        // The object instantiated as a result of a successful raycast intersection with a plane.
        public GameObject spawnedObject { get; private set; }

        void Awake()
        {
            m_RaycastManager = GetComponent<ARRaycastManager>();
        }

        // code from ARFoundation-samples
        //bool TryGetTouchPosition(out Vector2 touchPosition)
        //{
        //    if (Input.touchCount > 0)
        //    {
        //        touchPosition = Input.GetTouch(0).position;
        //        return true;
        //    }
        //    touchPosition = default;
        //    return false;
        //}

        // adapted from above method, added functionality to ensure placement is
        // being executed by user
        bool CanPlace(out Vector2 touchPosition)
        {
            // check if object has not yet been placed, then update touchPosition
            // we want placement to be with only a single touch
            if (Input.touchCount > 0 && Input.touchCount == 1 && spawnedObject == null)
            {
                // update touchPosition to give object position to be instantiated
                touchPosition = Input.GetTouch(0).position;
                return true;
            }
            // just return if not an attempt to place an object
            touchPosition = default;
            return false;
        }

        void TryPlacement()
        {
            // if we can't get a touch position, just return
            if(!CanPlace(out Vector2 touchPosition))
            {
                return;
            }

            // once we have a touch position, shoot a raycast (-:
            if (m_RaycastManager.Raycast(touchPosition, s_Hits, TrackableType.PlaneWithinPolygon))
            {
                Debug.Log("RAYCAST SUCCESS, INTANTIATED OBJECT NOW");
                // Raycast hits are sorted by distance, so the first one
                // will be the closest hit.
                var hitPose = s_Hits[0].pose;
                spawnedObject = Instantiate(m_PlacedPrefab, hitPose.position, hitPose.rotation);
                initScale = spawnedObject.transform.localScale;
            }
        }

        //bool CanScale(out Vector2 touchPosition)
        //{

        //}

        // a function that will handle rescaling depending on the processed touch inputs given
        // by the user
        void TryRescale()
        {
            if (spawnedObject != null && Input.touchCount > 0 && Input.touchCount == 2)
            {
                Debug.Log("RESCALING BEING ATTEMPTED");
                // store each touch
                var touch1 = Input.GetTouch(0);
                var touch2 = Input.GetTouch(1);

                // make sure the touches are continuing together
                if (touch1.phase == TouchPhase.Ended || touch2.phase == TouchPhase.Ended || touch1.phase == TouchPhase.Canceled || touch2.phase == TouchPhase.Canceled)
                {
                    // don't rescale if the touches have ended
                    return;
                }

                // at this point we know that both touches have neither ended nor canceled
                // store the touches initial positions
                else if (touch1.phase == TouchPhase.Began || touch2.phase == TouchPhase.Began)
                {
                    initDist = Vector2.Distance(touch1.position, touch2.position);
                    initScale = spawnedObject.transform.localScale;
                }
                // if none of them have just begun, then some movement is in order
                else if (touch1.phase == TouchPhase.Moved || touch2.phase == TouchPhase.Moved)
                {
                    var currDist = Vector2.Distance(touch1.position, touch2.position);

                    // don't rescale if scaling was accidental, so two-touch done by same finger
                    if (Mathf.Approximately(initDist, 0))
                    {
                        // don't rescale if the scaling is close to zero
                        return;
                    }

                    Debug.Log("RESCALING CHECKS DONE, RESCALING EXECUTING");
                    // rescale that baby
                    var factor = currDist / initDist;
                    spawnedObject.transform.localScale = initScale * factor;
                }
            }
        }

        // TO DO TO DO TO DO
        void TouchingSpawned()
        {
            Debug.Log("CHECKING TO SEE IF WE ARE TOUCHING SPAWNED OBJECT HEHEHE");
            // see if a touch was enacted
            if (spawnedObject != null && Input.touchCount > 0 && Input.touchCount == 1)
            {
                Debug.Log("TOUCH DETECTED, SEEING IF ITS TOUCHING A GAMEOBJECT");
                // store the touch and make sure it hasn't ended
                var touch1 = Input.GetTouch(0);
                if (touch1.phase != TouchPhase.Canceled && touch1.phase != TouchPhase.Ended)
                {
                    Debug.Log("SHOOTING RAYCAST :D");
                    // check to see if this touch induces a raycast that hits the gameobject (TO DO, make sure it's tou
                    // touching the right gameobject if multiple exist)
                    RaycastHit hit;
                    Ray ray = Camera.main.ScreenPointToRay(touch1.position);
                    if (Physics.Raycast(ray, out hit))
                    {
                        Debug.Log("HIT SOMETHING WOOT WOOT: " + hit.collider.gameObject);
                        // if hit a game object, then user wishes to move object
                        if (hit.collider.tag == "Spawnable")
                        {
                            Debug.Log("HIT A GAMEOBJECT :D");
                            spawnedObject.transform.position = touch1.position;
                            // while the user moves their finger, move the painting!
                            // while loop bugged and crashed, so maybe not the best way to go about things
                            hit.collider.gameObject.transform.position = touch1.position;
                        }
                    }
                }
            }
            else
            {
                return;
            }
        }

        // TO DO TO DO TO DO
        /*
        void TryMoving()
        {
            // see 
            if(!TouchingSpawned(out Vector2 touchPosition))
            {
                return;
            }
        }
        */

        void Update()
        {
            // check for touch input, return if none given or object already placed
            Debug.Log("SEEING IF PLACEMENT IS POSSIBLE :D");
            TryPlacement();
            TryRescale();
            // TouchingSpawned();
            // next will be to try moving?
        }
    }
}
