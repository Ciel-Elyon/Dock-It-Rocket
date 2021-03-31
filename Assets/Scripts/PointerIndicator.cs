using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointerIndicator : MonoBehaviour
{
    public float amplitude = 0.5f;
    public float frequency = 1f;

    float tempAlpha;

    private SpriteRenderer ren;

    void Start()
    {
        tempAlpha = 255f;
        ren = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        StartCoroutine(Indicate());

        tempAlpha = Mathf.Sin(Time.fixedTime * Mathf.PI * frequency) * amplitude + 0.5f;

        Color temp = ren.color;
        temp.a = tempAlpha;
        ren.color = temp;
    }


    IEnumerator Indicate()
    {
        yield return new WaitForSeconds(5f);
        Destroy(this.gameObject);
    }
}
