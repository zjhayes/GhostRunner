using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostHome : GhostBehaviour
{
    private float transitionTime = 0.5f;
    protected override bool TryPickDirection(Node node, IReadOnlyList<Cardinal> candidates, out Cardinal chosen)
    {
        chosen = default;
        return false;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        StopAllCoroutines();
        Context.Ghost.Movement.Stop();
        Context.Ghost.Movement.Rigidbody.linearVelocity = Vector2.zero;
        Context.Ghost.Movement.Rigidbody.bodyType = RigidbodyType2D.Kinematic;
        Context.Ghost.Movement.enabled = false;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        if (!gameObject.activeInHierarchy)
            return;

        StartCoroutine(ExitTransition());
    }

    private IEnumerator ExitTransition()
    {
        Context.Ghost.Movement.SetDirection(Vector2.up, true);
        Context.Ghost.Movement.Rigidbody.bodyType = RigidbodyType2D.Kinematic;
        Context.Ghost.Movement.enabled = false;

        Vector2 position = Context.Ghost.Movement.Rigidbody.position;
        float duration = transitionTime;
        float elapsed = 0.0f;

        // Move to center of home.
        while (elapsed < duration)
        {
            Vector2 newPosition = Vector2.Lerp(position, Context.HomeTransform.position, elapsed / duration);
            Context.Ghost.Movement.Rigidbody.MovePosition(newPosition);
            elapsed += Time.deltaTime;
            yield return null;
        }

        elapsed = 0.0f;

        // Move to exit position.
        while (elapsed < duration)
        {
            Vector2 newPosition = Vector2.Lerp(Context.HomeTransform.position, Context.ExitTransform.position, elapsed / duration);
            Context.Ghost.Movement.Rigidbody.MovePosition(newPosition);
            elapsed += Time.deltaTime;
            yield return null;
        }

        Cardinal cardinal = Random.value < 0.5f ? Cardinal.West : Cardinal.East;
        Context.Ghost.Movement.ApplyDirection(cardinal);
        Context.Ghost.Movement.Rigidbody.bodyType = RigidbodyType2D.Dynamic;
        Context.Ghost.Movement.enabled = true;
    }
}
