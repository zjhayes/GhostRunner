using UnityEngine;

public class Gate : MonoBehaviour
{
    [Header("Gates")]
    [SerializeField] private Transform gate1;
    [SerializeField] private Transform gate2;

    [Header("Rotation")]
    [SerializeField] private RotationAxis rotationAxis = RotationAxis.Z;

    [SerializeField] private float gate1ClosedAngle;
    [SerializeField] private float gate1OpenAngle;

    [SerializeField] private float gate2ClosedAngle;
    [SerializeField] private float gate2OpenAngle;

    [Header("Animation")]
    [SerializeField] private float openSpeed = 90f; // degrees per second

    public bool IsOpen { get; private set; }

    public event System.Action OnGateOpened;

    private bool isOpening;

    private void Awake()
    {
        SetGateRotation(closed: true);
    }

    private void Update()
    {
        if (!isOpening)
            return;

        bool gate1Done = RotateGate(
            gate1,
            gate1OpenAngle
        );

        bool gate2Done = RotateGate(
            gate2,
            gate2OpenAngle
        );

        if (gate1Done && gate2Done)
        {
            isOpening = false;
            IsOpen = true;
            OnGateOpened?.Invoke();
        }
    }

    public void Open()
    {
        if (IsOpen || isOpening)
            return;

        isOpening = true;
    }

    private bool RotateGate(Transform gate, float targetAngle)
    {
        Vector3 euler = gate.localEulerAngles;
        float current = GetAxisValue(euler);
        float next = Mathf.MoveTowardsAngle(
            current,
            targetAngle,
            openSpeed * Time.deltaTime
        );

        SetAxisValue(ref euler, next);
        gate.localEulerAngles = euler;

        return Mathf.Abs(Mathf.DeltaAngle(next, targetAngle)) < 0.1f;
    }

    private void SetGateRotation(bool closed)
    {
        SetGateAngle(gate1, closed ? gate1ClosedAngle : gate1OpenAngle);
        SetGateAngle(gate2, closed ? gate2ClosedAngle : gate2OpenAngle);

        IsOpen = !closed;
        isOpening = false;
    }

    private void SetGateAngle(Transform gate, float angle)
    {
        Vector3 euler = gate.localEulerAngles;
        SetAxisValue(ref euler, angle);
        gate.localEulerAngles = euler;
    }

    private float GetAxisValue(Vector3 euler)
    {
        return rotationAxis switch
        {
            RotationAxis.X => euler.x,
            RotationAxis.Y => euler.y,
            _ => euler.z
        };
    }

    private void SetAxisValue(ref Vector3 euler, float value)
    {
        switch (rotationAxis)
        {
            case RotationAxis.X: euler.x = value; break;
            case RotationAxis.Y: euler.y = value; break;
            case RotationAxis.Z: euler.z = value; break;
        }
    }
}

public enum RotationAxis
{
    X,
    Y,
    Z
}