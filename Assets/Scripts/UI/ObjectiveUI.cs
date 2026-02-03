using System.Collections;
using UnityEngine;
using TMPro;

public class ObjectiveUI : MonoBehaviour
{
    public static ObjectiveUI Instance { get; private set; }

    [Header("UI")]
    public TMP_Text objectiveText;
    public TMP_Text counterText;

    [Header("Goal")]
    public int totalPieces = 3;
    public int CurrentPieces { get; private set; } = 0;

    [Header("Animation Points (UI GameObjects)")]
    public RectTransform pointA; // off-screen start
    public RectTransform pointB; // on-screen visible
    public RectTransform pointC; // off-screen exit

    [Header("Timing")]
    public float slideInTime = 0.35f;
    public float stayTime = 2.0f;
    public float slideOutTime = 0.35f;

    private RectTransform rect;
    private Coroutine animRoutine;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        rect = GetComponent<RectTransform>();

        // start hidden at C
        if (pointC != null)
            rect.anchoredPosition = pointC.anchoredPosition;

        gameObject.SetActive(false);
        UpdateTexts(0, showPlus: false);
    }

    // Called after first sign is READ (dialogue closed)
    public void ShowObjectiveToast()
    {
        gameObject.SetActive(true);
        UpdateTexts(0, showPlus: false);
        PlayToast();
    }

    // Called when collecting pieces (well)
    public void AddPieces(int amount)
    {
        int before = CurrentPieces;
        CurrentPieces = Mathf.Clamp(CurrentPieces + amount, 0, totalPieces);
        int gained = Mathf.Max(0, CurrentPieces - before);

        gameObject.SetActive(true);
        UpdateTexts(gained, showPlus: gained > 0);
        PlayToast();
    }

    private void UpdateTexts(int gainedAmount, bool showPlus)
    {
        if (objectiveText != null)
            objectiveText.text = "Objective: Find mask pieces";

        if (counterText != null)
        {
            if (showPlus && gainedAmount > 0)
                counterText.text = $"+{gainedAmount}   ({CurrentPieces}/{totalPieces})";
            else
                counterText.text = $"({CurrentPieces}/{totalPieces})";
        }
    }

    private void PlayToast()
    {
        if (pointA == null || pointB == null || pointC == null)
        {
            Debug.LogWarning("ObjectiveUI missing PointA/PointB/PointC references.");
            return;
        }

        if (animRoutine != null)
            StopCoroutine(animRoutine);

        animRoutine = StartCoroutine(SlideAtoBThenC());
    }

    private IEnumerator SlideAtoBThenC()
    {
        rect.anchoredPosition = pointA.anchoredPosition;

        yield return Slide(pointA.anchoredPosition, pointB.anchoredPosition, slideInTime);

        yield return WaitUnscaled(stayTime);

        yield return Slide(pointB.anchoredPosition, pointC.anchoredPosition, slideOutTime);

        gameObject.SetActive(false);
        animRoutine = null;
    }

    private IEnumerator Slide(Vector2 from, Vector2 to, float duration)
    {
        if (duration <= 0f)
        {
            rect.anchoredPosition = to;
            yield break;
        }

        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float p = Mathf.Clamp01(t / duration);
            rect.anchoredPosition = Vector2.Lerp(from, to, p);
            yield return null;
        }

        rect.anchoredPosition = to;
    }

    private IEnumerator WaitUnscaled(float seconds)
    {
        float t = 0f;
        while (t < seconds)
        {
            t += Time.unscaledDeltaTime;
            yield return null;
        }
    }
}
