using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using CodeStage.AntiCheat.ObscuredTypes;
using CodeStage.AntiCheat.Detectors;

public class PlayerController : MonoBehaviour
{
    public float fuel = 100f;
    public float currentFuel = 0f;
    public float rotationSpeed;
    public float movementSpeed;
    public GameObject lazer;
    public AudioClip lazerFire;
    public float lazerVolume;
    public float fuelDrainRate = 4.5f;      // fuel per second, replaces the old flat per-frame deduction
    private bool rapidFireActive;
    private bool spreadFireActive;
    private bool hyperBlossomFireActive;
    public float cooldown = 1f;
    private float time = 0f;
    private bool cheaterDetected = false;
    public GameController gameController;

    void Start()
    {
        ObscuredCheatingDetector.StartDetection(OnCheaterDetected);
        gameController = GameObject.FindWithTag("GameController").GetComponent<GameController>();
        GetComponent<Rigidbody>().freezeRotation = true;
        currentFuel = fuel;
    }

    private void OnCheaterDetected()
    {
        cheaterDetected = true;
    }

    void OnDestroy()
    {
        if (gameController != null)
        {
            gameController.PlayerDies();
        }
    }

    void Update()
    {
        if (cheaterDetected)
        {
            Debug.Log("'I would prefer even to fail with honor than win by cheating' - Sophocles");
            Application.Quit();
        }

        if (time > 0f)
        {
            time -= Time.deltaTime;
        }
        else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space) && rapidFireActive == false && spreadFireActive == false && hyperBlossomFireActive == false && GetFuel() > 0)
        {
            UseFuel();
            Instantiate(lazer, transform.TransformPoint(Vector3.forward * 2), transform.rotation);
            AudioSource.PlayClipAtPoint(lazerFire, 0.9f * Camera.main.transform.position + 0.1f * transform.position, lazerVolume);
            time = cooldown;
        }
        else if (rapidFireActive == true && Input.GetKey(KeyCode.Return) || rapidFireActive == true && Input.GetKey(KeyCode.Space))
        {
            Instantiate(lazer, transform.TransformPoint(Vector3.forward * 2), transform.rotation);
            AudioSource.PlayClipAtPoint(lazerFire, 0.9f * Camera.main.transform.position + 0.1f * transform.position, lazerVolume);
            time = cooldown;
        }
        else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space) && spreadFireActive == true)
        {
            AudioSource.PlayClipAtPoint(lazerFire, 0.9f * Camera.main.transform.position + 0.1f * transform.position, lazerVolume);

            // Shoot lazer in spreading pattern
            Instantiate(lazer, transform.TransformPoint(Vector3.forward * 2), transform.rotation * Quaternion.Euler(0, 90, 0));
            Instantiate(lazer, transform.TransformPoint(Vector3.forward * 2), transform.rotation * Quaternion.Euler(0, 45, 0));
            Instantiate(lazer, transform.TransformPoint(Vector3.forward * 2), transform.rotation * Quaternion.Euler(0, 30, 0));
            Instantiate(lazer, transform.TransformPoint(Vector3.forward * 2), transform.rotation * Quaternion.Euler(0, 15, 0));
            Instantiate(lazer, transform.TransformPoint(Vector3.forward * 2), transform.rotation);
            Instantiate(lazer, transform.TransformPoint(Vector3.forward * 2), transform.rotation * Quaternion.Euler(0, -15, 0));
            Instantiate(lazer, transform.TransformPoint(Vector3.forward * 2), transform.rotation * Quaternion.Euler(0, -30, 0));
            Instantiate(lazer, transform.TransformPoint(Vector3.forward * 2), transform.rotation * Quaternion.Euler(0, -45, 0));
            Instantiate(lazer, transform.TransformPoint(Vector3.forward * 2), transform.rotation * Quaternion.Euler(0, -90, 0));
          
            time = cooldown * 0.10f;
        }
        else if (hyperBlossomFireActive)
        {
            Instantiate(lazer, transform.TransformPoint(Vector3.forward * 2), transform.rotation);
            transform.Rotate(0, 360f * Time.deltaTime * 2, 0);
        }

        if (Input.GetKey(KeyCode.W) && GetFuel() > 0)
        {
            UseFuel();
            GetComponent<Rigidbody>().AddForce(transform.forward * movementSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.S) && GetFuel() > 0)
        {
            UseFuel();
            GetComponent<Rigidbody>().AddForce(transform.forward * -movementSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.A) && GetFuel() > 0)
        {
            UseFuel();
            transform.Rotate(-Vector3.up * rotationSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.D) && GetFuel() > 0)
        {
            UseFuel();
            transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
        }
           
        if (Input.GetKey(KeyCode.UpArrow) && GetFuel() > 0)
        {
            UseFuel();
            GetComponent<Rigidbody>().AddForce(transform.forward * movementSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.DownArrow) && GetFuel() > 0)
        {
            UseFuel();
            GetComponent<Rigidbody>().AddForce(transform.forward * -movementSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.LeftArrow) && GetFuel() > 0)
        {
            UseFuel();
            transform.Rotate(-Vector3.up * rotationSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.RightArrow) && GetFuel() > 0)
        {
            UseFuel();
            transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
        }
    }

    void UseFuel()
    {
        currentFuel -= fuelDrainRate * Time.deltaTime;
        gameController.UpdateFuel(currentFuel);
    }

    public float GetFuel()
    {
        return currentFuel;
    }

    public void ActivateRapidFirePowerUp()
    {
        StartCoroutine(ToggleRapidFire(30f));
    }

    public void ActivateSpreadFirePowerUp()
    {
        StartCoroutine(ToggleSpreadFire(30f));
    }

    public void ActivateHyperBlossomFirePowerUp()
    {
        StartCoroutine(ToggleHyperBlossomFire(10f));
    }

    IEnumerator ToggleRapidFire(float duration)
    {
        rapidFireActive = true;
        yield return new WaitForSecondsRealtime(duration);
        rapidFireActive = false;
    }

    IEnumerator ToggleSpreadFire(float duration)
    {
        spreadFireActive = true;
        yield return new WaitForSecondsRealtime(duration);
        spreadFireActive = false;
    }

    IEnumerator ToggleHyperBlossomFire(float duration)
    {
        hyperBlossomFireActive = true;
        InvokeRepeating("PlayShot", 0.1f, 0.1f);
        yield return new WaitForSecondsRealtime(duration);
        CancelInvoke();
        hyperBlossomFireActive = false;
    }

    void PlayShot()
    {
        AudioSource.PlayClipAtPoint(lazerFire, 0.9f * Camera.main.transform.position + 0.1f * transform.position, lazerVolume);
    }
}
