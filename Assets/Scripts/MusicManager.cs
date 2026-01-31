using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Handles background music persistence across scenes.

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    public AudioClip backgroundMusic;
    public float volume = 0.5f;

    private AudioSource audioSource;

    void Awake()
    {
        // Singleton enforcement
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = backgroundMusic;
            audioSource.loop = true;
            audioSource.volume = volume;
            audioSource.playOnAwake = false;

            audioSource.Play();
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
