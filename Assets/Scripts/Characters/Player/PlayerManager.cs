using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerManager : CharacterManager
{

    private PlayerInput playerInput;
    private UnityEngine.InputSystem.InputAction moveAction;

    protected override void Awake()
    {
        base.Awake();
        playerInput = GetComponent<PlayerInput>();
    }

    private void OnMove(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        Vector2 v = ctx.ReadValue<Vector2>();

        // Ignore tiny stick noise
        if (v.sqrMagnitude < 0.01f)
            return;

        // Convert vector to a cardinal direction
        if (Mathf.Abs(v.x) > Mathf.Abs(v.y))
            Movement.SetDirection(v.x > 0 ? Vector2.right : Vector2.left);
        else
            Movement.SetDirection(v.y > 0 ? Vector2.up : Vector2.down);
    }

    private void OnEnable()
    {
        moveAction = playerInput.actions[InputAction.MOVE];
        moveAction.performed += OnMove;
        moveAction.canceled += OnMove; // when released, value becomes zero
        moveAction.Enable();
    }

    private void OnDisable()
    {
        if (moveAction == null) return;

        moveAction.performed -= OnMove;
        moveAction.canceled -= OnMove;
        moveAction.Disable();
    }
}
