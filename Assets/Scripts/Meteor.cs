using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteor : MonoBehaviour
{
    public Vector3 facing = Vector3.right;
    private const float speed = 8f;

    void Start()
    {
        facing = (transform.rotation.z > 0) ? transform.right * -1f : transform.right;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += facing * speed * Time.deltaTime;

        float viewportX = Camera.main.WorldToViewportPoint(transform.position).x;
        if (viewportX > 1.2f || viewportX < -0.2f)
        {
            Destroy(gameObject);
        }
    }
}
