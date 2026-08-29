using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HyperBlossomFirePowerUp : MonoBehaviour
{
    private bool powerupActive;
    public AudioClip hyperBlossomFirePowerUpClip;
    public GameController gameController;

    void Start()
    {
        gameController = GameObject.FindWithTag("GameController").GetComponent<GameController>();
    }

    void Update()
    {
        if (!powerupActive)
        {
            StartCoroutine(PowerUp(10f));
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Pickup(other);
        }
    }

    void Pickup(Collider player)
    {
        AudioSource.PlayClipAtPoint(hyperBlossomFirePowerUpClip, 0.9f * Camera.main.transform.position + 0.1f * transform.position, 10f);
        player.GetComponent<PlayerController>().ActivateHyperBlossomFirePowerUp();

        Destroy(gameObject);
    }

    IEnumerator PowerUp(float duration)
    {
        powerupActive = true;
        yield return new WaitForSeconds(duration);
        powerupActive = false;
        Destroy(gameObject);
    }
}
