using System.Collections;
using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    [Header("Follow")]
    public Transform target;
    public Vector3 offset;

    [Header("Intro Camera")]
    public bool useIntro = true;
    public Vector3 introPosition;
    public float introDuration = 4f;
    public float transitionToFollowTime = 1f;

    [Header("Smoothing")]
    public float followSmoothTime = 0.15f;

    [Header("Intro Y Lock (temporary)")]
    [Tooltip("Lock Y during intro/transition only. After that, camera follows Y normally.")]
    public bool lockYDuringIntro = true;

    private bool followingPlayer = false;
    private bool lockYActive = false;
    private float lockedY;
    private Vector3 velocity;

    void Start()
    {
        if (useIntro)
        {
            transform.position = introPosition;

            // lock Y only for intro if enabled
            lockYActive = lockYDuringIntro;
            lockedY = introPosition.y;

            StartCoroutine(IntroSequence());
        }
        else
        {
            followingPlayer = true;
        }
    }

    IEnumerator IntroSequence()
    {
        yield return new WaitForSeconds(introDuration);

        // Move camera toward player (optionally keep intro Y while moving)
        yield return MoveToTarget();

        // After the transition, unlock Y so it follows normally
        lockYActive = false;

        // Recalculate offset to keep framing consistent when follow starts
        if (target != null)
            offset = transform.position - target.position;

        // Clear SmoothDamp velocity so it doesn't "settle" weirdly
        velocity = Vector3.zero;

        followingPlayer = true;
    }

    IEnumerator MoveToTarget()
    {
        if (target == null) yield break;

        Vector3 startPos = transform.position;
        Vector3 endPos = target.position + offset;

        if (lockYActive)
            endPos.y = lockedY;

        if (transitionToFollowTime <= 0f)
        {
            transform.position = endPos;
            yield break;
        }

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / transitionToFollowTime;
            float eased = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(t));
            Vector3 p = Vector3.Lerp(startPos, endPos, eased);

            if (lockYActive)
                p.y = lockedY;

            transform.position = p;
            yield return null;
        }

        transform.position = endPos;
    }

    void LateUpdate()
    {
        if (!followingPlayer || target == null)
            return;

        Vector3 desiredPos = target.position + offset;

        // Only lock Y during intro (if enabled)
        if (lockYActive)
            desiredPos.y = lockedY;

        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredPos,
            ref velocity,
            followSmoothTime
        );
    }
}
