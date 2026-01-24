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

        // Ignore tiny stick noise (and ignore release)
        if (v.sqrMagnitude < Numeric.HUNDREDTH)
            return;

        Movement.SetDirection(Conversion.QuantizeToCardinal(v));
    }

    private void OnEnable()
    {
        moveAction = playerInput.actions[InputAction.MOVE];
        moveAction.performed += OnMove;
        moveAction.canceled += OnMove;
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
