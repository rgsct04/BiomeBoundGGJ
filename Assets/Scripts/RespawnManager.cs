using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles player death feedback and respawning.
/// </summary>
public class RespawnManager : MonoBehaviour
{
    public static RespawnManager Instance;

    [Header("UI")]
    public GameObject deathPanel;
    public float respawnDelay = 1f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RespawnPlayer(GameObject player)
    {
        StartCoroutine(RespawnRoutine(player));
    }

    private IEnumerator RespawnRoutine(GameObject player)
    {
        // Show feedback panel
        if (deathPanel != null)
            deathPanel.SetActive(true);

        yield return new WaitForSeconds(respawnDelay);

        // Move player to last checkpoint
        if (CheckpointManager.Instance != null)
        {
            player.transform.position = CheckpointManager.Instance.RespawnPoint;
        }

        // Hide panel
        if (deathPanel != null)
            deathPanel.SetActive(false);
    }
}
