using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;

// adapted from AR Foundation Samples SimpleAR and extended by adding option to let user to choose scaling or moving of object

namespace UnityEngine.XR.ARFoundation.Samples
{
    /// <summary>
    /// Listens for touch events and performs an AR raycast from the screen touch point.
    /// AR raycasts will only hit detected trackables like feature points and planes.
    ///
    /// If a raycast hits a trackable, the <see cref="placedPrefab"/> is instantiated
    /// and moved to the hit position.
    /// </summary>
    [RequireComponent(typeof(ARRaycastManager))]
    public class PlaceScaleDrag : MonoBehaviour
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
        bool canPlace, canScale, canMove, touchingSpawned;
        GameObject selectedObject;

        // buttons for scaling and moving
        public Button scaleButton, moveButton, allDoneButton;

        // canvas for the function buttons
        public GameObject ScaleMoveCanvas, AllDoneCanvas;

        // global touch
        Touch? touch = null;
        Vector2 touchPosition = default;

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

            ScaleMoveCanvas.SetActive(false);
            AllDoneCanvas.SetActive(false);

            if (ScaleMoveCanvas != null && scaleButton != null && moveButton != null && AllDoneCanvas != null && allDoneButton != null)
            {
                scaleButton.onClick.AddListener(clickedScaleButton);
                moveButton.onClick.AddListener(clickedMoveButton);
                allDoneButton.onClick.AddListener(clickedAllDoneButton);
            }
        }

        void clickedScaleButton()
        {
            Debug.Log("CLICKED SCALE BUTTON");
            // stop showing the buttons once a functionality has been chosen
            ScaleMoveCanvas.SetActive(false);
            AllDoneCanvas.SetActive(true);
            canScale = true;
            canMove = false;
        }

        void clickedMoveButton()
        {
            Debug.Log("CLICKED MOVE BUTTON");
            // stop showing buttons once a functionality has been chosen
            ScaleMoveCanvas.SetActive(false);
            AllDoneCanvas.SetActive(true);
            canScale = false;
            canMove = true;
        }

        void clickedAllDoneButton()
        {
            Debug.Log("CLICKED ALL DONE BUTTON");
            AllDoneCanvas.SetActive(false);
            // turn off scaling and moving functionality once they've been finished with
            canScale = false;
            canMove = false;
            // return selected object to null
            selectedObject = null;
        }

        // this function is called once a touch was recorded, and user is actively trying to place an object
        // start raycast and place object at touched position
        void TryPlacement(Vector2 placePosition)
        {
            // once we have a placePosition, shoot a raycast (-:
            if (m_RaycastManager.Raycast(placePosition, s_Hits, TrackableType.PlaneWithinPolygon))
            {
                Debug.Log("RAYCAST SUCCESS, INTANTIATED OBJECT NOW");
                // Raycast hits are sorted by distance, so the first one
                // will be the closest hit.
                var hitPose = s_Hits[0].pose;
                spawnedObject = Instantiate(m_PlacedPrefab, hitPose.position, hitPose.rotation);
                // update the initial scale for the resizing method to have information for rescaling
                initScale = spawnedObject.transform.localScale;
            }
        }

        // a function that will handle rescaling depending on the processed touch inputs given
        // by the user
        // adapted from https://docs.google.com/document/d/1z7xG20s7_jM7ftizfXexMd9b3ycYDfkBCPxUyRG3UNs/edit
        void TryRescale(GameObject selectedObject)
        {
            if (Input.touchCount > 0 && Input.touchCount == 2)
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
                    initScale = selectedObject.transform.localScale;
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
                    selectedObject.transform.localScale = initScale * factor;
                }
            }
        }

        // function that let's user select a game object for manipulation
        void TouchingSpawned(out GameObject touchedObject, Vector2 touchPosition)
        {
            // instantiate it to null and if it's not null at the end, we've touched a GameObject
            touchedObject = null;
            // check to see if this touch induces a raycast that hits the gameobject (TO DO, make sure it's
            // touching the right gameobject if multiple exist)
            RaycastHit hitObject;
            Ray ray = Camera.main.ScreenPointToRay(touchPosition);
            if (Physics.Raycast(ray, out hitObject))
            {
                Debug.Log("HIT SOMETHING WOOT WOOT: " + hitObject.collider.gameObject);
                // if hit a game object, then user wishes to move object
                if (hitObject.collider.tag == "Spawnable")
                {
                    touchedObject = hitObject.collider.gameObject;
                }
            }
        }

        void TryMove(GameObject selectedObject, Vector2 touchPosition)
        {
            Debug.Log("ATTEMPTING TO MOVE OBJECT, LET'S SEE HEHEHE");
            // nullable type requires Variable?.Value to access non-null stored value of Type = TypeOf(Variable)
            if ((touch.Value.phase == TouchPhase.Stationary) || (touch.Value.phase == TouchPhase.Moved) && m_RaycastManager.Raycast(touch.Value.position, s_Hits, TrackableType.PlaneWithinPolygon))
            {
                Pose hitPose = s_Hits[0].pose;
                selectedObject.transform.SetPositionAndRotation(hitPose.position, hitPose.rotation);
            }
        }

        void Update()
        {
            // first record touch and determine what kind of actions can follow
            if (Input.touchCount > 0 && Input.touchCount == 1)
            {
                touch = Input.GetTouch(0);
                touchPosition = touch.Value.position;
            }

            // first update bools to limit what the user can do
            if (spawnedObject == null)
            {
                canPlace = true;
            }
            else
            {
                canPlace = false;
            }

            // if placeable, let user place if a touch was given that was stored in nullable type touch
            if (canPlace && touch != null)
            {
                TryPlacement(touchPosition);
            }

            // let user select an object if they haven't already
            if (selectedObject == null && touch != null)
            {
                // see if user wants to select an object
                TouchingSpawned(out selectedObject, touchPosition);
                // show the user function options if gameobject selected
                if (selectedObject != null)
                {
                    ScaleMoveCanvas.SetActive(true);
                }
            }

            // else if an objected was already selected and user is still doing something with it, keep doing that thing
            else if (selectedObject != null && AllDoneCanvas.activeSelf)
            {
                // perform functionality based on users option
                if (canScale)
                {
                    TryRescale(selectedObject);
                }
                if (canMove && touch != null)
                {
                    TryMove(selectedObject, touchPosition);
                }
            }
        }
    }
}