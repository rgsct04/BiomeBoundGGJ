using UnityEngine;
using TMPro;

public class DialogueUI : MonoBehaviour
{
    public static DialogueUI Instance { get; private set; }

    [Header("UI")]
    public GameObject dialoguePanel;
    public TMP_Text dialogueText1;
    public TMP_Text dialogueText2;

    [Header("Optional Overlay (above dialogue box)")]
    public GameObject overlayObject; // egg/mask sprite UI object

    [Header("Input")]
    public KeyCode closeKey = KeyCode.E;

    private bool isOpen;
    private PlayerMovement2D lockedPlayer;
    private int lastCloseFrame = -999;
    private int openFrame = -999; // ✅ NEW

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        Close(); // start hidden
    }

    void Update()
    {
        if (isOpen && Time.frameCount != openFrame && Input.GetKeyDown(KeyCode.E))
            Close();
    }


    public void Open(string msg1, string msg2, PlayerMovement2D playerToLock)
    {
        Open(msg1, msg2, playerToLock, false);
    }

    public void Open(string msg1, string msg2, PlayerMovement2D playerToLock, bool overlayOn)
    {
        if (isOpen) return;

        openFrame = Time.frameCount; // ✅ NEW

        lockedPlayer = playerToLock;
        if (lockedPlayer != null)
            lockedPlayer.inputEnabled = false;

        if (dialoguePanel != null) dialoguePanel.SetActive(true);

        if (dialogueText1 != null) dialogueText1.text = msg1;
        if (dialogueText2 != null) dialogueText2.text = msg2;

        if (overlayObject != null) overlayObject.SetActive(overlayOn);

        isOpen = true;
    }

    public void Close()
    {
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
        if (overlayObject != null) overlayObject.SetActive(false);

        isOpen = false;

        if (lockedPlayer != null)
            lockedPlayer.inputEnabled = true;

        lockedPlayer = null;
        lastCloseFrame = Time.frameCount;
    }

    public bool IsOpen() => isOpen;

    public bool JustClosedThisFrame()
    {
        return Time.frameCount == lastCloseFrame;
    }
}
