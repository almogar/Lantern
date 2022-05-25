using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SkullGuideLightScript : MonoBehaviour
{
    public GameObject lightSource;

    [SerializeField]
    private SpriteRenderer shadowsSprite;

    [SerializeField]
    private Transform light;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        float pos =  light.position.x;



        
    }
}
