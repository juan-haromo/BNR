using UnityEngine;

public class CameraFacing_HP_Bar : MonoBehaviour
{

    Transform cameraTransform;
    Vector3 direction;

    private void Awake()
    {
        cameraTransform = Camera.main.transform;
    }


    private void LateUpdate()
    {
        direction = cameraTransform.transform.position - transform.position;
        direction.y = 0;
        transform.rotation = Quaternion.LookRotation(-direction);
    }
}
