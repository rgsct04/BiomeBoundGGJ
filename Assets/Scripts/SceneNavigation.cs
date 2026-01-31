using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


// This Script Handles scene navigation for UI buttons.
// Allows loading scenes by name and returning to the previous scene.

public class SceneNavigation : MonoBehaviour
{
    // Stores the name of the previously loaded scene.
    // Static so it persists between scene loads.
    private static string previousSceneName;

    /// Loads a new scene and records the current scene as the previous one.
    /// This method is intended to be called by a UI Button (OnClick).

    /// <param name="sceneName">
    /// The exact name of the scene to load.
    /// Must match the scene name in Build Settings.
    /// </param>
    public void LoadScene(string sceneName)
    {
        // Store the currently active scene before changing scenes
        previousSceneName = SceneManager.GetActiveScene().name;

        // Load the requested scene
        SceneManager.LoadScene(sceneName);
    }

    /// Loads the previously visited scene.
    /// Intended for use with a "Back" button.
   
    public void LoadPreviousScene()
    {
        // Only attempt to load if a previous scene exists
        if (!string.IsNullOrEmpty(previousSceneName))
        {
            SceneManager.LoadScene(previousSceneName);
        }
        else
        {
            Debug.LogWarning("No previous scene recorded to return to.");
        }
    }
}
