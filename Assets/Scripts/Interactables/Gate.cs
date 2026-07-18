using System.Collections;
using UnityEngine;

public class Gate : MonoBehaviour, ICheckpointState
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
    [SerializeField] private float openSpeed = 90f; // Initial degrees per second
    [SerializeField] private float fastOpenDegrees = 45f;
    [SerializeField] private float finishingOpenSpeed = 50f;

    [Header("Audio")]
    [SerializeField] private AudioSource openAudioSource;
    [SerializeField] private AudioSource lockedAudioSource;

    public bool IsOpen { get; private set; }
    public bool IsPassable => IsOpen || isOpening;

    public event System.Action OnGateOpened;

    private bool isOpening;
    private Coroutine openRoutine;

    private string CheckpointKey => CheckpointSnapshot.GetKey(this);

    private void Awake()
    {
        SetGateRotation(closed: true);
    }

    public void Open()
    {
        if (IsOpen || isOpening)
            return;

        isOpening = true;
        openAudioSource.Play();
        openRoutine = StartCoroutine(OpenRoutine());
    }

    public void PlayLockedSound()
    {
        lockedAudioSource.Play();
    }

    private IEnumerator OpenRoutine()
    {
        bool gate1Done;
        bool gate2Done;

        do
        {
            gate1Done = RotateGate(gate1, gate1ClosedAngle, gate1OpenAngle);
            gate2Done = RotateGate(gate2, gate2ClosedAngle, gate2OpenAngle);

            if (!gate1Done || !gate2Done)
                yield return null;
        }
        while (!gate1Done || !gate2Done);

        openRoutine = null;
        isOpening = false;
        IsOpen = true;
        OnGateOpened?.Invoke();
    }

    private bool RotateGate(Transform gate, float closedAngle, float targetAngle)
    {
        Vector3 euler = gate.localEulerAngles;
        float current = GetAxisValue(euler);
        float distanceOpened = Mathf.Abs(Mathf.DeltaAngle(closedAngle, current));
        float currentSpeed = distanceOpened < fastOpenDegrees
            ? openSpeed
            : finishingOpenSpeed;

        float next = Mathf.MoveTowardsAngle(
            current,
            targetAngle,
            currentSpeed * Time.deltaTime
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

    private void OnDisable()
    {
        if (openRoutine != null)
        {
            StopCoroutine(openRoutine);
            openRoutine = null;
        }

        isOpening = false;
    }

    public void CaptureCheckpointState(CheckpointSnapshot snapshot)
    {
        snapshot.SetBool(CheckpointKey, IsOpen || isOpening);
    }

    public void RestoreCheckpointState(CheckpointSnapshot snapshot)
    {
        if (!snapshot.TryGetBool(CheckpointKey, out bool wasOpen))
            return;

        if (openRoutine != null)
        {
            StopCoroutine(openRoutine);
            openRoutine = null;
        }

        SetGateRotation(closed: !wasOpen);
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
