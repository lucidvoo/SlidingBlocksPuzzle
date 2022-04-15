using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// there's markers those form a rectangle. It will fill camera view by changing camera Z coord.
// works for both horizontal and vertical aspects
// Camera looks at the center of the rectangle
// Camera must look strictly at the direction of Z axis.
// mainly this all needed for vertical aspects.
[RequireComponent(typeof(Camera))]
public class CameraMarkers : MonoBehaviour
{
    [SerializeField] Transform TopRightMarker;
    [SerializeField] Transform BottomLeftMarker;

    private float screenAspect;
    private Camera cameraCompon;
    private Vector3 TRMarker, BLMarker;

    void Start()
    {
        if (TopRightMarker == null || BottomLeftMarker == null)
        {
            Debug.LogError("Camera markers haven't set!");
        }
        
        TRMarker = TopRightMarker.position;
        BLMarker = BottomLeftMarker.position;

        cameraCompon = GetComponent<Camera>();

        AdjustCamera();
    }


    void LateUpdate()
    {
        // adjust camera if only aspect changed
        if (screenAspect != cameraCompon.aspect)
        {
            AdjustCamera();
        }
    }

    private void AdjustCamera()
    {
        float camX, camY;
        camX = (TRMarker.x + BLMarker.x) * 0.5f;
        camY = (TRMarker.y + BLMarker.y) * 0.5f;

        screenAspect = cameraCompon.aspect;

        // distance between camera and markers
        float deltaZ;
        // Z is calculated using trigonometry equations. Calculation depends on aspect of camera and rectangle
        if ((TRMarker.x - BLMarker.x)/(TRMarker.y - BLMarker.y) > screenAspect) // rectangle touches left and right sides of view
        {
            float camFOVHor = Camera.VerticalToHorizontalFieldOfView(cameraCompon.fieldOfView, screenAspect);
            deltaZ = (TRMarker.x - BLMarker.x) * 0.5f / (Mathf.Tan(Mathf.Deg2Rad * camFOVHor * 0.5f));
        }
        else // rectangle touches top and bottom sides of view
        {
            float camFOVVert = cameraCompon.fieldOfView;
            deltaZ = (TRMarker.y - BLMarker.y) * 0.5f / (Mathf.Tan(Mathf.Deg2Rad * camFOVVert * 0.5f));
        }

        float markersZPos = (TopRightMarker.position.z + BottomLeftMarker.position.z) * 0.5f;
        transform.position = new Vector3(camX, camY, markersZPos - deltaZ);
    }
}
