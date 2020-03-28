using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceImageToCamera : MonoBehaviour
{
    GameObject lookTarget;

    void Start()
    {
        lookTarget = GameObject.FindGameObjectWithTag("GameController");
    }

    void LateUpdate()
    {
        transform.eulerAngles = Vector3.zero;   
    }
}
