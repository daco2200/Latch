using UnityEngine;
using UnityEngine.InputSystem;

public class AimController : MonoBehaviour
{
    private PlayerInput playerInput;
    private InputAction pointAction;

    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform aimPoint;

    [SerializeField, Range(0f, 1f)] private float bottomMargin = 0.15f;
    [SerializeField, Range(0f, 1f)] private float topMargin = 0.10f;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        pointAction = playerInput.actions.FindAction("Point");
    }

    private void Update()
    {
        Vector2 mousePosition = pointAction.ReadValue<Vector2>();

        Vector3 viewportPosition = mainCamera.ScreenToViewportPoint(mousePosition);

        viewportPosition.x = Mathf.Clamp(viewportPosition.x, 0f, 1f);
        viewportPosition.y = Mathf.Clamp(viewportPosition.y, bottomMargin, 1 - topMargin);

        Vector3 worldPosition = mainCamera.ViewportToWorldPoint(viewportPosition);
        worldPosition.z = 0f;
        aimPoint.position = worldPosition;
    }
}
