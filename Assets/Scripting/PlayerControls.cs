using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripting;


public class PlayerControls : MonoBehaviour
{

    // movement
    public Status correntStatus = Status.Active;
    public float movementSpeed;
    private float movementSpeed_X;
    private Vector2 movement;
    [SerializeField] private LayerMask platfromLayerMask;
    public bool isAttacking;
    private bool lampGrounded = false;
    public bool isGrounded;

    //Lamp
    //------------------------------------------
    public GameObject lampOBJ;
    public Transform lampDropLocation;
    public UnityEngine.Experimental.Rendering.Universal.Light2D lampLight;
    private Vector3 dropLampPOS;
    private bool isLampAboutToUp = false;

    //Fireorb
    [SerializeField]
    private float lampVectorRange;
    private Vector3 lamp_mouseStartPos;
    private Vector3 lamp_updateMousePos;
    //------------------------------------------

    // Shadow 
    public GameObject Shadow;
    private ShadowScript ShadowScript;
    // --------------------------------

    //Mouse System
    //------------------------------------------
    private Vector2 mousePos1;
    private Vector2 mousePos2;
    public float mouseRange;
    private bool readyToAttack = false;
    private int attackDir;
    //private GameObject AttackCircle;
    //private Ray ray;
    //private RaycastHit hit;

    

    //Sounds
    //------------------------------------------------
    private AudioSource playerAudio;
    [SerializeField]
    private AudioClip[] stepsAudio;
    [SerializeField]
    private AudioClip[] jumpAudio;
    [SerializeField]
    private AudioClip[] attackSounds;

    //FOOTSTEPS
    //get random footstep sound and play it once
    private void footsteps()
    {
        playerAudio.clip = getRandomStep();
        playerAudio.PlayOneShot(playerAudio.clip);
    }
    //clip random
    private AudioClip getRandomStep()
    {
        return stepsAudio[Random.Range(0, stepsAudio.Length)];
    }

    //JUMP SOUNDS
    private void jumpSound(int i)
    {
        switch (i)
        {
            case 0:
                playerAudio.clip = jumpAudio[0];
                playerAudio.PlayOneShot(playerAudio.clip);
                break;

            case 1:
                playerAudio.clip = jumpAudio[1];
                playerAudio.PlayOneShot(playerAudio.clip);
                break;
        }
    }

    //Attack Sounds
    private void attackSound()
    {
        readyToAttack = false;
        playerAudio.clip = getRandomAttack();
        playerAudio.PlayOneShot(playerAudio.clip);
    }
    private AudioClip getRandomAttack()
    {
        return attackSounds[Random.Range(0, attackSounds.Length)];
    }

    //Animations
    //------------------------------------------------
    public Animator anim;
    private Rigidbody2D rb;
    private BoxCollider2D playerCollider;

    [SerializeField]
    private float jumpForce;
    private float jumpHorizontalSpeed;
    public bool isJumping = false;
   

    //--------------------------------------------------

    // Start is called before the first frame update
    void Start()
    {
        playerCollider = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        playerAudio = GetComponent<AudioSource>();
        ShadowScript = Shadow.GetComponent<ShadowScript>();
        //lampLight = GetComponentInChildren<UnityEngine.Experimental.Rendering.Universal.Light2D>();
        
    }

    // Update is called once per frame
    void Update()
    {
        checkIfGrounded();

        // Movement
        //----------------------------------------------------------------
        //Horizontal
        if (correntStatus == Status.Active)
        {
            //if the player hits the ground - gives velocity in X axis according to input and gives direction
            if (isGrounded)
                movementSpeed_X = Input.GetAxis("Horizontal");
            if (movementSpeed_X < 0 && !isAttacking && !isJumping)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
            if (movementSpeed_X > 0 && !isAttacking && !isJumping)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
            anim.SetFloat("speed", Mathf.Abs(movementSpeed_X));

            //Jump
            //------------------------------------------------------------------
            if (Input.GetKeyDown("w") && !isAttacking && isGrounded)
            {
                Jump();
            }
            if (isJumping)
            {
                anim.SetFloat("VerticalSpeed", rb.velocity.y);
            }


            // Attack
            //------------------------------------------------------------------
            // Close Range
            if (Input.GetMouseButtonDown(0) && isGrounded && !isJumping && !isAttacking)
            {
                startAttack();
            }
            if (Input.GetMouseButton(0) && !isJumping)
            {
                readyAttack();
            }
            if (Input.GetMouseButtonUp(0) && !isJumping)
            {
                Attack();
            }
        }

        if(!isJumping && rb.velocity.y < -0.1f)
        {
            anim.Play("Player_JumpHang");
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            correntStatus = correntStatus.changeStatus();
            ShadowScript.correntStatus = ShadowScript.correntStatus.changeStatus();
        }

        if (Input.GetMouseButtonDown(1))
        {
            lamp_mouseStartPos = Input.mousePosition;
        }

        if (Input.GetMouseButton(1))
        {
            lamp_updateMousePos = Input.mousePosition;
            //DropLamp
            if (isGrounded && lamp_mouseStartPos.y - lamp_updateMousePos.y >= lampVectorRange && Mathf.Abs(lamp_mouseStartPos.x - lamp_updateMousePos.x) <= 50)
            {
                anim.Play("Player_DropLamp");
                lampGrounded = true;
                anim.SetBool("LampGrounded", true);
            }
            
            else
            {
                if (!isLampAboutToUp && isGrounded && lamp_updateMousePos.y - lamp_mouseStartPos.y >= lampVectorRange && Mathf.Abs(lamp_mouseStartPos.x - lamp_updateMousePos.x) <= 50)
                {
                    BackToLampCode();
                }
                else if(ShadowScript.correntStatus.Equals(Status.Dead))
                    ShadowScript.correntStatus = Status.Summon;
            }
        }

        if (Input.GetMouseButtonUp(1))
        {
            if (ShadowScript.correntStatus.Equals(Status.Summon))
            {
                ShadowScript.correntStatus = Status.Dead;
            }
        }
        
    }
    private void FixedUpdate()
    {
        //if player is not jumping he moves according to horizontal input, if he is jumping he used the x velocity when jumped
        if (correntStatus == Status.Active)
        {
            if (!isJumping)
                movement = new Vector2(movementSpeed_X * movementSpeed, rb.velocity.y);
            else
                rb.velocity = new Vector2(jumpHorizontalSpeed, rb.velocity.y);

            if (!isAttacking && !isJumping)
                rb.velocity = movement;
        }
        if (correntStatus == Status.Waiting)
        {
            rb.velocity = new Vector2(0, 0);
        }
        
    }

    // ------------------------------------------------


    ////////////// Jump /////////////
    private void Jump()
    {
        //saves spped when keydown adn gives velocity to Y axis
        // anim.Play("Player_Jump");
        if (!lampGrounded)
        {
            anim.Play("Player_Jump_01");
        }
        else
        {
            anim.Play("Player_Jump_NoLamp");
        }
        isJumping = true;
        movementSpeed = 2;
        jumpHorizontalSpeed = rb.velocity.x;
    }
    public void jumpAction()
    {
        movementSpeed = 5;
        //Debug.Log(rb.velocity);
        isJumping = true;
        // mx = rb.velocity.x;
        rb.velocity = Vector2.up * jumpForce;
        anim.SetFloat("VerticalSpeed", rb.velocity.y);
    }
    private void Jumpland()
    {
        isJumping = false;
    }
    //checks if the player is hitting the ground with Raycast2d

    // general
    private void checkIfGrounded()
    {
        //isJumping = false;
        float range = 0.05f;
        Collider2D groundRay = Physics2D.BoxCast(playerCollider.bounds.center, playerCollider.bounds.size, 0f, Vector2.down, range, platfromLayerMask).collider;
        Color rayColor;
        isGrounded = false;

        if (groundRay != null)
        {
            isGrounded = groundRay.name == "GroundCave";
            isJumping = groundRay.name != "GroundCave";
            rayColor = Color.green;
        }
        else
            rayColor = Color.red;
        anim.SetBool("isGrounded", isGrounded);

        Debug.DrawRay(playerCollider.bounds.center + new Vector3(playerCollider.bounds.extents.x, 0f), Vector2.down * range, rayColor);
        Debug.DrawRay(playerCollider.bounds.center - new Vector3(playerCollider.bounds.extents.x, 0f), Vector2.down * range, rayColor);
        //Debug.DrawRay(playerCollider.bounds.center - new Vector3(playerCollider.bounds.extents.x, playerCollider.bounds.extents.y + range), Vector2.right * range, rayColor);
    }
    private Vector2 getMousePosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.nearClipPlane;
        return Camera.main.ScreenToWorldPoint(mousePos);

    }
    
    ///////////// Attack ////////////////
    private void startAttack()
    {
        mousePos1 = Input.mousePosition;
        anim.SetBool("attackCanceld", false);
    }
    private void readyAttack()
    {
        mousePos2 = Input.mousePosition;
        RaycastHit2D hit = Physics2D.Raycast(mousePos1, mousePos2);

        //checks direction of player
        bool readyLeft = (transform.localScale.x < 0 && mousePos2.x > mousePos1.x + mouseRange);
        bool readyRight = (transform.localScale.x >= 0 && mousePos2.x < mousePos1.x - mouseRange);
        //Attack to Left
        if (!readyToAttack)
        {
            if (readyLeft || readyRight)
            {
                rb.velocity = new Vector2(rb.velocity.x / 3f, rb.velocity.y);
                isAttacking = true;
                anim.Play("Player_AttackReady");
                readyToAttack = true;
                attackDir = readyLeft ? -1 : 1;
            }
        }
        else
        {
            if((attackDir == -1 && mousePos2.x < mousePos1.x + (mouseRange * 0.8f)) || (attackDir == 1 && mousePos2.x > mousePos1.x + (mouseRange * 0.8f)))
            {
                anim.Play("Player_AttackSwing");
                attackDir = 0;
            }
        }
        

        //if (transform.localScale.x < 0)
        //{
        //    if (!readyToAttack)
        //    {
        //        if (mousePos2.x > mousePos1.x + mouseRange)
        //        {
        //            rb.velocity = new Vector2(rb.velocity.x / 3f, rb.velocity.y);
        //            isAttacking = true;
        //            anim.Play("Player_AttackReady");
        //            readyToAttack = true;
        //        }
        //    }
        //    else
        //    {
        //        if (mousePos2.x < mousePos1.x + (mouseRange * 0.6f))
        //        {
        //            anim.Play("Player_AttackSwing");
        //            //attackSound();
        //        }
        //    }
        //}
        //else //Attack to right
        //{
        //    if (!readyToAttack)
        //    {
        //        if (mousePos2.x > mousePos1.x + mouseRange)
        //        {
        //            rb.velocity = new Vector2(rb.velocity.x / 3f, rb.velocity.y);
        //            isAttacking = true;
        //            anim.Play("Player_AttackReady");
        //            readyToAttack = true;
        //        }
        //    }
        //    else
        //    {
        //        if (mousePos2.x < mousePos1.x + (mouseRange * 0.8f))
        //        {
        //            anim.Play("Player_AttackSwing");
        //            //attackSound();
        //        }
        //    }
        //}
    }
    private void Attack()
    {
        if (readyToAttack)
        {
            anim.SetBool("attackCanceld", true);
            readyToAttack = false;
            isAttacking = false; ///////// i change
        }
        //AttackCircle.SetActive(false);
        //isAttacking = false;
    }
    public void doneAttack()
    {
        Debug.Log("ggg");
        readyToAttack = false;
        isAttacking = false;
    }

    ////////////// Lamp //////////////////////
    private void dropLamp()
    {
        dropLampPOS = transform.position;
        lampOBJ.SetActive(true);
        lampLight.gameObject.SetActive(false);
        lampOBJ.transform.position = lampDropLocation.position;
    }
    private void backToLamp()
    {
        lampOBJ.SetActive(false);
        lampLight.gameObject.SetActive(true);
        anim.Play("Player_BackToLamp");
        transform.position = dropLampPOS;
    }
    public void BackToLampCode()
    {
        correntStatus = Status.Active;
        ShadowScript.correntStatus = Status.Dead;
        backToLamp();
        anim.SetBool("LampGrounded", false);
        isLampAboutToUp = true;
    }

    
    
}
