using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripting;

public class ShadowScript : MonoBehaviour
{

    private PlayerControls playerScript;
    private GameObject player;
    public Material ShadowMaterial;
    private Rigidbody2D rb;
    private string Slider = "Vector1_27b92db214274837916893e1929c6284";
    public Vector3 ShadowScale;

    public float movementSpeed;
    private float movementSpeed_X;
    private Vector2 movement;
    private float jumpHorizontalSpeed;

    private float lerpT = 0.2f;
    public Status correntStatus = Status.Dead;
    private bool isAttacking = false;
    private bool isJumping = false;


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        playerScript = player.GetComponent<PlayerControls>();
        rb = GetComponent<Rigidbody2D>();
        ShadowMaterial = GetComponent<Renderer>().material;
        ShadowScale = GameObject.Find("Player").transform.localScale;
        ShadowMaterial.SetFloat(Slider, 0);
    }

    // Update is called once per frame
    void Update()
    {
        switch (correntStatus)
        {
            case Status.Active:
                movementSpeed_X = Input.GetAxis("Horizontal");
                if (movementSpeed_X < 0)
                {
                    transform.localScale = new Vector3(-3, 3, 3);
                }
                if (movementSpeed_X > 0)
                {
                    transform.localScale = new Vector3(3, 3, 3);
                }
                //anim.SetFloat("speed", Mathf.Abs(movementSpeed_X));
                break;

            case Status.Summon:
                Prepare();
                break;


            case Status.Dead:
                if (ShadowMaterial.GetFloat(Slider) > 0)
                    DestroyShadow();
                break;
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

    private void Prepare()
    {
        //transform.position = OrbPos.position;
        ShadowMaterial.SetFloat(Slider, Mathf.Lerp(ShadowMaterial.GetFloat(Slider), 0.83f, lerpT));
        lerpT += 0.03f * Time.deltaTime;
        if (ShadowMaterial.GetFloat(Slider) >= 0.815f)
        {
            correntStatus = Status.Active;
            playerScript.correntStatus = playerScript.correntStatus.changeStatus();
            lerpT = 0.8f * Time.deltaTime;
        }
    }
    private void DestroyShadow()
    {
        ShadowMaterial.SetFloat(Slider, Mathf.Lerp(ShadowMaterial.GetFloat(Slider), 0f, lerpT));
        lerpT += 0.04f * Time.deltaTime;
        if (ShadowMaterial.GetFloat(Slider) <= 0.1f)
        {
            ShadowMaterial.SetFloat(Slider, 0);
            lerpT = 0;
        }
    }
}
