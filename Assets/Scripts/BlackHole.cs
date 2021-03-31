using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHole : MonoBehaviour
{
    [SerializeField]
    AudioSource blackHoleSound;
    [SerializeField]
    private float AngularSpeed = 50;
    // Start is called before the first frame update
    void Start()
    {
        blackHoleSound = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, 0, AngularSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Rocket")
        {
            blackHoleSound.Play();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Rocket")
        {
            blackHoleSound.Stop();
        }
    }
}
