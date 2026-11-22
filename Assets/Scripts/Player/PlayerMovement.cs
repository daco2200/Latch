using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private PlayerInput playerInput;
    private InputAction moveAction;

    [SerializeField] private float speed = 8f;
    [SerializeField] private float railMinMax = 8f;
    private Rigidbody2D rb2d;

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions.FindAction("Move");
    }

    private void FixedUpdate()
    {
        float moveX = moveAction.ReadValue<float>();
        if (RuleState.InvertControls) moveX = -moveX;

        rb2d.linearVelocity = new Vector2(moveX * speed, 0f);

        float clampedX = Mathf.Clamp(
            rb2d.position.x,
            -railMinMax,
            railMinMax
        );

        rb2d.position = new Vector2(clampedX, rb2d.position.y);
    }
}