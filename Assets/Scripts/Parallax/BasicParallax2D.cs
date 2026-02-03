using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicParallax2D : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Usually your Main Camera transform. If left null, it will auto-find Camera.main.")]
    public Transform cam;

    [Header("Parallax")]
    [Tooltip("0 = no movement (super far away). 1 = same speed as camera (no parallax). Typical: 0.1 - 0.8")]
    [Range(0f, 1f)]
    public float parallaxStrength = 0.3f;

    [Header("Lock Axes")]
    public bool affectX = true;
    public bool affectY = false;

    [Header("Optional: Smoothing")]
    [Tooltip("0 = instant. Higher = smoother.")]
    public float smooth = 0f;

    private Vector3 lastCamPos;

    void Start()
    {
        if (cam == null)
        {
            if (Camera.main != null) cam = Camera.main.transform;
        }

        if (cam == null)
        {
            Debug.LogError("BasicParallax2D: No camera assigned and Camera.main not found.");
            enabled = false;
            return;
        }

        lastCamPos = cam.position;
    }

    void LateUpdate()
    {
        Vector3 camDelta = cam.position - lastCamPos;

        float dx = affectX ? camDelta.x * parallaxStrength : 0f;
        float dy = affectY ? camDelta.y * parallaxStrength : 0f;

        Vector3 targetPos = transform.position + new Vector3(dx, dy, 0f);

        if (smooth <= 0f)
            transform.position = targetPos;
        else
            transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * smooth);

        lastCamPos = cam.position;
    }
}
