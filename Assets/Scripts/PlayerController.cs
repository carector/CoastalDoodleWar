using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Player mechanics:
// - Move
// - Jump
// - Grab objects from out of ground
//   - Throw held objects
//   - Set down objects if not moving
// - Climb up ladders
// - Ride on enemies
// - Ride on projectiles (egg)

public class PlayerController : MonoBehaviour
{
    // Public vars
    public float maxWalkSpeed;
    public float maxRunSpeed;
    public float acceleration;
    public float jumpForce;
    public bool isGrounded;

    public bool isPullingPlant;
    public bool isHoldingPlant;
    public bool canMove;
        
    public GameObject heldObj;
    public Vector3 startPos;

    public Plant currentPlant;
    Rigidbody2D rb;
    SpriteRenderer spr;
    SpriteRenderer torsoSpr;
    Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spr = GetComponent<SpriteRenderer>();
        torsoSpr = GameObject.Find("UpperHalfSprite").GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // Check to see if we're grounded
        GroundCheck();

        // Also check for plants
        Plantcast();

        if (!isPullingPlant && canMove)
        {

            if (Input.GetAxis("Horizontal") != 0)
            {
                // FLip our sprite depending on which direction we're facing
                if (Input.GetAxis("Horizontal") > 0)
                {
                    spr.flipX = true;
                    torsoSpr.flipX = true;
                }
                else
                {
                    spr.flipX = false;
                    torsoSpr.flipX = false;
                }

                if (Input.GetKey(KeyCode.LeftShift))
                {
                    // Move at sprint spead if we're holding x
                    Move(Input.GetAxis("Horizontal"), true);
                    SetAnimation(2);
                }
                else
                {
                    // Walk normally otherwise
                    Move(Input.GetAxis("Horizontal"), false);
                    SetAnimation(1);
                }
            }
            else
            {
                Move(0, false);

                if (Mathf.Abs(rb.velocity.x) <= 0.05f)
                    SetAnimation(0);
            }

            if (Input.GetKeyDown(KeyCode.Z) && isGrounded)
            {
                Jump();
            }

            // If we press X when we are standing next to a plant, call PullPlant()
            if (Input.GetKeyDown(KeyCode.X) && currentPlant != null && isGrounded && !isHoldingPlant && Mathf.Abs(Input.GetAxis("Horizontal")) <= 0.2)
            {
                StartCoroutine(PullPlant());
                isPullingPlant = true;
            }
            else if (Input.GetKeyDown(KeyCode.X) && isHoldingPlant)
            {
                StartCoroutine(ThrowPlant());
            }
        }

        if (!isGrounded && rb.velocity.y < 0)
        {
            SetAnimation(5);
        }
    }


    // Moves the player (uses RigidBody)
    void Move(float direction, bool isRunning)
    {
        float maxSpeed;

        if (isRunning)
        {
            maxSpeed = maxRunSpeed;
        }
        else
        {
            maxSpeed = maxWalkSpeed;
        }

        if (direction != 0)
        {
            /*if (Mathf.Abs(rb.velocity.x) < maxSpeed)
            {
                rb.velocity = Vector3.right * direction * 10 * acceleration;
            }
            else
            {*/
                rb.velocity = new Vector2(maxSpeed * direction, rb.velocity.y);
            //}
        }
        else
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
        }
    }

    void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce);
        SetAnimation(6);
        isGrounded = false;
    }

    IEnumerator ThrowPlant()
    {
        Rigidbody2D hrb = heldObj.GetComponent<Rigidbody2D>();
        hrb.bodyType = RigidbodyType2D.Dynamic;

        int dir = 1;

        if (!spr.flipX)
        {
            dir = -1;
        }

        if (Input.GetAxis("Horizontal") == 0)
        {
            hrb.transform.position = new Vector3(hrb.transform.position.x + 1 * dir, hrb.transform.position.y, hrb.transform.position.z);
            //hrb.AddForce(new Vector2(dir * 600 * hrb.mass + rb.velocity.x, 100 * hrb.mass + rb.velocity.y));
            
        }
        else
        {
            if (Input.GetKey(KeyCode.UpArrow))
            {
                hrb.AddForce(new Vector2(dir * 250 * hrb.mass + rb.velocity.x, 450 * hrb.mass + rb.velocity.y));
            }
            else
            {
                hrb.AddForce(new Vector2(dir * 700 * hrb.mass + rb.velocity.x, 300 * hrb.mass + rb.velocity.y));
            }
        }
        

        yield return new WaitForSeconds(0.1f);

        heldObj.transform.parent = null;
        heldObj.GetComponent<BoxCollider2D>().isTrigger = false;
        torsoSpr.enabled = false;

        heldObj = null;
        isHoldingPlant = false;
    }

    IEnumerator PullPlant()
    {

        print("Pulling plant");

        SetAnimation(3);

        yield return new WaitForSeconds(0.2f);

        while (Mathf.Abs(transform.position.x - currentPlant.transform.position.x) >= 0.1f)
        {
            rb.velocity = Vector3.Lerp(rb.velocity, new Vector3(0, rb.velocity.y, 0), 1);
            transform.position = Vector3.Lerp(transform.position, new Vector3(currentPlant.transform.position.x, transform.position.y, transform.position.z), 0.75f);
            yield return null;
        }

        transform.position = new Vector3(currentPlant.transform.position.x, transform.position.y, transform.position.z);

        yield return new WaitForSeconds(0.2f);

        SetAnimation(4);

        heldObj = Instantiate(currentPlant.objectToPull, new Vector3(currentPlant.transform.position.x, currentPlant.transform.position.y - 1, 0), Quaternion.identity);
        heldObj.transform.parent = transform;

        Rigidbody2D hrb = heldObj.GetComponent<Rigidbody2D>();
        hrb.bodyType = RigidbodyType2D.Kinematic;
        heldObj.GetComponent<BoxCollider2D>().isTrigger = true;

        Destroy(currentPlant.gameObject);

        print(Mathf.Abs(heldObj.transform.position.y - (transform.position.y + 1.5f)));

        while (Mathf.Abs(heldObj.transform.position.y - (transform.position.y + 1.5f)) >= 0.05f)
        {
            heldObj.transform.position = Vector2.Lerp(heldObj.transform.position, new Vector2(transform.position.x, transform.position.y + 1.5f), 0.5f);
            yield return null;
        }

        heldObj.transform.position = new Vector3(transform.position.x, transform.position.y + 1.5f, 0);
        yield return new WaitForSeconds(0.2f);
        torsoSpr.enabled = true;

        anim.SetTrigger("ReturnToIdle");
        isPullingPlant = false;
        isHoldingPlant = true;
    }

    void GroundCheck()
    {
        startPos = new Vector3(transform.position.x, transform.position.y - 1.05f);
        RaycastHit2D hit = Physics2D.BoxCast(startPos, new Vector2(1, 0.01f), 0, Vector2.down, 0.05f);
        
        Debug.DrawRay(startPos, Vector3.down * 0.05f, Color.red);

        // Perform raycast if our velocity is less than zero (can't be grounded if we're moving up)
        if (rb.velocity.y <= 0)
        {
            if (hit.collider != null)
            {
                isGrounded = true;
                anim.SetTrigger("ReturnToIdle");
                anim.SetBool("Fall", false);
                anim.SetBool("Jump", false);

            }
            else
            {
                isGrounded = false;
            }
        }
    }

    // Sets the animation to be a specific clip
    void SetAnimation(int animNumber)
    {
        if (animNumber == 0)
        {
            anim.SetBool("Walk", false);
            anim.SetBool("Run", false);
        }
        else if (animNumber == 1)
        {
            anim.SetBool("Walk", true);
            anim.SetBool("Run", false);
        }
        else if (animNumber == 2)
        {
            anim.SetBool("Run", true);
            anim.SetBool("Walk", false);
            //anim.SetTrigger("Jump");
        }
        else if (animNumber == 3)
        {
            anim.SetBool("Walk", false);
            anim.SetBool("Run", false);
            anim.SetTrigger("PullIdle");
        }
        else if (animNumber == 4)
        {
            anim.SetTrigger("Pull");
        }
        else if (animNumber == 5)
        {
            anim.SetBool("Fall", true);
        }
        else if (animNumber == 6)
        {
            anim.SetBool("Jump", true);
        }
    }

    void Plantcast()
    {
        Vector3 plantStartPos = new Vector3(transform.position.x, transform.position.y);

        Debug.DrawRay(plantStartPos, Vector3.down * 1.1f, Color.green);
        RaycastHit2D hit = Physics2D.BoxCast(plantStartPos, new Vector2(1, 0.1f), 0, Vector2.down);

        if (hit.collider != null && hit.collider.tag == "Plant")
        {
            print("hit plant");
            currentPlant = hit.transform.GetComponent<Plant>();
        }
        else if (!isPullingPlant)
        {
            currentPlant = null;
        }
    }
}

