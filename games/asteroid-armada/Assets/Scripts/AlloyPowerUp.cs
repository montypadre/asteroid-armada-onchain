using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlloyPowerUp : MonoBehaviour
{
    private float fuel;
    private bool powerupActive;
    public GameController gameController;
    public AudioClip alloyPowerUpClip;

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
        AudioSource.PlayClipAtPoint(alloyPowerUpClip, 0.9f * Camera.main.transform.position + 0.1f * transform.position, 10f);
        fuel = player.GetComponent<PlayerController>().GetFuel();
        if (fuel < 100.0f)
        {
            fuel += 10f;
            gameController.UpdateFuel(fuel);
        }

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
