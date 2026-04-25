using UnityEngine;

public class ModelOrbitController : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Orbit")]
    public float orbitSpeed = 200f;
    public float zoomSpeed = 5f;
    public float panSpeed = 0.3f;
    public float minDistance = 1f;
    public float maxDistance = 50f;

    private float _distance = 10f;
    private float _yaw;
    private float _pitch = 20f;

    private void LateUpdate()
    {
        if (target == null) return;

        if (Input.GetMouseButton(0))
        {
            _yaw   += Input.GetAxis("Mouse X") * orbitSpeed * Time.deltaTime;
            _pitch -= Input.GetAxis("Mouse Y") * orbitSpeed * Time.deltaTime;
            _pitch  = Mathf.Clamp(_pitch, -80f, 80f);
        }

        if (Input.GetMouseButton(1))
        {
            float dx = -Input.GetAxis("Mouse X") * panSpeed;
            float dy = -Input.GetAxis("Mouse Y") * panSpeed;
            target.position += transform.right * dx + transform.up * dy;
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        _distance = Mathf.Clamp(_distance - scroll * zoomSpeed, minDistance, maxDistance);

        Quaternion rotation = Quaternion.Euler(_pitch, _yaw, 0f);
        transform.position = target.position + rotation * (Vector3.back * _distance);
        transform.LookAt(target);
    }

    public void ResetView()
    {
        _yaw = 0f;
        _pitch = 20f;
        _distance = 10f;
        if (target != null) target.position = Vector3.zero;
    }

    public void SetOrbitAngles(float yaw, float pitch)
    {
        _yaw = yaw;
        _pitch = pitch;
    }
}
