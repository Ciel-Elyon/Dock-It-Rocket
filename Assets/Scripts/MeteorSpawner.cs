using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorSpawner : MonoBehaviour
{
    private float meteorOffsetY = 6f;
    private const float meteorDelay = 2f;
    private Quaternion quat;

    private Vector3 pos;
    private Transform playerPos;
    [SerializeField] private GameObject meteorPrefab;

    // Blink
    private SpriteRenderer sprite;
    private const float blinkFrequency = 0.3f;
    private float blinkTimer = blinkFrequency;

    // Start is called before the first frame update
    void Start()
    {
        playerPos = GameObject.Find("Rocket").GetComponent<Transform>();
        sprite = GetComponent<SpriteRenderer>();

        pos = transform.position;
        meteorOffsetY = Camera.main.ViewportToWorldPoint(new Vector3(1f, 1f)).y - playerPos.position.y - 1f;
        quat = transform.rotation;
        transform.rotation = Quaternion.identity;

        StartCoroutine(StageMeteor());
    }

    // Update is called once per frame
    private void Update()
    {
        // Follow player position
        pos = transform.position;
        pos.y = playerPos.position.y + meteorOffsetY;
        transform.position = pos;

        // Blink
        blinkTimer -= Time.deltaTime;
        if (blinkTimer <= 0f)
        {
            sprite.enabled = !sprite.enabled;
            blinkTimer = blinkFrequency;
        }
    }

    private IEnumerator StageMeteor()
    {
        yield return new WaitForSeconds(meteorDelay);

        GameObject meteor = GameObject.Instantiate(meteorPrefab, transform.position, quat);
        Destroy(gameObject);
    }
}
