using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Rendering;

public class GameManager : MonoBehaviour
{
    private Transform playerPos;
    [SerializeField] private GameObject planetPrefab;
    [SerializeField] private GameObject blackholePrefab;
    [SerializeField] private GameObject asteroidPrefab;
    [SerializeField] private GameObject meteorPrefab;
    [SerializeField] private GameObject fuelPrefab;
    [SerializeField] private GameObject whiteholePrefab;
    [SerializeField] private GameObject amazingPlanetPrefab;

    // For scoring
    [SerializeField] private TMP_Text scoreText;
    private int score;

    // For spawning planets and orbiting asteroids
    private const float spawnOffsetY = 12f;
    private const float spawnOffsetRangeY = 2f;
    private const float spawnOffsetRangeX = 6f;
    private const float spawnScaleRange = 0.2f;
    private float nextSpawnPointY = 0f;

    // For spawning blackholes instead of planets
    private const float blackholeSpawnChance = 0.25f;
    private bool prevHoleSpawn = false;

    private const float whileholeSpawnChance = 0.5f;
    private bool prevwhiteHoleSpawn = false;

    private const float amazingPlanetSpawnChance = 0.75f;
    private bool prevAmazingspawn = false;

    // For spawning meteors
    private float meteorSpawnInterval = 20f;
    private const float meteorSpawnDecrement = 2f;
    private const float meteorSpawnCap = 8f;
    private const float meteorAngleMin = 5f;
    private const float meteorAngleMax = 35f;
    private float nextMeteorSpawnY = 15f;

    private float screenWidthWorld = 0f;
    private float screenHeightWorld = 0f;

    // For managing sounds
    private AudioSource[] sounds;
    public enum SoundClip : int
    {
        Refill = 0,
        Explode = 1,
        Impact = 2,
        SoftLanding = 3,
        HardLanding = 4
    };

    private void Start()
    {
        playerPos = GameObject.Find("Rocket").GetComponent<Transform>();
        sounds = gameObject.GetComponents<AudioSource>();
        score = 0;

        Vector3 v = Camera.main.ViewportToWorldPoint(new Vector3(1f, 1f));
        screenWidthWorld = v.x - playerPos.position.x - 0.8f;
        screenHeightWorld = v.y - playerPos.position.y - 1f;
    }

    public void PlaySound(SoundClip clip)
    {
        sounds[(int)clip].Stop();
        sounds[(int)clip].Play();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (playerPos.position.y > nextSpawnPointY)
        {
            Vector3 spawnPos = Vector3.zero;
            spawnPos.x = Random.Range(-spawnOffsetRangeX, spawnOffsetRangeX);
            spawnPos.y = nextSpawnPointY + spawnOffsetY +
                Random.Range(-spawnOffsetRangeY, spawnOffsetRangeY);
            spawnPos.z = -1f;

            float spawnRoll = Random.value;
            if (spawnRoll < blackholeSpawnChance && !prevHoleSpawn)
            {
                // Spawn blackhole instead
                GameObject blackhole = GameObject.Instantiate(blackholePrefab, spawnPos, Quaternion.identity);
                blackhole.transform.localScale *= Random.Range(1f - spawnScaleRange, 1f + spawnScaleRange) + 0.2f;
                blackhole.transform.GetChild(0).transform.localScale = blackhole.transform.localScale; // center collider
                prevHoleSpawn = true;
            }
            else if(spawnRoll < whileholeSpawnChance && !prevwhiteHoleSpawn)
            {
                prevHoleSpawn = false;
                //Spawn whilehole instead
                GameObject whitehole = GameObject.Instantiate(whiteholePrefab, spawnPos, Quaternion.identity);
                whitehole.transform.localScale *= Random.Range(1f - spawnScaleRange, 1f + spawnScaleRange) + 0.2f;
                whitehole.transform.GetChild(0).transform.localScale = whitehole.transform.localScale;
                prevwhiteHoleSpawn = true;
            }
            else if(spawnRoll < amazingPlanetSpawnChance && !prevAmazingspawn)
            {
                prevHoleSpawn = false;
                prevwhiteHoleSpawn = false;

                GameObject planet = GameObject.Instantiate(amazingPlanetPrefab, spawnPos, Quaternion.identity);
                prevAmazingspawn = true;
                
            }
            else
            {
                // Spawn planet
                prevHoleSpawn = false;
                prevwhiteHoleSpawn = false;
                prevAmazingspawn = false;

                GameObject planet = GameObject.Instantiate(planetPrefab, spawnPos, Quaternion.identity);
                planet.transform.localScale *= Random.Range(1f - spawnScaleRange, 1f + spawnScaleRange);
                planet.transform.GetChild(0).transform.localScale = planet.transform.localScale; // Planet sprite
                planet.transform.GetChild(0).transform.localRotation = Quaternion.Euler(0f, 0f, Random.Range(-180f, 180f));
                planet.transform.GetChild(1).transform.localScale = planet.transform.localScale; // Platform sprite
                planet.transform.GetChild(1).transform.localPosition *= planet.transform.localScale.x;

                // Spawn Asteroids
                // Asteroid # thresholds based on planet size:
                // < 0.85f no asteroid; 0.85f - 0.9f 1 asteroid; 0.9f - 1f 2 asteroids; > 1f 3 asteroids
                int numAsteroids = 0;
                float scale = planet.transform.localScale.x;
                if (scale > 0.85f) numAsteroids++;
                if (scale > 0.9f) numAsteroids++;
                if (scale > 1f) numAsteroids++;

                for (int i = 0; i < numAsteroids; i++)
                {
                    GameObject asteroid = GameObject.Instantiate(asteroidPrefab, spawnPos, Quaternion.identity);
                    asteroid.GetComponent<AsteroidOrbit>().SetParentPlanet(
                    planet.transform.position, 1f, 2f * planet.transform.localScale.x,
                    360f * i / numAsteroids);
                }

                // Spawn fuel cans
                // The chance that a fuel can is spawned is positively related to planet size
                // Has 1/4 of this chance to spawn 2 cans instead
                float chance = (scale - 1f + spawnScaleRange) / (2f * spawnScaleRange);
                chance = chance / 2f + 0.4f;
                float chanceForTwo = chance / 4f;
                float roll = Random.value;
                if (roll < chanceForTwo)
                {
                    GameObject fuel1 = GameObject.Instantiate(fuelPrefab,
                        planet.transform.position + new Vector3(-0.5f, 2.25f, 0f) * scale * scale,
                        Quaternion.identity);
                    GameObject fuel2 = GameObject.Instantiate(fuelPrefab,
                        planet.transform.position + new Vector3(0.5f, 2.25f, 0f) * scale * scale,
                        Quaternion.identity);
                }
                else if (roll < chance)
                {
                    GameObject fuel = GameObject.Instantiate(fuelPrefab,
                        planet.transform.position + new Vector3(-0.3f, 2.25f, 0f) * scale * scale,
                        Quaternion.identity);
                }
            } // End spawn planet

            nextSpawnPointY += spawnOffsetY;
        } // End planet spawn check

        if (playerPos.position.y > nextMeteorSpawnY)
        {
            // Prepare to spawn meteor; code here should spawn a MeteorSpawner
            float spawnSide = (Random.value < 0.5f) ? -1f : 1f;
            float angle = Random.Range(meteorAngleMin, meteorAngleMax) * spawnSide;
            Vector3 spawnPos = new Vector3(screenWidthWorld * spawnSide, 
                playerPos.position.y + screenHeightWorld, -3f);
            Quaternion quat = Quaternion.Euler(0f, 0f, angle);

            GameObject.Instantiate(meteorPrefab, spawnPos, quat);

            nextMeteorSpawnY += meteorSpawnInterval;
        }


    }

    public void UpdateScore()
    {
        score++;
        scoreText.text = "Planets Docked: " + score.ToString();
    }

    public void IncreaseMeteorSpawnRate()
    {
        if (meteorSpawnInterval > meteorSpawnCap)
        {
            meteorSpawnInterval -= meteorSpawnDecrement;
        }
    }
}
