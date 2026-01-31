using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerManager : CharacterManager
{
    [SerializeField] LanternController lanternController;

    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction runAction;

    private bool isRunning = false;

    public LanternController Lantern => lanternController;

    protected override void Awake()
    {
        base.Awake();
        playerInput = GetComponent<PlayerInput>();
    }

    private void OnMove(InputAction.CallbackContext ctx)
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
        moveAction = playerInput.actions[InputBinding.MOVE];
        moveAction.performed += OnMove;
        moveAction.canceled += OnMove;
        moveAction.Enable();

        runAction = playerInput.actions[InputBinding.RUN];
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
