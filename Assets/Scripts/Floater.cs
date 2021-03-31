using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floater : MonoBehaviour
{
    [SerializeField]
    private float amplitude = 0.5f;
    [SerializeField]
    private float frequency = 1f;

    Vector2 posOffset = new Vector2();
    Vector2 tempPos = new Vector2();
    // Start is called before the first frame update
    void Start()
    {
        posOffset = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //float up and down
        tempPos = posOffset;
        tempPos.y += Mathf.Sin(Time.fixedTime * Mathf.PI * frequency) * amplitude;
        transform.position = tempPos;
    }
}
