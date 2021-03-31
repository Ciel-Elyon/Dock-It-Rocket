using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [SerializeField] private Joystick joystick;
    private Rigidbody2D rb2d;
    private TrailRenderer tr;
    private AudioSource audioSource;
    [SerializeField] private AudioClip engineSpark;
    [SerializeField] private ParticleSystem smokeParticles;
    [SerializeField] private GameObject explosion; // 2d animation
    [SerializeField] private GameObject spriteObject; // Incl. actual sprite and trail renderer
    [SerializeField] private GameObject fuelCanvas;
    [SerializeField] private FuelBar fuelBar;

    [SerializeField] private float speed;
    [SerializeField] private float tiltSpeed;
    [SerializeField] private float maxFuel;
    [SerializeField] private float currentFuel;
    [SerializeField] private float fuelRate;
    private bool hasExploded = false;

    private float X_LIMIT;

    bool engineSparkPlayed = false;

    private GameManager gameManager;

    // For checking whether we can dock at a planet.
    // Simplest way: don't allow docking at a position lower than prev height
    float heightOfLastDocking = 0f;


    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        tr = GetComponentInChildren<TrailRenderer>();
        tr.enabled = false;
        audioSource = GetComponent<AudioSource>();

        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        explosion.SetActive(false);
        spriteObject.SetActive(true);
        smokeParticles.Stop();

        X_LIMIT = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0.0f, 0.0f)).x;

        currentFuel = maxFuel;
    }

    void Update()
    {
        if (hasExploded) return;

        float horizontal = joystick.Horizontal;
        float vertical = joystick.Vertical;

        Vector2 vec = new Vector2(horizontal, vertical);
        float magnitude = vec.magnitude;
        vec.Normalize();

        // Up boosting movement
        if (magnitude > 0)
        {
            rb2d.velocity = new Vector2(transform.up.x * vec.magnitude * speed, transform.up.y * vec.magnitude * speed*1.1f);
            //rb2d.velocity = new Vector2(vec.x * speed, vec.y * speed);

            tr.enabled = true;

            if (!engineSparkPlayed) // Hard to hear, probably just ignore this
            {
                audioSource.PlayOneShot(engineSpark, 1.0f);
                engineSparkPlayed = true;
            }
            audioSource.volume = 0.1f;

            // Drain fuel
            currentFuel -= fuelRate * Time.deltaTime;
            if (currentFuel <= 0)
            {
                currentFuel = 0;
                Explode();
            }
        }
        else
        {
            tr.Clear();
            tr.enabled = false;

            audioSource.volume = 0.0f;
            engineSparkPlayed = false;
        }

        // Left right tilt movement
        if (horizontal != 0)
        {
            float angle = Vector2.Angle(vec, transform.up);

            Quaternion target = Quaternion.FromToRotation(Vector2.up, vec);
            if ((transform.up.y < 0 && vec.y >= 0) || (transform.up.y >= 0 && vec.y < 0)) target.eulerAngles = new Vector4(target.eulerAngles.x, target.eulerAngles.y, target.eulerAngles.z + Mathf.PI*3/2);
            //if ((transform.up.y > 0 && vec.y < 0) || (transform.up.y <= 0 && vec.y >= 0)) target = Quaternion.FromToRotation(transform.up * -1, vec * -1);
            //else target = Quaternion.FromToRotation(transform.up, vec);
            transform.rotation = Quaternion.Lerp(transform.rotation, target, tiltSpeed * Time.deltaTime);

            //    Old stuff
            //transform.Rotate(0.0f, 0.0f, -horizontal * tiltSpeed, Space.Self);
        }

        //float currAngle = transform.rotation.z;
        //Debug.Log("currangle = " + currAngle.ToString());

        //float newAngle = Vector2.Angle(transform.up, vec);
        //if (horizontal > 0)
        //{
        //    newAngle *= -1;
        //}
        //Debug.Log("newAngle = " + newAngle.ToString());

        //transform.Rotate(0.0f, 0.0f, newAngle, Space.Self);

        //Quaternion target = Quaternion.FromToRotation(transform.up, vec);
        //transform.rotation = Quaternion.Lerp(transform.rotation, target, tiltSpeed * Time.deltaTime);

        // Old stuff
        //transform.Rotate(0.0f, 0.0f, -horizontal * tiltSpeed, Space.World);


        // Screen wrapping
        WrapXPosition();
        if (transform.position.x > X_LIMIT + 0.3f || transform.position.x < -X_LIMIT - 0.3f)
        {
            tr.Clear();
            tr.enabled = false;
        }

        // Smoke particles if fuel < 30%
        if (currentFuel < 30f)
        {
            if (smokeParticles.isStopped)
            {
                smokeParticles.Play();
            }
        }
        else if (currentFuel >= 30f)
        {
            if (smokeParticles.isPlaying)
            {
                smokeParticles.Stop();
            }
        }

        // Set fuel amount
        fuelBar.SetFuelAmount(currentFuel);
    }

    private void WrapXPosition()
    {
        Vector3 currentPosition = gameObject.transform.position;

        // The 0.5f and 0.4f is buffer, makes trail renderer not glitch out
        if (currentPosition.x >= (X_LIMIT + 0.5f))
        {
            tr.Clear();
            currentPosition.x = -X_LIMIT - 0.4f;
        }
        else if (currentPosition.x <= (-X_LIMIT - 0.5f))
        {
            tr.Clear();
            currentPosition.x = X_LIMIT + 0.4f;
        }
        gameObject.transform.position = currentPosition;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (hasExploded) return;

        // Lose 10% fuel on every collision
        if (collision.gameObject.CompareTag("Asteroid")
            || collision.gameObject.CompareTag("Meteor"))
        {
            currentFuel -= 10f;
            gameManager.PlaySound(GameManager.SoundClip.Impact);
            StartCoroutine(DamageRocket());
        }

        // Restore 40%(TBD) fuel upon fuel can pick-up
        else if (collision.gameObject.CompareTag("Fuel"))
        {
            //Debug.Log("Collider: Hit a fuel");
            Destroy(collision.gameObject);
            currentFuel += 30f;
            currentFuel = Mathf.Min(currentFuel, 100f);
            gameManager.PlaySound(GameManager.SoundClip.Refill);
        }

        // Got sucked into center of black hole
        else if (collision.gameObject.CompareTag("BlackholeCenter"))
        {
            currentFuel = 0f;
            Explode();
        }

        // For playing landing audio
        else if (!collision.gameObject.CompareTag("Planet"))
        {
            //Debug.Log("Collider: Hit a planet");
            if (collision.relativeVelocity.magnitude < 2f)
            {
                // Don't play audio
            }
            else if (collision.relativeVelocity.magnitude < 4f)
            {
                gameManager.PlaySound(GameManager.SoundClip.SoftLanding);
            }
            else
            {
                gameManager.PlaySound(GameManager.SoundClip.HardLanding);
            }
        }

        // For checking whether we've docked on a planet
        if (collision.gameObject.CompareTag("Dock"))
        {
            if (transform.position.y > heightOfLastDocking)
            {
                heightOfLastDocking = transform.position.y + 2f; // 2f is buffer, in case we hit sides of platform
                gameManager.UpdateScore();
                // Increase meteor spawn rate when player docks
                gameManager.IncreaseMeteorSpawnRate();
            }
        }
    }

    // Called when rocket hits a meteor/asteroid, tints red briefly
    IEnumerator DamageRocket()
    {
        spriteObject.GetComponent<SpriteRenderer>().color = new Color32(255, 150, 150, 255);

        yield return new WaitForSeconds(0.3f);

        spriteObject.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 255);
    }

    // Called when we reach fuel 0
    private void Explode()
    {
        hasExploded = true;
        audioSource.volume = 0f;
        gameManager.PlaySound(GameManager.SoundClip.Explode);

        explosion.SetActive(true);
        spriteObject.SetActive(false);
        fuelCanvas.SetActive(false);
        smokeParticles.Stop();
        StartCoroutine(ResetGame());
    }

    IEnumerator ResetGame()
    {
        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Called to fade engine noise in or out. Terrible sounding, don't use this...
    //IEnumerator StartFade(float duration, float targetVolume)
    //{
    //    float currentTime = 0;
    //    float start = audioSource.volume;

    //    while (currentTime < duration)
    //    {
    //        currentTime += Time.deltaTime;
    //        audioSource.volume = Mathf.Lerp(start, targetVolume, currentTime / duration);
    //        yield return null;
    //    }
    //    yield break;
    //}

}
