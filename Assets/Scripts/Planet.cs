using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    GameObject ship;
    Rigidbody2D shipRb;

    [SerializeField] float maxDist;
    [SerializeField] [Range(1, 100)] float forceMultiplier;

    const float G_CONST = 6674f;

    // Start is called before the first frame update
    void Start()
    {
        ship = FindObjectOfType<Player>().gameObject;
        shipRb = ship.GetComponent<Rigidbody2D>();

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //If within certain distance, apply gravitational force on rocket
        if (Mathf.Abs((ship.transform.position - transform.position).magnitude) <= maxDist)
        {
            Vector3 direction = transform.position - ship.transform.position;
            float distance = direction.magnitude;
            float forceMag = forceMultiplier / Mathf.Pow(distance, 2);
            Vector3 force = direction.normalized * forceMag;
            shipRb.AddForce(force);
        }
    }
}
