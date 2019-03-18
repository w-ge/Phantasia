using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody2D rb;
    int jumpCapacity;
    int jumpForce;
    float jumpTimer;
    int jumpForceAir;
    
    public float wallSlideSpeedMax = 3;
    public bool wallSliding;
    public Transform wallCheckPoint;
    public bool wallCheck;
    public LayerMask wallLayerMask;
    public bool grounded;
    public bool leftCollision;
    public bool rightCollision;
    public Vector2 wallJumpOff;

    const float skinWidth = 0.30f;
    public int horizontalRayCount = 4;
    public int verticalRayCount = 4;
    float horizontalRaySpacing;
    float verticalRaySpacing;
    RaycastOrigins raycastOrigins;
    BoxCollider2D collider;
    public LayerMask collisionMask;
    public CollisionInfo collisions;

    Vector2 input;

    private SpriteRenderer mySpriteRenderer;
    private Animator animator;
    public Sprite jumpUp;
    public Sprite jumpDown;
    public Sprite slide;

    public float maxMoveSpeed;
    public float velocityX;

    public int wallJumpBufferTime;
    int wallJumpFrameCounter = 0;

    public bool hooked;
    public float swingForce;

    // ------------------------------------------------------------------------

    void CalculateRaySpacing() {
        Bounds bounds = collider.bounds;    
        bounds.Expand(skinWidth * -2);

        horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
        verticalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);

        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
    }

    void UpdateRaycastOrigins() {
        Bounds bounds = collider.bounds;
        bounds.Expand(skinWidth * -2);

        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
    }

    void HorizontalCollisions() {
        float directionX = collisions.faceDir;
        float rayLength = Mathf.Abs(rb.velocity.x) + skinWidth;

        if (Mathf.Abs(rb.velocity.x) < skinWidth) {
            rayLength = 2 * skinWidth;
        }

        for (int i = 0; i < horizontalRayCount; i++) {
            Vector2 raycastOrigin;
            if (directionX == -1) {
                raycastOrigin = raycastOrigins.bottomLeft;
            }
            else {
                raycastOrigin = raycastOrigins.bottomRight;
            }

            raycastOrigin += Vector2.up * (horizontalRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(raycastOrigin, Vector2.right * directionX, rayLength, collisionMask);

            Debug.DrawRay(raycastOrigin, Vector2.right * directionX * rayLength, Color.red);
        
            if (hit) {
                rayLength = hit.distance;

                if (collisions.faceDir > 0)
                {
                    Debug.Log("Right");
                    collisions.right = true;
                }
                else if (collisions.faceDir < 0)
                {
                    collisions.left = true;
                }
            }
        }
    }

    void WallCollisionDetection() 
    {
        wallCheck = Physics2D.OverlapCircle(wallCheckPoint.position, 0.5f, wallLayerMask);
        if (wallCheck == true)
        {
            if (collisions.right) {
                rightCollision = true;
                leftCollision = false;
            }
            else if (collisions.left) {
                leftCollision = true;
                rightCollision = false;
            }
        }
        else {
            leftCollision = false;
            rightCollision = false;
        }
    }
    
    void JumpFunction()
    {
        int wallDirX;
        if (leftCollision) {
            wallDirX = -1;
        }
        else {
            wallDirX = 1;
        }

        //Wall Jump
        if (wallCheck || wallJumpBufferTime > 0) {
            if (Input.GetButtonDown("Jump")) {

                    Debug.Log("Wall Jump Off");
                    velocityX = wallJumpOff.x;
                    rb.velocity = new Vector2(-wallDirX * velocityX, wallJumpOff.y);
                
                wallJumpFrameCounter = 30;
            }

            if (wallJumpBufferTime > 0) {
                wallJumpBufferTime--;
            }
        }

        //Regular Jump
        else {
            if (Input.GetButtonDown("Jump") && jumpCapacity != 0) {
                rb.velocity =  new Vector2(rb.velocity.x, 0);
                rb.AddForce(new Vector2(0, jumpForce));
                jumpCapacity--;
                jumpTimer = 30;
                grounded = false;
            }

            if (rb.velocity.y < 0.2)
            {
                rb.gravityScale = 5;
            }
            
            else if (rb.velocity.y > 0)
            {
                rb.gravityScale = 3;
            } 

            if (Input.GetKey(KeyCode.Space) && jumpTimer > 0) {
                rb.AddForce(new Vector2(0, jumpForceAir));
                jumpTimer--;
            }
        }
    }

    void MovementFunction()
    {
        if (hooked)
        {
            rb.AddForce(new Vector2(Input.GetAxisRaw("Horizontal") * swingForce, 0));
            
        }
        else if(grounded){
            Debug.Log("nu");
            velocityX = Input.GetAxisRaw("Horizontal") * maxMoveSpeed;
            rb.velocity = new Vector2(velocityX, rb.velocity.y);
        }
        else 
        {
            if (Mathf.Abs(rb.velocity.x) < maxMoveSpeed)
            {
                Debug.Log("adding movement");
                Debug.Log(Input.GetAxisRaw("Horizontal") * maxMoveSpeed / 10);
                rb.AddForce(new Vector2(40 * Input.GetAxisRaw("Horizontal"), 0));
            }

            else if (Mathf.Sign(rb.velocity.x) != Mathf.Sign(input.x))
            {
                rb.AddForce(new Vector2(40 * Input.GetAxisRaw("Horizontal"), 0));
            }

            else 
            {
                Debug.Log("Max Move");
                new Vector2(Input.GetAxisRaw("Horizontal") * maxMoveSpeed, rb.velocity.y);
            }
        } 
    }

    void HandleWallSliding()
    {
        wallSliding = false;
        if ((leftCollision || rightCollision) && !grounded && rb.velocity.y < 0) {
            wallSliding = true;
            wallJumpBufferTime = 30;

            if (rb.velocity.y < -wallSlideSpeedMax && wallJumpFrameCounter == 0) {
                rb.velocity = new Vector2(rb.velocity.x, -wallSlideSpeedMax);
            }

            if (wallJumpFrameCounter > 0)
            {
                wallJumpFrameCounter--;
            }
        }
    }
    
    void OnCollisionEnter2D(Collision2D col){
         if (col.gameObject.tag == "Platform") {
             jumpCapacity = 2;
             grounded = true;
         }
    }

    // Start is called before the first frame update
    void Start()
    {
        collider = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        jumpForce = 500;
        jumpForceAir = 10;
        CalculateRaySpacing();
        collisions.faceDir = 1;
        maxMoveSpeed = 10;
        wallJumpOff = new Vector2(18f, 10.5f);
        hooked = false;
        swingForce = 20.0f;
        mySpriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        collisions.Reset();

        UpdateRaycastOrigins();
        HorizontalCollisions();

        if (collisions.faceDir > 0)
        {
            mySpriteRenderer.flipX = true;
        }
        else
        {
            mySpriteRenderer.flipX = false;
        }

        if (input.x != 0)
        {
            collisions.faceDir = (int)input.x;
        }

        WallCollisionDetection();
        HandleWallSliding();
        MovementFunction();
        JumpFunction();

        // Animations!!!
        /*if (!grounded && rb.velocity.y >= 0)
        {
            mySpriteRenderer.sprite = jumpUp;
            animator.SetBool("Movement", false);
        }
        else if (!grounded)
        {
            mySpriteRenderer.sprite = jumpDown;
            animator.SetBool("Movement", false);
            Debug.Log(mySpriteRenderer.sprite.name);
        }
        else */
        
        animator.SetFloat("Movement", Math.Abs(rb.velocity.x));
        if (!grounded)
        {
            if(jumpCapacity == 0 || jumpCapacity == 2)
            {
                animator.SetBool("Jump", true);
                animator.SetBool("JumpFirst", false);
            }
            else
            {
                animator.SetBool("JumpFirst", true);
                animator.SetBool("Jump", false);
            }
            
        }
        else
        {
            animator.SetBool("Jump", false);
            animator.SetBool("JumpFirst", false);
        }
        

    }
    struct RaycastOrigins {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }

    public struct CollisionInfo {
        public bool above, below;
        public bool left, right;
        public int faceDir;

        public void Reset() {
            above = below = false;
            left = right = false;
        }
    }
}
