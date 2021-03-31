using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidOrbit : MonoBehaviour
{
    private Vector3 parentPlanet;
    private bool hasParent = false;
    private float rotateDirection;
    private float orbitRadius;
    private float orbitAngle;

    private float orbitOffset = 2.8f;
    private float rotateSpeed = 45f;
    private float variation = 1f;

    void Start()
    {
        variation = Random.Range(0.8f, 1.2f);
        rotateDirection = (variation > 0f) ? 1f : -1f;
        orbitOffset *= variation;
        rotateSpeed *= variation;
    }

    public void SetParentPlanet(Vector3 pos, float direct, float radius, float angleOffset)
    {
        parentPlanet = pos;
        rotateDirection = direct;
        orbitRadius = radius + orbitOffset;
        orbitAngle = angleOffset;
        hasParent = true;

        Vector3 relativeV = Quaternion.Euler(0f, 0f, orbitAngle) * Vector3.up;
        relativeV *= orbitRadius;
        gameObject.transform.position = parentPlanet + relativeV;
    }

    void Update()
    {
        gameObject.transform.Rotate(0f, 0f, 2f * rotateSpeed * rotateDirection * Time.deltaTime);
        if (!hasParent) return;

        orbitAngle += rotateSpeed * rotateDirection * Time.deltaTime;
        if (orbitAngle > 180f) orbitAngle -= 360f;
        else if (orbitAngle < -180f) orbitAngle += 360f;

        Vector3 relativeV = Quaternion.Euler(0f, 0f, orbitAngle) * Vector3.up;
        relativeV *= orbitRadius;
        gameObject.transform.position = parentPlanet + relativeV;
    }
}
