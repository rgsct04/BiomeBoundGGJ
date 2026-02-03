using UnityEngine;
using TMPro;

public class InteractableTrigger : MonoBehaviour
{
    [Header("Prompt (OPTIONAL)")]
    [Tooltip("Optional: assign an existing prompt GameObject. Leave empty to auto-create TMP prompt.")]
    public GameObject promptObject;

    [Header("Auto Prompt Settings (used only if promptObject is empty)")]
    public string promptMessage = "E to Interact";
    public Vector3 promptOffset = new Vector3(0f, 1.2f, 0f);
    public int promptFontSize = 4;
    public Color promptColor = Color.white;
    public string promptSortingLayer = "UI";
    public int promptOrderInLayer = 50;

    [Header("Mask Piece Reward (optional)")]
    public bool givesMaskPiece = false;
    public int maskPiecesToGive = 1;
    public bool onlyGiveOnce = true;

    [Header("Objective Reveal (optional)")]
    [Tooltip("Use this on your FIRST sign. It will reveal the objective UI AFTER the player closes the dialogue.")]
    public bool revealObjectiveAfterThisDialogueCloses = false;
    public bool revealOnlyOnce = true;

    [Header("Dialogue (Two Lines - editable per object)")]
    [TextArea(1, 6)] public string message1 = "Title / Speaker";
    [TextArea(2, 10)] public string message2 = "Body text goes here...\n\nPress E to continue...";

    [Header("Overlay Above Dialogue (optional)")]
    [Tooltip("If true, DialogueUI will show its overlayObject while this dialogue is open.")]
    public bool showOverlayAboveDialogue = false;

    [Header("Input")]
    public KeyCode interactKey = KeyCode.E;

    [Header("Debug")]
    [SerializeField] private bool debugPlayerInRange = false;

    private bool playerInRange;
    private PlayerMovement2D playerMovement;

    private TextMeshPro autoPromptTMP;
    private Collider2D myCol;

    // Mask piece state
    private bool alreadyGiven = false;

    // Objective reveal state
    private bool waitingToRevealObjective = false;
    private bool objectiveAlreadyRevealed = false;

    void Awake()
    {
        myCol = GetComponent<Collider2D>();
        if (myCol == null)
            myCol = GetComponentInChildren<Collider2D>();

        if (myCol == null)
        {
            Debug.LogError($"InteractableTrigger on '{name}' needs a Collider2D (or a child with one).");
            enabled = false;
            return;
        }

        myCol.isTrigger = true;

        if (promptObject == null)
        {
            GameObject go = new GameObject("AutoPromptText");
            go.transform.SetParent(transform);
            go.transform.localPosition = promptOffset;

            autoPromptTMP = go.AddComponent<TextMeshPro>();
            autoPromptTMP.text = promptMessage;
            autoPromptTMP.fontSize = promptFontSize;
            autoPromptTMP.color = promptColor;
            autoPromptTMP.alignment = TextAlignmentOptions.Center;
            autoPromptTMP.gameObject.SetActive(false);

            var r = autoPromptTMP.GetComponent<MeshRenderer>();
            r.sortingLayerName = promptSortingLayer;
            r.sortingOrder = promptOrderInLayer;
        }
        else
        {
            promptObject.SetActive(false);
        }
    }

    void Update()
    {
        debugPlayerInRange = playerInRange;
        if (!playerInRange) return;

        if (DialogueUI.Instance == null)
        {
            Debug.LogWarning($"[{name}] No DialogueUI.Instance found in scene.");
            return;
        }

        bool dialogueOpen = DialogueUI.Instance.IsOpen();

        // ✅ If we opened dialogue earlier and we're waiting for it to close, reveal objective AFTER closing.
        if (waitingToRevealObjective && !dialogueOpen)
        {
            waitingToRevealObjective = false;
            objectiveAlreadyRevealed = true;

            // Show objective UI (choose ONE system below)

            // Option A: using ObjectiveUI (recommended)
            if (ObjectiveUI.Instance != null)
                ObjectiveUI.Instance.ShowObjectiveToast();
            else
                Debug.LogWarning("ObjectiveUI.Instance not found in scene (ObjectiveUI missing?)");

            // Option B: if you prefer to keep MaskPieceTracker as your objective UI, uncomment:
            // if (MaskPieceTracker.Instance != null) MaskPieceTracker.Instance.ShowCounter();
        }

        // Prompt only when in range AND dialogue not open
        SetPromptVisible(!dialogueOpen);

        // Don't re-open while open
        if (dialogueOpen) return;

        // Prevent instant reopen on the same frame it closed
        if (DialogueUI.Instance.JustClosedThisFrame()) return;

        if (Input.GetKeyDown(interactKey))
        {
            SetPromptVisible(false);

            // Open dialogue
            DialogueUI.Instance.Open(message1, message2, playerMovement, showOverlayAboveDialogue);

            // ✅ Arm the objective reveal ONLY when we actually opened the dialogue
            if (revealObjectiveAfterThisDialogueCloses && (!revealOnlyOnce || !objectiveAlreadyRevealed))
            {
                waitingToRevealObjective = true;
            }

            // Give mask piece reward (only once if set)
            if (givesMaskPiece && (!onlyGiveOnce || !alreadyGiven))
            {
                if (MaskPieceTracker.Instance != null)
                {
                    MaskPieceTracker.Instance.AddPiece(maskPiecesToGive);
                    alreadyGiven = true;
                }
                else
                {
                    Debug.LogWarning("No MaskPieceTracker.Instance found in scene.");
                }
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = true;
        playerMovement = other.GetComponent<PlayerMovement2D>();

        Debug.Log($"Player entered interact range of '{name}'");

        if (DialogueUI.Instance == null || !DialogueUI.Instance.IsOpen())
            SetPromptVisible(true);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = false;
        playerMovement = null;

        Debug.Log($"Player left interact range of '{name}'");

        SetPromptVisible(false);
    }

    void SetPromptVisible(bool visible)
    {
        if (promptObject != null)
        {
            if (promptObject.activeSelf != visible)
                promptObject.SetActive(visible);
        }
        else if (autoPromptTMP != null)
        {
            if (autoPromptTMP.gameObject.activeSelf != visible)
                autoPromptTMP.gameObject.SetActive(visible);
        }
    }
}
