using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// Stores and manages the player's current respawn point.

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance;

    public Vector3 RespawnPoint { get; private set; }

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

    public void SetCheckpoint(Vector3 newCheckpoint)
    {
        RespawnPoint = newCheckpoint;
    }
}

