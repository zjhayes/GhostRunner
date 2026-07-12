using System.Collections.Generic;
using UnityEngine;

public interface ICheckpointState
{
    void CaptureCheckpointState(CheckpointSnapshot snapshot);
    void RestoreCheckpointState(CheckpointSnapshot snapshot);
}

public sealed class CheckpointSnapshot
{
    private readonly Dictionary<string, bool> boolValues = new();

    public void SetBool(string key, bool value)
    {
        boolValues[key] = value;
    }

    public bool TryGetBool(string key, out bool value)
    {
        return boolValues.TryGetValue(key, out value);
    }

    public static string GetKey(Component component)
    {
        string path = string.Empty;

        for (Transform current = component.transform; current != null; current = current.parent)
            path = $"/{current.name}[{GetSameNameSiblingIndex(current)}]{path}";

        return $"{component.gameObject.scene.path}:{path}:{component.GetType().FullName}";
    }

    private static int GetSameNameSiblingIndex(Transform transform)
    {
        if (transform.parent == null)
            return 0;

        int sameNameIndex = 0;
        int siblingIndex = transform.GetSiblingIndex();

        for (int i = 0; i < siblingIndex; i++)
        {
            if (transform.parent.GetChild(i).name == transform.name)
                sameNameIndex++;
        }

        return sameNameIndex;
    }
}

public class Checkpoint : GameBehaviour
{
    private static bool hasActiveCheckpoint;
    private static Vector2 checkpointPosition;
    private static Cardinal checkpointDirection;
    private static CheckpointSnapshot snapshot;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetCheckpoint()
    {
        hasActiveCheckpoint = false;
        checkpointPosition = default;
        checkpointDirection = default;
        snapshot = null;
    }

    public void Activate(Node node, Cardinal startDirection)
    {
        if (node == null)
            return;
        checkpointPosition = node.transform.position;
        checkpointDirection = startDirection;
        snapshot = new CheckpointSnapshot();
        VisitStateParticipants(participant => participant.CaptureCheckpointState(snapshot));
        hasActiveCheckpoint = true;
    }

    public void Restore(PlayerManager player)
    {
        if (!hasActiveCheckpoint || player == null || player.Movement == null)
            return;

        player.Movement.ResetState(checkpointPosition, checkpointDirection);

        if (snapshot != null)
            VisitStateParticipants(participant => participant.RestoreCheckpointState(snapshot));
    }

    private static void VisitStateParticipants(System.Action<ICheckpointState> visit)
    {
        MonoBehaviour[] behaviours = Object.FindObjectsByType<MonoBehaviour>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );

        foreach (MonoBehaviour behaviour in behaviours)
        {
            if (behaviour is ICheckpointState participant)
                visit(participant);
        }
    }
}
