using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alien : MonoBehaviour
{

    [SerializeField]
    [Range(0, 1)]
    private float fuelChance = .55f;

    [SerializeField]
    [Range(0, 1)]
    private float blackholeChance = 1f;

    [SerializeField]
    [Range(0, 1)]
    private float whiteholeChance = 0.75f;

    [Space]

    [SerializeField]
    GameObject fuelTankPrefab;

    [SerializeField]
    GameObject blackHolePrefab;

    [SerializeField]
    GameObject whiteHolePrefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Rocket"))
        {
            GameObject planet = transform.parent.gameObject;
            float scale = planet.transform.localScale.x;

            float spawnroll = Random.value;

            //spawn new things

            if (spawnroll <= fuelChance)
            {
                //spawn fuel tanks
                Instantiate(fuelTankPrefab, planet.transform.position + new Vector3(0.5f, 2.25f, 0f) * scale * scale, Quaternion.identity);

                Destroy(gameObject);
            }
            else if (spawnroll <= blackholeChance)
            {
                //spawn blackhole
                Instantiate(blackHolePrefab, transform.parent.position, Quaternion.identity);

                Destroy(transform.parent.gameObject);
            }
            else
            {
                //spawn whitehole
                Instantiate(whiteHolePrefab, transform.parent.position, Quaternion.identity);

                Destroy(transform.parent.gameObject);
            }
        }
    }
}
