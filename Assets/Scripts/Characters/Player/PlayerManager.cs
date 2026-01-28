using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerManager : CharacterManager
{
    private PlayerInput playerInput;
    private UnityEngine.InputSystem.InputAction moveAction;
    private UnityEngine.InputSystem.InputAction runAction;

    private bool isRunning = false;

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

    private void ToggleRun()
    {
        isRunning = !isRunning;
        Movement.SpeedMultiplier = isRunning ? 3f : 1.5f;
    }

    private void OnEnable()
    {
        moveAction = playerInput.actions[InputAction.MOVE];
        moveAction.performed += OnMove;
        moveAction.canceled += OnMove;
        moveAction.Enable();

        runAction = playerInput.actions[InputAction.RUN];
        runAction.performed += ctx => ToggleRun();
        runAction.Enable();
    }

    private void OnDisable()
    {
        if (moveAction == null) return;

        moveAction.performed -= OnMove;
        moveAction.canceled -= OnMove;
        moveAction.Disable();

        runAction.performed -= ctx => ToggleRun();
        runAction.Disable();
    }

    public override void ResetState()
    {
        base.ResetState();
        isRunning = false;
        ToggleRun();
    }
}
