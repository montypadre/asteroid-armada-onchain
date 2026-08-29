using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Runtime.InteropServices;
using CodeStage.AntiCheat.ObscuredTypes;
using CodeStage.AntiCheat.Detectors;
// use web3.jslib
// using Nethereum RPC
using Nethereum.JsonRpc.UnityClient;
// using leaderboard contract definition
using LeaderboardContract.Contracts.Leaderboard.ContractDefinition;
// using gamelog contract definition
using GameLogContract.Contracts.GameLog.ContractDefinition;


public class GameController : MonoBehaviour
{
    public GameObject player;
    public InputField playerNameInput;
    private Animation wave1Anim;
    private Animation wave2Anim;
    private Animation wave3Anim;
    private Animation wave4Anim;
    public Text wave1;
    public Text wave2;
    public Text wave3;
    public Text wave4;
    private bool animPlayed;
    public GameObject[] standardAsteroidObjects;
    public GameObject[] hyperAsteroidObjects;
    public GameObject[] explodingAsteroidObjects;
    private bool cheaterDetected = false;


    public float maxRange = 10f;
    public float minRange = 5f;
    public float maximumScale = 5f;
    public float minimumScale = 5f;
    public float spawnInterval = 3f;

    public Vector3 screenCenter;

    float time = 0.0f;
    float minY;
    float maxY;
    float minX;
    float maxX;

    public float gameOverDelay = 1f;
    public float gameOverExpire = 10f;
    public float playerExpire = 30f;

    public GameObject scoreValue;
    public Text highScore;
    public GameObject gamePanel;
    public HealthBar healthBar;
    public ShieldBar shieldBar;
    public HyperFuelBar hyperFuelBar;
    public GameObject highScorePanel;
    public GameObject gameOverPanel;
    public GameObject blurPanel;
    private ObscuredInt playerShield;
    private ObscuredInt currentPlayerShield;

    public bool isPlayerAlive = true;
    bool findPlayer = false;
    public AudioClip impact;
    bool areShieldsUp = false;
    private bool canSpawn = true;

    public float playedTime;

    private long gameSeed;

    public string gameServeAddress;

    // use WalletAddress function from web3.jslib
    [DllImport("__Internal")] private static extern string WalletAddress();
    string url;
    string contractAddress;
    string gameLogContractAddress;

    void Start()
    {
        gameSeed = new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds();

        //StartCoroutine(SignAndSendGameServeAddress(gameServeAddress));
        //StartCoroutine(SignAndSendGameInfo("asteroids", "0x89267B20f07A0AdD565C5f60e4abde37Cc55d6eD", 250000, gameSeed));

        ObscuredCheatingDetector.StartDetection(OnCheaterDetected);
        SpeedHackDetector.StartDetection(OnCheaterDetected);

        // Setting the active panel
        gameOverPanel.SetActive(false);
        highScorePanel.SetActive(false);
        shieldBar.gameObject.SetActive(false);
        gamePanel.SetActive(true);

        healthBar.SetMaxHealth(100);
        hyperFuelBar.SetMaxFuel(100);

        // Instantiating player
        player = Instantiate(player, new Vector3(0, 0, 0), Quaternion.Euler(-90, 0, 0));
        screenCenter = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 0));

        isPlayerAlive = true;

        this.minY = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, -Camera.main.transform.position.z)).y;
        this.maxY = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, -Camera.main.transform.position.z)).y;
        this.minX = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, -Camera.main.transform.position.z)).x;
        this.maxX = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, -Camera.main.transform.position.z)).x;

        wave1Anim = wave1.GetComponent<Animation>();
        wave2Anim = wave2.GetComponent<Animation>();
        wave3Anim = wave3.GetComponent<Animation>();
        wave4Anim = wave4.GetComponent<Animation>();
        animPlayed = false;

        // Set contract URL and address
        url = "https://data-seed-prebsc-1-s2.binance.org:8545/";
        contractAddress = "0x8c8559286612050B75a232e1ccDA5bEC8d771a5b";
        gameLogContractAddress = "0xEBc9068f6dEAEdb959d8eb409e226054C562032D";
    }

    //private IEnumerator SignAndSendGameServeAddress(string gameServeAddress)
    //{
    //    yield return new WaitForSeconds(4);
    //    var gameServeAddressRequest = new TransactionSignedUnityRequest(url, "2a135a7c4a0309f4e77a197d863803f2127ba31119d14fe5e91ae6f24e0ef2bf", 97);

    //    if (gameServeAddress != null)
    //    {
    //        yield return gameServeAddressRequest.SignAndSendTransaction(new UpdateGameServeAddressFunction() { GameServeAddress = gameServeAddress }, gameLogContractAddress);
    //    }
    //    if (gameServeAddressRequest.Exception == null)
    //    {
    //        Debug.Log("Game Serve Address submitted tx: " + gameServeAddressRequest.Result);
    //    }
    //    else
    //    {
    //        Debug.Log("Error submitted tx: " + gameServeAddressRequest.Exception.Message);
    //    }
    //}

    //private IEnumerator SignAndSendGameInfo(string gameId, string userAddress, int score, long gameSeed)
    //{
    //    yield return new WaitForSeconds(8);
    //    var gameInfoRequest = new TransactionSignedUnityRequest(url, "2a135a7c4a0309f4e77a197d863803f2127ba31119d14fe5e91ae6f24e0ef2bf", 97);

    //    if (gameId != null && userAddress != null)
    //    {
    //        yield return gameInfoRequest.SignAndSendTransaction(new UpdateGameInfoFunction() { GameId = gameId, UserAddress = userAddress, Score = score, GameSeed = gameSeed }, gameLogContractAddress);
    //    }
    //    if (gameInfoRequest.Exception == null)
    //    {
    //        Debug.Log("Game Info submitted tx: " + gameInfoRequest.Result);
    //        StartCoroutine(ValidateGameSeed(gameSeed));
    //    }
    //    else
    //    {
    //        Debug.Log("Error submitted tx: " + gameInfoRequest.Exception.Message);
    //    }
    //}

    private IEnumerator ValidateGameSeed(long gameSeed)
    {
        var queryRequest = new QueryUnityRequest<IsGameSeedAlreadyUsedFunction, IsGameSeedAlreadyUsedOutputDTOBase>("https://data-seed-prebsc-1-s2.binance.org:8545/", gameLogContractAddress);
        yield return queryRequest.Query(new IsGameSeedAlreadyUsedFunction() { GameSeed = gameSeed }, gameLogContractAddress);
        Debug.Log(gameSeed);
        Debug.Log("Game Seed Validated: " + queryRequest.Result);
        //if (queryRequest.Result.SeedUsed)
        //{
        //    Debug.Log("Seed already exists");
        //}
        //else
        //{
        //    Debug.Log("Seed does not already exist");
        //}
    }

    private void OnCheaterDetected()
    {
        cheaterDetected = true;
    }

    public Vector3 GetNewPosition(Vector3 position)
    {
        return new Vector3(screenCenter.x - position.x, screenCenter.y - position.y, 0);
    }

    bool FindPlayer()
    {
        Collider[] colliders = player.GetComponents<Collider>();

        Collider collider;

        if (colliders[0].isTrigger)
        {
            collider = colliders[0];
        }
        else
        {
            collider = colliders[1];
        }

        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
        if (GeometryUtility.TestPlanesAABB(planes, collider.bounds))
            return true;
        else
            return false;
    }

    void InstantiateRandomAsteroid()
    {
        bool targetPending = true;

        float spawnX = 0;
        float spawnY = 0;

        while (targetPending)
        {
            if (UnityEngine.Random.value > 0.5f)
            {
                Range[] rangesX = new Range[] { new Range(minX - maxRange, minX - minRange), new Range(maxX + minRange, maxX + maxRange) };
                spawnX = RandomValueFromRanges(rangesX);
                spawnY = UnityEngine.Random.Range(minY - maxRange, maxY + maxRange);
            }
            else
            {
                Range[] rangesY = new Range[] { new Range(minY - maxRange, minY - minRange), new Range(maxY + minRange, maxY + maxRange) };
                spawnX = UnityEngine.Random.Range(minX - maxRange, maxX + maxRange);
                spawnY = RandomValueFromRanges(rangesY);
            }

            // Avoiding spawning 2 asteroids on top of each other
            Collider[] colliders = Physics.OverlapBox(new Vector3(spawnX, spawnY, 0), new Vector3(1, 1, 1));

            targetPending = colliders.Length > 0;
        }
        // Level 1
        if (playedTime <= 150)
        {
            while (!animPlayed)
            {
                wave1.gameObject.SetActive(true);
                wave1Anim.Play("fade-in-out");
                animPlayed = true;
            }

            GameObject asteroidObject = Instantiate(standardAsteroidObjects[UnityEngine.Random.Range(0, standardAsteroidObjects.Length - 1)], new Vector3(spawnX, spawnY, 0), Quaternion.Euler(0, 0, 0));

            asteroidObject.transform.LookAt(screenCenter);
            float scale = UnityEngine.Random.Range(minimumScale, maximumScale);

            asteroidObject.transform.localScale = new Vector3(scale, scale, scale);
        }
        // Level 2
        else if (playedTime > 150 && playedTime <= 300)
        {
            while (animPlayed)
            {
                wave1.gameObject.SetActive(false);
                wave2.gameObject.SetActive(true);
                wave2Anim.Play("fade-in-out");
                animPlayed = false;
            }
            StartCoroutine(ClearAsteroids());

            GameObject asteroidObject = Instantiate(standardAsteroidObjects[UnityEngine.Random.Range(0, standardAsteroidObjects.Length - 1)], new Vector3(spawnX, spawnY, 0), Quaternion.Euler(0, 0, 0));
            GameObject hyperAsteroidObject = Instantiate(hyperAsteroidObjects[UnityEngine.Random.Range(1, hyperAsteroidObjects.Length - 1)], new Vector3(spawnX, spawnY, 0), Quaternion.Euler(0, 0, 0));

            asteroidObject.transform.LookAt(screenCenter);
            hyperAsteroidObject.transform.LookAt(screenCenter);
            float scale = UnityEngine.Random.Range(minimumScale, maximumScale);

            //asteroidObject.transform.localScale = new Vector3(scale, scale, scale);
            //hyperAsteroidObject.transform.localScale = new Vector3(scale, scale, scale);
        }
        // Level 3 
        else if (playedTime > 300 && playedTime <= 450)
        {
            while (!animPlayed)
            {
                wave2.gameObject.SetActive(false);
                wave3.gameObject.SetActive(true);
                wave3Anim.Play("fade-in-out");
                animPlayed = true;
            }
            StartCoroutine(ClearAsteroids());

            GameObject asteroidObject = Instantiate(explodingAsteroidObjects[UnityEngine.Random.Range(0, explodingAsteroidObjects.Length - 1)], new Vector3(spawnX, spawnY, 0), Quaternion.Euler(0, 0, 0));
            asteroidObject.transform.LookAt(screenCenter);
            float scale = UnityEngine.Random.Range(minimumScale, maximumScale);

            asteroidObject.transform.localScale = new Vector3(scale, scale, scale);

        }
        // Level 4
        else
        {
            while (animPlayed)
            {
                wave3.gameObject.SetActive(false);
                wave4.gameObject.SetActive(true);
                wave4Anim.Play("fade-in-out");
                animPlayed = false;
            }
            StartCoroutine(ClearAsteroids());

            GameObject asteroidObject1 = Instantiate(hyperAsteroidObjects[UnityEngine.Random.Range(0, hyperAsteroidObjects.Length - 1)], new Vector3(spawnX, spawnY, 0), Quaternion.Euler(0, 0, 0));
            GameObject asteroidObject2 = Instantiate(explodingAsteroidObjects[UnityEngine.Random.Range(0, explodingAsteroidObjects.Length - 1)], new Vector3(spawnX, spawnY, 0), Quaternion.Euler(0, 0, 0));
            asteroidObject1.transform.LookAt(screenCenter);
            asteroidObject2.transform.LookAt(screenCenter);
            float scale = UnityEngine.Random.Range(minimumScale, maximumScale);

            asteroidObject1.transform.localScale = new Vector3(scale, scale, scale);
            asteroidObject2.transform.localScale = new Vector3(scale, scale, scale);

        }
    }

    IEnumerator ClearAsteroids()
    {
        canSpawn = false;
        yield return new WaitForSeconds(3f);
        canSpawn = true;
    }

    void Update()
    {
        //Debug.Log(playedTime);
        if (cheaterDetected)
        {
            Debug.Log("'I would prefer even to fail with honor than win by cheating' - Sophocles");
            Application.Quit();
        }

        playedTime += Time.unscaledDeltaTime;

        if (isPlayerAlive)
        {
            // As player score increases, decrease spawn interval for asteroids
            if (Int64.Parse(scoreValue.GetComponent<Text>().text) < 1000)
            {
                spawnInterval = 3f;
            }
            else if (Int64.Parse(scoreValue.GetComponent<Text>().text) > 1000 && Int64.Parse(scoreValue.GetComponent<Text>().text) < 5000)
            {
                spawnInterval = 2f;
            }
            else if (Int64.Parse(scoreValue.GetComponent<Text>().text) > 5000 && Int64.Parse(scoreValue.GetComponent<Text>().text) < 10000)
            {
                spawnInterval = 1f;
            }
            else if (Int64.Parse(scoreValue.GetComponent<Text>().text) > 10000 && Int64.Parse(scoreValue.GetComponent<Text>().text) < 50000)
            {
                spawnInterval = 0.5f;
            }

            if (!FindPlayer())
            {
                playerExpire -= Time.deltaTime;
                if (playerExpire <= 10 && !findPlayer)
                {
                    InvokeRepeating("PlayImpact", 0.0f, 1.0f);
                    InvokeRepeating("DepleteHealth", 0.0f, 1.0f);
                }
                else if (playerExpire <= 0)
                {
                    PlayerDies();
                }
                player.transform.position = GetNewPosition(player.transform.position);
            }
            else
            {
                playerExpire = 30f;
            }

            time += Time.deltaTime;

            if (time >= spawnInterval && canSpawn)
            {
                time = time - spawnInterval;

                InstantiateRandomAsteroid();
            }
        }
        else
        {
            if (!highScorePanel.activeSelf)
            {
                if (time < gameOverDelay)
                {
                    time = time + Time.deltaTime;
                }
                else if (Input.anyKey || time > gameOverExpire)
                {
                    SceneManager.LoadScene("MainMenu");
                }
            }
        }
    }

    // Play impact sound when player has been offscreen for more than 30 seconds
    void PlayImpact()
    {
        AudioSource.PlayClipAtPoint(impact, 0.9f * Camera.main.transform.position + 0.1f * transform.position, 1f);
        findPlayer = true;
    }
    // Deplete player health when player has been offscreen for more than 30 seconds
    void DepleteHealth()
    {
        player.GetComponent<PlayerHealth>().DealDamage(5);
        findPlayer = true;
    }

    public static float RandomValueFromRanges(Range[] ranges)
    {
        if (ranges.Length == 0)
            return 0;
        float count = 0;
        foreach (Range r in ranges)
            count += r.range;
        float sel = UnityEngine.Random.Range(0, count);
        foreach (Range r in ranges)
        {
            if (sel < r.range)
            {
                return r.min + sel;
            }
            sel -= r.range;
        }
        throw new Exception("This should never happen");
    }

    public void IncreaseScore(int score)
    {
        scoreValue.GetComponent<Text>().text = (Int64.Parse(scoreValue.GetComponent<Text>().text) + score).ToString();
    }

    private IEnumerator CheckHighScore()
    {
        int leaderboardIndex = 4;
        var queryRequest = new QueryUnityRequest<LeaderboardFunction, LeaderboardOutputDTOBase>(url, contractAddress);
        yield return queryRequest.Query(new LeaderboardFunction() { ReturnValue1 = leaderboardIndex }, contractAddress);

        if (scoreValue != null)
        {
            if (Int64.Parse(scoreValue.GetComponent<Text>().text) > queryRequest.Result.Score)
            {
                highScorePanel.SetActive(true);
                Cursor.visible = true;
                playerNameInput.Select();
                playerNameInput.onEndEdit.AddListener(delegate { StartCoroutine(SignAndSendTransaction()); });
            }
            else if (Int64.Parse(scoreValue.GetComponent<Text>().text) < queryRequest.Result.Score)
            {
                gamePanel.SetActive(false);
                blurPanel.SetActive(true);
                gameOverPanel.SetActive(true);
                Cursor.visible = true;
            }
        }
    }

    // TODO: Obfusacte private key
    // Going to store private key
    // on server and make a web request to retrieve it when needed
    private IEnumerator SignAndSendTransaction()
    {
        yield return new WaitForSeconds(4);
        var addScoreRequest = new TransactionSignedUnityRequest(url, "2a135a7c4a0309f4e77a197d863803f2127ba31119d14fe5e91ae6f24e0ef2bf", 97);
        Debug.Log(playerNameInput.text);
        if (playerNameInput.text != null)
        {
            yield return addScoreRequest.SignAndSendTransaction(new AddScoreFunction() { User = playerNameInput.text, Score = Convert.ToInt32(scoreValue.GetComponent<Text>().text) }, contractAddress);
        }
        else
        {
            yield return addScoreRequest.SignAndSendTransaction(new AddScoreFunction() { User = "xxx", Score = Convert.ToInt32(scoreValue.GetComponent<Text>().text) }, contractAddress);
        }
        if (addScoreRequest.Exception == null)
        {
            Debug.Log("High score submitted tx: " + addScoreRequest.Result);
        }
        else
        {
            Debug.Log("Error submitted tx: " + addScoreRequest.Exception.Message);
        }
        SceneManager.LoadScene("MainMenu");
    }

    public void UpdateHealth(int health)
    {
        healthBar.SetHealth(health);
    }

    public void EngageShield()
    {
        playerShield = player.GetComponent<PlayerHealth>().GetShield();
        shieldBar.SetMaxShield(100);
        player.GetComponent<PlayerHealth>().SetCurrentShield(100);
        shieldBar.gameObject.SetActive(true);

        areShieldsUp = true;
    }

    public void DisengageShield()
    {
        shieldBar.gameObject.SetActive(false);

        areShieldsUp = false;
    }

    public bool ShieldsUp()
    {
        return areShieldsUp;
    }

    public void UpdateShield(int shield)
    {
        shieldBar.SetShield(shield);
    }

    public void UpdateFuel(float fuel)
    {
        hyperFuelBar.SetFuel(fuel);
        player.GetComponent<PlayerController>().currentFuel = fuel;
    }

    public void PlayerDies()
    {
        CancelInvoke();
        StartCoroutine(CheckHighScore());
        findPlayer = false;
        isPlayerAlive = false;
        time = 0.0f;
    }
}
