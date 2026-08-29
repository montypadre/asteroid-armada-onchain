using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeStage.AntiCheat.ObscuredTypes;
using CodeStage.AntiCheat.Detectors;

public class PlayerHealth : MonoBehaviour
{
    public ObscuredInt health = 100;
    private ObscuredInt currentHealth = 0;
    public ObscuredInt currentShield = 100;
    private bool cheaterDetected = false;

    public GameObject explosion;
    private bool exploding = false;
    public AudioClip explosionClip;
    public float explosionVolume;

    public int scoreValue = 0;
    public GameController gameController;

    void Start()
    {
        ObscuredCheatingDetector.StartDetection(OnCheaterDetected);
        gameController = GameObject.FindWithTag("GameController").GetComponent<GameController>();
        currentHealth = health;
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

    public int GetHealth()
    {
        return currentHealth;
    }

    public int GetShield()
    {
        return currentShield;
    }

    public void SetCurrentShield(int shield)
    {
        currentShield = shield;
    }

    public void DealDamage(int damage)
    {
        if (gameController.ShieldsUp() == true && currentShield > 0)
        {
            currentShield -= damage;
            gameController.UpdateShield(currentShield);
        }
        else if (currentShield <= 0)
        {
            gameController.DisengageShield();
            currentHealth -= damage;
            gameController.UpdateHealth(currentHealth);
        }
        else
        {
            currentHealth -= damage;
            gameController.UpdateHealth(currentHealth);
        }
        
        if (currentHealth <= 0 && !exploding)
        {
            StartCoroutine(Explode());
            if (scoreValue > 0)
            {
                gameController.IncreaseScore(scoreValue);
            }
        }
    }

    IEnumerator Explode()
    {
        exploding = true;
        yield return new WaitForSeconds(0.1f);
        GameObject explosionGo = Instantiate(explosion, transform.position, Quaternion.Euler(0, 0, 0));
        Destroy(gameObject, 0.2f);
        Destroy(explosionGo, 0.2f);
        yield return 0;
        AudioSource.PlayClipAtPoint(explosionClip, 0.9f * Camera.main.transform.position + 0.1f * transform.position, 10f);
        yield return new WaitForSeconds(0.1f);
        exploding = false;
    }
}
