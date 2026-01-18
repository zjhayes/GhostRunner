using System.Collections;
using UnityEngine;

public class GhostHome : GhostBehaviour
{
    private float transitionTime = 0.5f;

    private void OnEnable()
    {
        StopAllCoroutines();
    }

    private void OnDisable()
    {
        StartCoroutine(ExitTransition());
    }

    private IEnumerator ExitTransition()
    {
        Context.Ghost.Movement.SetDirection(Vector2.up, true);
        Context.Ghost.Movement.Rigidbody.bodyType = RigidbodyType2D.Kinematic;
        Context.Ghost.Movement.enabled = false;

        Vector3 position = transform.position;
        float duration = transitionTime;
        float elapsed = 0.0f;

        // Move to center of home.
        while (elapsed < duration)
        {
            Vector3 newPosition = Vector3.Lerp(position, Context.HomeTransform.position, elapsed / duration);
            newPosition.z = position.z;

            Context.Ghost.transform.position = newPosition;
            elapsed += Time.deltaTime;
            yield return null;
        }

        elapsed = 0.0f;

        // Move to exit position.
        while (elapsed < duration)
        {
            Vector3 newPosition = Vector3.Lerp(Context.HomeTransform.position, Context.ExitTransform.position, elapsed / duration);
            newPosition.z = position.z;

            Context.Ghost.transform.position = newPosition;
            elapsed += Time.deltaTime;
            yield return null;
        }

        Vector2 randomDirection = new(Random.value < 0.5f ? -1.0f : 1.0f, 0.0f);
        Context.Ghost.Movement.SetDirection(randomDirection, true);
        Context.Ghost.Movement.Rigidbody.bodyType = RigidbodyType2D.Dynamic;
        Context.Ghost.Movement.enabled = true;
    }
}
