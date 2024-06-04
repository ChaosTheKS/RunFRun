using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Jobs;
using UnityEngine.UIElements;

public class ParallaxBackground : MonoBehaviour
{
    private GameObject cam;

    [SerializeField] private float ParallaxEffect;

    private float length;
    private float xPosition;

    
    void Start()
    {
        cam = GameObject.Find("Main Camera");

        length = GetComponent<SpriteRenderer>().bounds.size.x;
        xPosition = transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        float DistanceMoved = cam.transform.position.x * (1 - ParallaxEffect);
        float DistanceToMove = cam.transform.position.x * ParallaxEffect;

        transform.position = new Vector3(xPosition + DistanceToMove, transform.position.y);

        if (DistanceMoved > xPosition + length)
        {
            xPosition = xPosition + length;
        }
    }
}
