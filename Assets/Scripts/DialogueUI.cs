using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueUI : MonoBehaviour
{
    public static DialogueUI Instance;

    public GameObject dialoguePanel;
    public TMP_Text dialogueText1;
    public TMP_Text dialogueText2;

    private bool isOpen;
    private PlayerMovement2D lockedPlayer;

    // ✅ stores the frame we closed on
    private int lastCloseFrame = -999;

    void Awake()
    {
        Instance = this;
        Close();
    }

    void Update()
    {
        // E closes dialogue while open
        if (isOpen && Input.GetKeyDown(KeyCode.E))
        {
            Close();
        }
    }

    public void Open(string msg1, string msg2, PlayerMovement2D playerToLock)
    {
        if (isOpen) return;

        lockedPlayer = playerToLock;
        if (lockedPlayer != null)
            lockedPlayer.inputEnabled = false; // only affects movement script

        dialoguePanel.SetActive(true);
        if (dialogueText1 != null) dialogueText1.text = msg1;
        if (dialogueText2 != null) dialogueText2.text = msg2;

        isOpen = true;
    }

    public void Close()
    {
        dialoguePanel.SetActive(false);
        isOpen = false;

        if (lockedPlayer != null)
            lockedPlayer.inputEnabled = true;

        lockedPlayer = null;

        // ✅ mark close frame to stop instant reopen
        lastCloseFrame = Time.frameCount;
    }

    public bool IsOpen() => isOpen;

    // ✅ InteractableTrigger uses this to avoid reopening on same key press/frame
    public bool JustClosedThisFrame()
    {
        return Time.frameCount == lastCloseFrame;
    }
}
