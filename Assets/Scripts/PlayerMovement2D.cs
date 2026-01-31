using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement2D : MonoBehaviour
{
    [Header("Speeds")]
    public float walkSpeed = 5f;
    public float runSpeed = 8f;

    [Header("Turn Settings")]
    public float turnDuration = 0.2f;   // match Idle_Turn / Walk_Turn / Run_Turn length
    public float moveThreshold = 0.1f;  // MUST match your Animator MoveX threshold (0.1)

    [Header("Ground Check (for Grounded bool)")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Jump")]
    public float jumpForce = 12f;

    [Header("Input")]
    public float inputDeadzone = 0.01f;

    [Header("Physics Material (No Stick)")]
    [Tooltip("Assign a PhysicsMaterial2D with Friction=0 and Bounciness=0 (e.g., 'NoFriction').")]
    public PhysicsMaterial2D noFrictionMaterial;

    // ✅ ADDED: interaction lock (set false when reading dialogue)
    [Header("Interaction Lock")]
    public bool inputEnabled = true;

    private Rigidbody2D rb;
    private Animator anim;
    private BoxCollider2D boxCol;

    private bool facingRight = true;
    private bool turnInProgress = false;
    private bool grounded = false;

    // prevents jump spam: can only jump once until landing
    private bool canJump = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCol = GetComponent<BoxCollider2D>();

        rb.freezeRotation = true;

        // Apply no-friction material to the BoxCollider2D to prevent sticking to walls
        if (boxCol != null && noFrictionMaterial != null)
        {
            boxCol.sharedMaterial = noFrictionMaterial;
        }
        else
        {
            if (boxCol == null)
                Debug.LogWarning("PlayerMovement2D: No BoxCollider2D found on this GameObject.");
            if (noFrictionMaterial == null)
                Debug.LogWarning("PlayerMovement2D: No 'noFrictionMaterial' assigned. Player may stick to walls.");
        }

        anim.SetBool("FacingRight", facingRight);
        anim.SetBool("IsTurning", false);
        anim.SetBool("IsRunning", false);
        anim.SetBool("IsFalling", false);

        ApplyFacingScale();
    }

    void Update()
    {
        // Always update grounded even if input is locked (so Jump/Fall animations behave)
        grounded = false;
        if (groundCheck != null)
        {
            grounded = Physics2D.OverlapCircle(
                groundCheck.position,
                groundCheckRadius,
                groundLayer
            );
        }
        anim.SetBool("Grounded", grounded);

        // If input is disabled (dialogue etc), stop locomotion params + ignore controls
        if (!inputEnabled)
        {
            anim.SetFloat("MoveX", 0f);
            anim.SetBool("IsRunning", false);
            anim.SetBool("IsFalling", !grounded && rb.velocity.y < -0.1f);
            return;
        }

        float horizontal = Input.GetAxisRaw("Horizontal");
        float moveX = Mathf.Abs(horizontal);

        // reset jump when we are grounded (landed)
        if (grounded)
        {
            canJump = true;
        }

        // SPACE = JUMP (once per landing)
        if (grounded && canJump && Input.GetKeyDown(KeyCode.Space))
        {
            DoJump();
            canJump = false;
        }

        // Falling animation
        anim.SetBool("IsFalling", !grounded && rb.velocity.y < -0.1f);

        // Running
        bool runHeld = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        bool isRunningNow = runHeld && moveX > moveThreshold;

        anim.SetFloat("MoveX", moveX);
        anim.SetBool("IsRunning", isRunningNow);

        // Turn logic
        if (!turnInProgress && moveX > inputDeadzone)
        {
            if (horizontal > 0 && !facingRight)
                StartTurn(true);
            else if (horizontal < 0 && facingRight)
                StartTurn(false);
        }
    }

    void FixedUpdate()
    {
        // If input is locked, don't accept movement input. Stop horizontal movement.
        if (!inputEnabled)
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
            return;
        }

        float horizontal = Input.GetAxisRaw("Horizontal");
        float moveX = Mathf.Abs(horizontal);

        bool runHeld = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        float speed = (runHeld && moveX > moveThreshold) ? runSpeed : walkSpeed;

        // No slide when grounded (don't snap in-air)
        if (grounded && Mathf.Abs(horizontal) <= inputDeadzone)
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
            return;
        }

        rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
    }

    void DoJump()
    {
        // consistent jump height
        rb.velocity = new Vector2(rb.velocity.x, 0f);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        // Animator trigger (make sure you have a Trigger param named "Jump")
        anim.SetTrigger("Jump");
    }

    void StartTurn(bool newFacingRight)
    {
        if (turnInProgress) return;

        facingRight = newFacingRight;
        ApplyFacingScale();

        anim.SetBool("FacingRight", facingRight);
        anim.SetBool("IsTurning", true);

        StartCoroutine(ResetTurn());
    }

    IEnumerator ResetTurn()
    {
        turnInProgress = true;
        yield return new WaitForSeconds(turnDuration);

        anim.SetBool("IsTurning", false);
        turnInProgress = false;
    }

    void ApplyFacingScale()
    {
        Vector3 s = transform.localScale;
        float absX = Mathf.Abs(s.x);
        if (absX < 0.0001f) absX = 1f;

        s.x = absX * (facingRight ? 1f : -1f);
        transform.localScale = s;
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
