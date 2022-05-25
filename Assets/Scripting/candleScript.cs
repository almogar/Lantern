using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class candleScript : MonoBehaviour
{
    private Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        anim.Play(0, -1, Random.value);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
