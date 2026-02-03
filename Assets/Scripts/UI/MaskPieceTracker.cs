using System.Collections;
using UnityEngine;
using TMPro;

public class MaskPieceTracker : MonoBehaviour
{
    public static MaskPieceTracker Instance { get; private set; }

    [Header("Progress")]
    public int totalPieces = 3;
    public int CurrentPieces { get; private set; }

    [Header("Counter Toast UI")]
    public RectTransform counterPanel;   // CounterToast (the moving panel)
    public TMP_Text counterText;         // TMP inside CounterToast

    [Header("Counter Toast Points (A->B->C)")]
    public RectTransform pointA;
    public RectTransform pointB;
    public RectTransform pointC;

    [Header("Timing")]
    public float slideInTime = 0.25f;
    public float stayTime = 1.5f;
    public float slideOutTime = 0.25f;

    private Coroutine toastRoutine;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Start hidden
        if (counterPanel != null)
        {
            if (pointC != null) counterPanel.anchoredPosition = pointC.anchoredPosition;
            counterPanel.gameObject.SetActive(false);
        }

        UpdateText(plusAmount: 0, showPlus: false);
    }

    public void AddPiece(int amount = 1)
    {
        int before = CurrentPieces;
        CurrentPieces = Mathf.Clamp(CurrentPieces + amount, 0, totalPieces);
        int gained = Mathf.Max(0, CurrentPieces - before);

        // Update text
        UpdateText(gained, showPlus: gained > 0);

        // Play toast animation (only if we actually gained something)
        if (gained > 0)
            PlayToast();
    }

    private void UpdateText(int plusAmount, bool showPlus)
    {
        if (counterText == null) return;

        if (showPlus && plusAmount > 0)
            counterText.text = $" + {plusAmount}  Mask Fragments Found: {CurrentPieces}/{totalPieces}";
        else
            counterText.text = $"Fragments: {CurrentPieces}/{totalPieces}";
    }

    private void PlayToast()
    {
        if (counterPanel == null || pointA == null || pointB == null || pointC == null)
        {
            Debug.LogWarning("MaskPieceTracker toast missing counterPanel or A/B/C points.");
            return;
        }

        counterPanel.gameObject.SetActive(true);

        if (toastRoutine != null)
            StopCoroutine(toastRoutine);

        toastRoutine = StartCoroutine(ToastRoutine());
    }

    private IEnumerator ToastRoutine()
    {
        // Start at A
        counterPanel.anchoredPosition = pointA.anchoredPosition;

        // A -> B
        yield return Slide(pointA.anchoredPosition, pointB.anchoredPosition, slideInTime);

        // Wait
        yield return WaitUnscaled(stayTime);

        // B -> C
        yield return Slide(pointB.anchoredPosition, pointC.anchoredPosition, slideOutTime);

        counterPanel.gameObject.SetActive(false);
        toastRoutine = null;
    }

    private IEnumerator Slide(Vector2 from, Vector2 to, float duration)
    {
        if (duration <= 0f)
        {
            counterPanel.anchoredPosition = to;
            yield break;
        }

        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float p = Mathf.Clamp01(t / duration);
            counterPanel.anchoredPosition = Vector2.Lerp(from, to, p);
            yield return null;
        }

        counterPanel.anchoredPosition = to;
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
