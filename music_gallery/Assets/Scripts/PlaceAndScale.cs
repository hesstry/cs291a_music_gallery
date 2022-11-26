using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

// code taken from https://docs.google.com/document/d/1z7xG20s7_jM7ftizfXexMd9b3ycYDfkBCPxUyRG3UNs/edit

[RequireComponent(typeof(ARRaycastManager))]
public class PlaceAndScale : MonoBehaviour
{

    public GameObject arObjectToSpawn;
    public GameObject placementIndicator;
    GameObject spawnedObject;
    Pose PlacementPose;
    ARRaycastManager arRaycastManager;
    static List<ARRaycastHit> hits = new List<ARRaycastHit>();
    bool placementPoseIsValid = false;
    float initialDistance;
    Vector3 initialScale;

    void Awake()
    {
        arRaycastManager = GetComponent<ARRaycastManager>();
    }

    // need to update placement indicator, placement pose and spawn 
    void Update()
    {
        UpdatePlacementPose();
        UpdatePlacementIndicator();

        // if not spawned AND a placement pose is instantiated AND a touch is given AND said touch has just started
        // then place object
        if (spawnedObject == null && placementPoseIsValid && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            ARPlaceObject(); // simply spawns
        }

        // scale using pinch involves two touches
        // we need to count both the touches, store it somewhere, measure the distance between pinch 
        // and scale gameobject depending on the pinch distance
        // we also need to ignore if the pinch distance is small (cases where two touches are registered accidently)
        if (Input.touchCount == 2)
        {
            var touchZero = Input.GetTouch(0);
            var touchOne = Input.GetTouch(1);

            // if any one of touchzero or touchOne is cancelled or maybe ended then do nothing
            if (touchZero.phase == TouchPhase.Ended || touchZero.phase == TouchPhase.Canceled ||
                touchOne.phase == TouchPhase.Ended || touchOne.phase == TouchPhase.Canceled)
            {
                return; // basically do nothing
            }

            if (touchZero.phase == TouchPhase.Began || touchOne.phase == TouchPhase.Began)
            {
                initialDistance = Vector2.Distance(touchZero.position, touchOne.position);
                initialScale = spawnedObject.transform.localScale;
                Debug.Log("Initial Disatance: " + initialDistance + "GameObject Name: "
                    + arObjectToSpawn.name); // Just to check in console
            }
            else // if touch is moved
            {
                var currentDistance = Vector2.Distance(touchZero.position, touchOne.position);

                //if accidentally touched or pinch movement is very very small
                if (Mathf.Approximately(initialDistance, 0))
                {
                    return; // do nothing if it can be ignored where inital distance is very close to zero
                }

                // if currDist > initDist then currDist/initDist > 1
                // leads to enlargment, as fingers stretch the object
                // if currDist < initDist then currDist/initDist < 1
                // leads to shrinkage, as fingers shrink the object
                var factor = currentDistance / initialDistance;
                spawnedObject.transform.localScale = initialScale * factor; // scale multiplied by the factor we calculated
            }
        }
    }
    void UpdatePlacementIndicator()
    {
        // if no object has been spawned, then leave the indicator on the screen
        if (spawnedObject == null && placementPoseIsValid)
        {
            placementIndicator.SetActive(true);
            // placementPose is equal to the raycast that hits a plane starting from the middle of the screen
            placementIndicator.transform.SetPositionAndRotation(PlacementPose.position, PlacementPose.rotation);
        }
        // if an object has been spawned then remove the indicator
        else
        {
            placementIndicator.SetActive(false);
        }
    }

    void UpdatePlacementPose()
    {
        Debug.Log("IN UPDATEPLACEMENTPOSE() UPDATEPLACEMENTPOSE()  UPDATEPLACEMENTPOSE()  UPDATEPLACEMENTPOSE()");
        var screenCenter = Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.5f, 0));
        Debug.Log("screenCenter is null: " + (Object.ReferenceEquals(screenCenter, null)));
        if (arRaycastManager.Raycast(screenCenter, hits, TrackableType.PlaneWithinPolygon))
        {
            Debug.Log("hits is null: " + (Object.ReferenceEquals(hits, null)));
            placementPoseIsValid = true;
            PlacementPose = hits[0].pose;
        }
        else
        {
            placementPoseIsValid = false;
        }
    }

    void ARPlaceObject()
    {
        Debug.Log("PLACING OBJECT HEHEHE HOHOHO");
        spawnedObject = Instantiate(arObjectToSpawn, PlacementPose.position, PlacementPose.rotation);
    }
}