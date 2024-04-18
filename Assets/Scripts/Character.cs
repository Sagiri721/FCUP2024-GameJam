using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public Stats playerStats;

    [SerializeField] private Transform groundCheck, wallCheck;

    private Rigidbody2D rb;
    private BoxCollider2D groundCollider, wallCollider;
    private Animator animator;

    private float jumpTimeCounter, 
        coyoteTimeCounter, 
        jumpBufferCounter = 0;

    private Vector2 movement;

    // State booleans
    private bool grounded = false, 
        wallSliding = false,
        crouched = false, 
        jumping = false, 
        isFacingRight = true,
        running = false;

    private bool isWalljumping;
    private float walljumpingDir, walljumpingCounter;

    private Vector2 respawnPoint;

    void Start()
    {
        respawnPoint = transform.position;

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        groundCollider = groundCheck.gameObject.GetComponent<BoxCollider2D>();
        wallCollider = wallCheck.gameObject.GetComponent<BoxCollider2D>();

        animator.SetBool("walk", false);
        animator.SetBool("run", false);
    }
    
    void Update()
    {
        // Get frame constants
        grounded = isGrounded();
        wallSliding = isOnWall();

        crouched = movement.x == 0 && Input.GetAxisRaw("Vertical") < 0;
        running = Utils.GetKeyAll(playerStats.runKeys);

        handleMovement();
        handleAnimationState();
        handleJump();
        handleFlip();

        rb.velocity = movement;
        movement = new Vector2();

        // Respawn code
        if (transform.position.y < -10) {

            System.Action respawnCallback = respawn;

            Transition manager = Transition.getInstance();
            StartCoroutine(manager.DoTransition(respawn));
        } 
    }

    // Animation    
    private void handleAnimationState(){

        animator.SetBool("walk", (movement.x != 0));
        animator.SetBool("run", running && movement.x != 0);
        animator.SetBool("jump", !grounded);
        animator.SetBool("crouch", crouched);
    }

    private void handleMovement(){

        // Speed 
        float speed = (running ? playerStats.runSpeed : playerStats.walkSpeed);

        // Handle movement directions
        // When crouching wait until no longer crouched to move
        float direction = crouched ? 0 : Input.GetAxisRaw("Horizontal") * speed;

        movement = new Vector2(
            isWalljumping ? 0 : direction,
            rb.velocity.y
        );
    }

    private bool isGrounded() {
        return Physics2D.BoxCast(groundCheck.position, groundCollider.bounds.size, 0, Vector2.down, 1f, playerStats.groundLayer);;
    }

    private bool isOnWall() {
        return Physics2D.BoxCast(wallCheck.position, wallCollider.bounds.size, 0, Vector2.right * (isFacingRight ? 1 : -1), 1f, playerStats.groundLayer);;
    }

    private void handleFlip()
    {
        if(isWalljumping) return;

        if (isFacingRight && movement.x < 0f || !isFacingRight && movement.x > 0f)
        {
            isFacingRight = !isFacingRight;
            
            // Flip sprite
            Vector3 localScale = transform.localScale;
            localScale.x *= -1;
            transform.localScale = localScale;
        }
    }

    private void handleJump(){

        bool jumped = Utils.GetKeyDownAll(playerStats.jumpKeys);

        // Coyote time
        if (grounded) coyoteTimeCounter = playerStats.coyoteTime; 
        else coyoteTimeCounter -= Time.deltaTime;

        // Jump buffering
        if (jumped) jumpBufferCounter = playerStats.jumpBufferTime;
        else jumpBufferCounter -= Time.deltaTime;

        if (jumpBufferCounter > 0f && coyoteTimeCounter > 0f){

            if (grounded) jumping = true;
            jumpTimeCounter = playerStats.jumpTime;

            movement.y = playerStats.jumpPower;

            // Reset jump buffer
            jumpBufferCounter = 0;
        }

        if(Utils.GetKeyUpAll(playerStats.jumpKeys)){

            coyoteTimeCounter = 0;
            jumping = false;  
        } 

        if (Utils.GetKeyAll(playerStats.jumpKeys) && (rb.velocity.y > 0f)){

            if (jumpTimeCounter > 0 && jumping) {

                movement.y = playerStats.jumpPower;
                jumpTimeCounter -= Time.deltaTime;
                
            } else jumping = false;
        }

        // Apply gravity pull
        if(rb.velocity.y < 0f) {

            // Jump apex modifier
            float pull = playerStats.gravityPull * (rb.velocity.y < 5 ? playerStats.jumpApexModifier : 1);
            movement.y -= playerStats.gravityPull;
        }

        movement.y = Mathf.Clamp(movement.y, -playerStats.gravityClamp, playerStats.gravityClamp);

        // Wall jumping
        // handleWallJumping(jumped);
    }

    private void stopWalljump(){ isWalljumping = false; }

    private void handleWallJumping(bool jumped){

        // Handle wall sliding
        if(wallSliding && !grounded && rb.velocity.x != 0){

            // Apply the lesser velocity
            movement.y =  Mathf.Clamp(rb.velocity.y, -playerStats.wallSlidingSpeed, float.MaxValue);

            // Prepare wall jump
            isWalljumping = false;
            walljumpingDir = -Mathf.Sign(transform.localScale.x);
            walljumpingCounter = playerStats.wallJumpingTime;

            CancelInvoke(nameof(stopWalljump));

        } else { walljumpingCounter -= Time.deltaTime; }

        if(jumped && walljumpingCounter > 0f){

            // Perform wall jump
            isWalljumping = true;

            movement = new Vector2(
                playerStats.wallJumpingPower.x * walljumpingDir, 
                playerStats.wallJumpingPower.y
            );

            walljumpingCounter = 0;

            // Stop wall jump timer
            Invoke(nameof(stopWalljump), playerStats.wallJumpingDuration);
        }
    }

    void respawn(){
        transform.position = respawnPoint;
    }

}
