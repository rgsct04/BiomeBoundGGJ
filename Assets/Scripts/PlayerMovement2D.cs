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

    private Rigidbody2D rb;
    private Animator anim;

    // We flip using SCALE to avoid animations overriding SpriteRenderer.flipX
    private bool facingRight = true;
    private bool turnInProgress = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        anim.SetBool("FacingRight", facingRight);
        anim.SetBool("IsTurning", false);
        anim.SetBool("IsRunning", false);

        ApplyFacingScale();
    }

    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float moveX = Mathf.Abs(horizontal);

        // Grounded (Animator param)
        bool grounded = false;
        if (groundCheck != null)
            grounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        anim.SetBool("Grounded", grounded);

        // Running (Animator param)
        bool runHeld = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        bool isRunningNow = runHeld && moveX > moveThreshold;

        anim.SetFloat("MoveX", moveX);
        anim.SetBool("IsRunning", isRunningNow);

        // Turn ONLY when direction flips and there's actual input
        if (!turnInProgress && moveX > 0.01f)
        {
            if (horizontal > 0 && !facingRight)
                StartTurn(true);
            else if (horizontal < 0 && facingRight)
                StartTurn(false);
        }
    }

    void FixedUpdate()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float moveX = Mathf.Abs(horizontal);

        bool runHeld = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        float speed = (runHeld && moveX > moveThreshold) ? runSpeed : walkSpeed;

        rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
    }

    void StartTurn(bool newFacingRight)
    {
        if (turnInProgress) return;

        facingRight = newFacingRight;

        // Flip visually (scale-based)
        ApplyFacingScale();

        // Optional Animator param (bool)
        anim.SetBool("FacingRight", facingRight);

        // Enter Idle_Turn / Walk_Turn / Run_Turn based on your Animator conditions
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

    // Flips the character by scaling X (+ for right, - for left)
    // More reliable than SpriteRenderer.flipX if animations are fighting you.
    void ApplyFacingScale()
    {
        Vector3 s = transform.localScale;
        float absX = Mathf.Abs(s.x);

        if (absX < 0.0001f) absX = 1f; // safety if scale was 0

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