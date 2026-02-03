using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Triggers player respawn without reloading the scene.
/// </summary>
public class KillZone : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            RespawnManager.Instance.RespawnPlayer(other.gameObject);
        }
    }
}


