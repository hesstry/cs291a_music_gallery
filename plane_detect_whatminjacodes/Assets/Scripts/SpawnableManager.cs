using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;

public class SpawnableManager : MonoBehaviour
{
    [SerializeField] private ARRaycastManager _raycastManager;
    [SerializeField] private GameObject _spawnablePrefab;

    private List<ARRaycastHit> _raycastHits = new List<ARRaycastHit>();
    private GameObject _spawnedObject;

    private void Start()
    {
        _spawnedObject = null;
    }

    private void Update()
    {
        // No touch events
        if (Input.touchCount == 0)
        {
            return;
        }

        // Save the found touch event
        Touch touch = Input.GetTouch(0);

        if (_raycastManager.Raycast(touch.position, _raycastHits))
        {
            // Beginning of the touch, this triggers when the finger first touches the screen
            if (touch.phase == TouchPhase.Began)
            {
                // check if touching UI element first
                bool isTouchOverUI = touch.position.IsPointOverUIObject();

                if(!isTouchOverUI)
                {
                    // Spawn a GameObject
                    SpawnPrefab(_raycastHits[0].pose.position);
                }
            }
            // Finger still touching the screen and moving, GameObject has already been instantiated
            else if (touch.phase == TouchPhase.Moved && _spawnedObject != null)
            {
                // Moves the spawned GameObject where the finger is moving
                _spawnedObject.transform.position = _raycastHits[0].pose.position;
            }

            // Finger lifted from the screen
            if (touch.phase == TouchPhase.Ended)
            {
                // Don't track the object anymore
                _spawnedObject = null;
            }
        }
    }


    // Instantiate a GameObject to the location where finger was touching the screen
    private void SpawnPrefab(Vector3 spawnPosition)
    {
        _spawnedObject = Instantiate(_spawnablePrefab, spawnPosition, Quaternion.identity);
    }
}