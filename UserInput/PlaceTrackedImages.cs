using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
//GIT_TEST_DEBUG_UNSAFE_DIRECTORIES = true;
[RequireComponent(typeof(ARTrackedImageManager))]
public class PlaceTrackedImages : MonoBehaviour
{   // global variables
    private ARTrackedImageManager _trackedImageManager;
    // an array of GameObjects
    public GameObject[] ArPrefabs;

    private readonly Dictionary<string, GameObject> _intantiatedPrefabs = new Dictionary<string, GameObject>();

    private void Awake()
    {
        _trackedImageManager = GetComponent<ARTrackedImageManager>();
        Debug.Log("Awake function");
    }
    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        Debug.Log("OnTrackedImagesChanged function");

        foreach (var trackedImage in eventArgs.added)
        {
            var imageName = trackedImage.referenceImage.name;
            //loop over the array of prefabs
            foreach (var curPrefab in ArPrefabs)
            {
                if (string.Compare(curPrefab.name, imageName, StringComparison.OrdinalIgnoreCase) == 0
                    && !_intantiatedPrefabs.ContainsKey(imageName))
                {
                    var newPrefab = Instantiate(curPrefab, trackedImage.transform);
                    _intantiatedPrefabs[imageName] = newPrefab;
                }
            }
        }
        foreach (var trackedImage in eventArgs.updated)
        {
            _intantiatedPrefabs[trackedImage.referenceImage.name]
                .SetActive(trackedImage.trackingState == TrackingState.Tracking);
        }
        foreach (var trackedImage in eventArgs.removed)
        {
            Destroy(_intantiatedPrefabs[trackedImage.referenceImage.name]);
            _intantiatedPrefabs.Remove(trackedImage.referenceImage.name);
        }

    }

    private void OnEnable()
    {
        _trackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
        Debug.Log("OnEnable function");
    }
    private void OnDisable()
    {
        _trackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
        Debug.Log("OnDisable function");
    }

    
}
