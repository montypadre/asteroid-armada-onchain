using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeStage.AntiCheat.ObscuredTypes;
using CodeStage.AntiCheat.Detectors;

public class LazerBehavior : MonoBehaviour
{
    public float speed;
    public ObscuredInt damage = 40;
    private bool cheaterDetected = false;

    void Start()
    {
        ObscuredCheatingDetector.StartDetection(OnCheaterDetected);
    }

    private void OnCheaterDetected()
    {
        cheaterDetected = true;
    }

    void Update()
    {
        if (cheaterDetected)
        {
            Debug.Log("'I would prefer even to fail with honor than win by cheating' - Sophocles");
            Application.Quit();
        }

        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        AsteroidHealth asteroidHealth = other.gameObject.GetComponent<AsteroidHealth>();

        if (asteroidHealth != null)
        {
            asteroidHealth.DealDamage(damage);
        }

        GameObject.Destroy(gameObject);
    }

    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
