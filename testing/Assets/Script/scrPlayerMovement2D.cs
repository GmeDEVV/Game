using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//NOTES:
// Please review the code and let me know if this is any better than the last one; especially rows 45-73.
// The code works fine with 'slow' and 'fast' jumps
// Maybe there's a better way to do it in C#, idk :(

public class scrPlayerMovement2D : MonoBehaviour
{
    Rigidbody2D rb;

    public float speed;
    public float jumpForce;

    public float x = 0f;
    public float moveBy = 0f;

    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;
    public float runSpeed = 1f;

    bool isGrounded = false;
    bool isRunning = false;
    public Transform isGroundedChecker;
    public float checkGroundRadius;
    public LayerMask groundLayer;

    public float rememberGroundedFor;
    float lastTimeGrounded;

    public int defaultAdditionalJumps = 1;
    int additionalJumps;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        additionalJumps = defaultAdditionalJumps;
    }


    // Update is called once per frame
    void Update()
    {
        // Toggle running by pressing Shift
        // works only while walking on the ground
        if (Input.GetKeyDown(KeyCode.LeftShift) && isRunning == false && isGrounded)
        {
            runSpeed = 1.5f;
            isRunning = true;
        }
        else if (Input.GetKeyDown(KeyCode.LeftShift) && isRunning == true && isGrounded)
        {
            runSpeed = 1f;
            isRunning = false;
        }


        // movement
        x = Input.GetAxisRaw("Horizontal");

        moveBy = x * speed * runSpeed;

        rb.velocity = new Vector2(moveBy, rb.velocity.y);


        // Restet running state if no there's no input from the player
        // I wanted to stop players from running when they stop moving
        if (moveBy == 0 && !Input.anyKey)
        {
            runSpeed = 1f;
            isRunning = false;
        }


        // Jumping
        if (Input.GetKeyDown(KeyCode.Space) && (isGrounded || Time.time - lastTimeGrounded <= rememberGroundedFor || additionalJumps > 0))
        {
            rb.velocity = new Vector2(rb.velocity.x * runSpeed, jumpForce);
            additionalJumps--;
        }

        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            rb.velocity += Vector2.up * Physics2D.gravity * (lowJumpMultiplier - 1) * Time.deltaTime;
        }


        // Check if the player is on the groud ground and can jump
        Collider2D colliders = Physics2D.OverlapCircle(isGroundedChecker.position, checkGroundRadius, groundLayer);

        if (colliders != null)
        {
            isGrounded = true;
            additionalJumps = defaultAdditionalJumps;
        }
        else
        {
            if (isGrounded)
            {
                lastTimeGrounded = Time.time;
            }
            isGrounded = true;
        }
    }
}
