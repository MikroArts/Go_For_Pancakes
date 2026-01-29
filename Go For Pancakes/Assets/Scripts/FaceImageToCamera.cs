using UnityEngine;

public class FaceImageToCamera : MonoBehaviour
{
    GameObject lookTarget;

    void Start()
    {
        lookTarget = GameController.gameController.gameObject;
    }

    void LateUpdate()
    {
        transform.eulerAngles = Vector3.zero;
    }
}
