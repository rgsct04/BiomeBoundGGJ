using System.Collections;
using UnityEngine;

public class UIMove : MonoBehaviour
{
    [Header("Points (RectTransforms under same Canvas)")]
    public RectTransform pointA;
    public RectTransform pointB;
    public RectTransform pointC;

    [Header("Timing")]
    public float durationAB = 0.6f;
    public float holdAtBSeconds = 1.5f;
    public float durationBC = 0.6f;

    [Header("Options")]
    public bool useSmoothStep = true;
    public bool disableAfterC = false;

    [Header("Input Lock")]
    [Tooltip("If true, player movement inputEnabled is disabled while the UI sequence plays.")]
    public bool lockPlayerInput = true;

    [Tooltip("If > 0, unlock after this many seconds (e.g., 4). If 0, unlock when sequence finishes.")]
    public float forceUnlockAfterSeconds = 4f;

    private RectTransform rect;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    void Start()
    {
        StartCoroutine(PlaySequence());
    }

    IEnumerator PlaySequence()
    {
        if (pointA == null || pointB == null || pointC == null)
        {
            Debug.LogError("MoveUIABC: Assign PointA, PointB, and PointC in the Inspector.");
            yield break;
        }

        // Lock player input at start (movement only)
        if (lockPlayerInput)
            SetPlayerInputEnabled(false);

        // Optional timed force unlock (so you're guaranteed to regain control)
        Coroutine forcedUnlock = null;
        if (lockPlayerInput && forceUnlockAfterSeconds > 0f)
            forcedUnlock = StartCoroutine(ForceUnlockAfter(forceUnlockAfterSeconds));

        // Set to A immediately
        rect.anchoredPosition = pointA.anchoredPosition;

        // Move A -> B
        yield return MoveTo(pointB.anchoredPosition, durationAB);

        // Hold at B
        yield return new WaitForSeconds(holdAtBSeconds);

        // Move B -> C
        yield return MoveTo(pointC.anchoredPosition, durationBC);

        // If we DIDN'T force-unlock by timer, unlock now when animation finishes
        if (lockPlayerInput && forceUnlockAfterSeconds <= 0f)
            SetPlayerInputEnabled(true);

        // If we used force unlock timer, stop it (if it hasn't fired yet) and ensure unlocked
        if (lockPlayerInput && forceUnlockAfterSeconds > 0f)
        {
            if (forcedUnlock != null) StopCoroutine(forcedUnlock);
            SetPlayerInputEnabled(true);
        }

        if (disableAfterC)
            gameObject.SetActive(false);
    }

    IEnumerator ForceUnlockAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        SetPlayerInputEnabled(true);
    }

    void SetPlayerInputEnabled(bool enabled)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        PlayerMovement2D pm = player.GetComponent<PlayerMovement2D>();
        if (pm != null)
            pm.inputEnabled = enabled;
    }

    IEnumerator MoveTo(Vector2 target, float duration)
    {
        Vector2 start = rect.anchoredPosition;
        float t = 0f;

        if (duration <= 0f)
        {
            rect.anchoredPosition = target;
            yield break;
        }

        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            float eased = useSmoothStep
                ? Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(t))
                : Mathf.Clamp01(t);

            rect.anchoredPosition = Vector2.Lerp(start, target, eased);
            yield return null;
        }

        rect.anchoredPosition = target;
    }
}
