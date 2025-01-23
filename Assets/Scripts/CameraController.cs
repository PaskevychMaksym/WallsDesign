using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float rotationSpeed = 100f;
    [SerializeField] private float zoomSpeed = 10f;
    [SerializeField] private float minFOV = 20f;
    [SerializeField] private float maxFOV = 100f;
    
    private Vector3 lastMousePosition;
    private Camera cameraComponent;

    private void Awake()
    {
        cameraComponent = GetComponent<Camera>();
    }

    private void Update()
    {
        HandleRotation();
        HandleZoom();
    }

    private void HandleRotation()
    {
        if (Input.GetMouseButton(1))
        {
            Vector3 deltaMouse = Input.mousePosition - lastMousePosition;
            float rotationX = deltaMouse.x * rotationSpeed * Time.deltaTime;
            float rotationY = -deltaMouse.y * rotationSpeed * Time.deltaTime;
            
            transform.RotateAround(target.position, Vector3.up, rotationX);
            transform.RotateAround(target.position, transform.right, rotationY);
        }

        lastMousePosition = Input.mousePosition;
    }

    private void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (!(Mathf.Abs(scroll) > 0.01f))
        {
            return;
        }

        cameraComponent.fieldOfView -= scroll * zoomSpeed;
        cameraComponent.fieldOfView = Mathf.Clamp(cameraComponent.fieldOfView, minFOV, maxFOV);
    }
}
