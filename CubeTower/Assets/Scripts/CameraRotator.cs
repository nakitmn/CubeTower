using UnityEngine;

public class CameraRotator : MonoBehaviour
{

    public Transform CameraTransform;
    public float RotationSpeed = 10f;

    // Update is called once per frame
    void Update()
    {
        CameraTransform.Rotate(0, RotationSpeed * Time.deltaTime, 0);
    }
}
