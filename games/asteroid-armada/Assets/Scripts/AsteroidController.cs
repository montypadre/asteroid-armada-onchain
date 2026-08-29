using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeStage.AntiCheat.ObscuredTypes;
using CodeStage.AntiCheat.Detectors;


public class AsteroidController : MonoBehaviour
{
    public ObscuredInt damage;
    public GameObject sparks;
    public AudioClip impact;
    public float force;
    private bool cheaterDetected = false;
    public GameController gameController;

    void Start()
    {
        ObscuredCheatingDetector.StartDetection(OnCheaterDetected);
        gameController = GameObject.FindWithTag("GameController").GetComponent<GameController>();
        GetComponent<Rigidbody>().AddForce(transform.forward * 700f);
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
    }

    private void OnCollisionStay(Collision other)
    {
        if (other.gameObject.tag == "Player")
        {
            PlayerHealth playerHealth = other.gameObject.GetComponent<PlayerHealth>();
            PlayerController player = other.gameObject.GetComponent<PlayerController>();

            // Bounces player off asteroid
            Vector3 dir = other.contacts[0].point - player.transform.position;
            dir = -dir.normalized;
            player.GetComponent<Rigidbody>().AddForce(dir * force);

            if (playerHealth != null)
            {
                AudioSource.PlayClipAtPoint(impact, 0.9f * Camera.main.transform.position + 0.1f * transform.position, 10f);
                GameObject sparksGo = Instantiate(sparks, other.transform.position, other.transform.rotation);
                Destroy(sparksGo, 0.1f);
                playerHealth.DealDamage(damage);
            }
        }
    }

    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
