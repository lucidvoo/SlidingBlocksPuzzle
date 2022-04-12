using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// задаются маркеры, обозначающие прямоугольник, который должен входить в поле зрения камеры.
// Камера независимо от аспекта поддерживает поле зрения так, чтобы прямоугольник упирался в края поля зрения.
// у камеры меняется position для этого. Камера смотрит в центр прямоугольника.
// камера должна смотреть вдоль оси Z. Маркеры лежат в плоскости, перпендикулярной Z.
// (по идее это не нужно если аспект горизонтальный, это нужно если аспект в игре может стать вертикальным.)
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
        // рассчитываем камеру только если размер экрана изменился
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

        // расстояние от камеры до маркеров
        float deltaZ;
        // Z рассчитывается по простой тригонометрической формуле. Но зависит рассчет от аспектов прямоугольника и экрана
        if ((TRMarker.x - BLMarker.x)/(TRMarker.y - BLMarker.y) > screenAspect) // прямоугольник упирается боками
        {
            float camFOVHor = Camera.VerticalToHorizontalFieldOfView(cameraCompon.fieldOfView, screenAspect);
            deltaZ = (TRMarker.x - BLMarker.x) * 0.5f / (Mathf.Tan(Mathf.Deg2Rad * camFOVHor * 0.5f));
        }
        else // прямоугольник упирается верхом и низом
        {
            float camFOVVert = cameraCompon.fieldOfView;
            deltaZ = (TRMarker.y - BLMarker.y) * 0.5f / (Mathf.Tan(Mathf.Deg2Rad * camFOVVert * 0.5f));
        }

        float markersZPos = (TopRightMarker.position.z + BottomLeftMarker.position.z) * 0.5f;
        transform.position = new Vector3(camX, camY, markersZPos - deltaZ);
    }
}
