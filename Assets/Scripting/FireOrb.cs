using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireOrb : MonoBehaviour
{
    //private GameObject player;
    public UnityEngine.Experimental.Rendering.Universal.Light2D fireLight;
    public Transform OrbPos;
    private Vector3 FireScale;
    public bool isThorw = false;
    public bool needDestroy = false;
    [SerializeField]
    private float flameThrowForce;
    
    void Start()
    {
        //player = GameObject.Find("Player");
    }

    
    void Update()
    {
        if (name.Contains("Clone") && !isThorw && !needDestroy)
        {
            Prepare();
        }
        if(name.Contains("Clone") && needDestroy)
        {
            destroyFireOrb();
        }
    }
    void Awake()
    {
        string name = this.name;
        if(name.Contains("Clone"))
        {
            FireScale = transform.localScale;
            transform.localScale = Vector3.zero;
            fireLight = GetComponentInChildren<UnityEngine.Experimental.Rendering.Universal.Light2D>();
            fireLight.intensity = 0;
            gameObject.SetActive(true);
            gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        }
    }
    private void Prepare()
    {
        transform.position = OrbPos.position;
        transform.localScale = Vector3.Lerp(transform.localScale, FireScale, 2f * Time.deltaTime);
        fireLight.intensity = Mathf.Lerp(fireLight.intensity, 1f, 1f * Time.deltaTime);
    }
    public bool tryToThrow(Vector2 flameDirection)
    {
        
        if (transform.localScale.x < FireScale.x - 0.3f)
        {
            return false;
        }
        float angle = Mathf.Atan2(flameDirection.x, flameDirection.y) * Mathf.Rad2Deg;
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        GetComponent<Rigidbody2D>().AddForce(flameDirection.normalized * flameThrowForce);
        isThorw = true;
        return true;
    }
    public void destroyFireOrb()
    {
        if (!isThorw)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, 1f * Time.deltaTime);
            fireLight.intensity = Mathf.Lerp(fireLight.intensity, 0f, 1f * Time.deltaTime);
            if (transform.localScale.x - 0.3f <= 0)
                Destroy(gameObject);
        }
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (gameObject.name == "FireOrb(Clone)")
        {
            Destroy(gameObject);
        }
    }
}
