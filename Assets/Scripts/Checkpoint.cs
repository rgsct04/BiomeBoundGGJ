using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Sets the player's respawn point when touched.
/// </summary>
public class Checkpoint : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            CheckpointManager.Instance.SetCheckpoint(transform.position);
        }
    }
}
