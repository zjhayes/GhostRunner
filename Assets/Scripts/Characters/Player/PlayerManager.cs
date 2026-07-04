using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerManager : CharacterManager
{
    [SerializeField] LanternController lanternController;
    [SerializeField] float walkSpeed = 1.5f;
    [SerializeField] float runSpeed = 3f;

    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction runAction;

    private bool isRunning = true;

    public LanternController Lantern => lanternController;

    protected void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    protected override void Start()
    {
        base.Start();
        SetRun();
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
        SetRun();
    }

    private void SetRun()
    {
        Movement.SpeedMultiplier = isRunning ? runSpeed : walkSpeed;
    }

    public void PlayerFrightened()
    {
        Debug.Log("Player Frightened");
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
        SetRun();
    }
}
