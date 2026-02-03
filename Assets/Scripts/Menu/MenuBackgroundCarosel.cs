using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// Scrolls multiple full-width backgrounds horizontally in sequence.
/// When a background exits left, it is moved to the far right.
/// Perfect for menu biome carousels.

public class MenuBackgroundCarousel : MonoBehaviour
{
    public Transform[] backgrounds;
    public float scrollSpeed = 1f;
    public float backgroundWidth = 20f;

    void Update()
    {
        foreach (Transform bg in backgrounds)
        {
            bg.position += Vector3.left * scrollSpeed * Time.deltaTime;

            // If fully off screen to the left
            if (bg.position.x <= -backgroundWidth)
            {
                MoveToRight(bg);
            }
        }
    }

    void MoveToRight(Transform bg)
    {
        float rightMostX = GetRightMostX();
        bg.position = new Vector3(
            rightMostX + backgroundWidth,
            bg.position.y,
            bg.position.z
        );
    }

    float GetRightMostX()
    {
        float maxX = backgrounds[0].position.x;

        foreach (Transform bg in backgrounds)
        {
            if (bg.position.x > maxX)
                maxX = bg.position.x;
        }

        return maxX;
    }
}
