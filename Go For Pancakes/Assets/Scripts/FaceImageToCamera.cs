using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceImageToCamera : MonoBehaviour
{
    Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void LateUpdate()
    {
        mainCamera = Camera.main;
        transform.LookAt(mainCamera.transform);        
    }
}
