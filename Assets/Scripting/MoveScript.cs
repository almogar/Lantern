using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveScript : MonoBehaviour
{
    Vector2 startPosition;
    float startZ;
    public float speed;

    Vector2 travel => (Vector2)Camera.main.transform.position - startPosition;
    // Start is called before the first frame update
    Vector2 parallaxFactor;

    void Start()
    {
        startPosition = transform.position;
        startZ = transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = startPosition + travel * speed;
        
    }
}
