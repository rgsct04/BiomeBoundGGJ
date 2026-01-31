using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InteractableTrigger : MonoBehaviour
{
    [Header("Prompt UI (World Space)")]
    public GameObject promptObject;

    [Header("Dialogue (Two Lines)")]
    [TextArea(1, 2)] public string message1 = "Find the pieces, rebuild whats broken... release the spirits, escape the biomes.";
    [TextArea(2, 6)] public string message2 = "Press e to continue...";

    private bool playerInRange;
    private PlayerMovement2D playerMovement;

    void Start()
    {
        if (promptObject != null)
            promptObject.SetActive(false);
    }

    void Update()
    {
        if (!playerInRange) return;
        if (DialogueUI.Instance == null) return;

        // If dialogue is open, don't open again
        if (DialogueUI.Instance.IsOpen())
            return;

        // ✅ If dialogue just closed this frame, don't reopen on the same E press
        if (DialogueUI.Instance.JustClosedThisFrame())
            return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (promptObject != null)
                promptObject.SetActive(false);

            DialogueUI.Instance.Open(message1, message2, playerMovement);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = true;
        playerMovement = other.GetComponent<PlayerMovement2D>();

        if (promptObject != null && (DialogueUI.Instance == null || !DialogueUI.Instance.IsOpen()))
            promptObject.SetActive(true);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = false;
        playerMovement = null;

        if (promptObject != null)
            promptObject.SetActive(false);
    }
}
