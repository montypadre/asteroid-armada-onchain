using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighScoreController : MonoBehaviour
{
    public Text[] names;
    public Text[] scores;

    public static HighScoreController instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            if (instance == null)
            {
                instance = new HighScoreController();
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // TODO (Phase 2): request the leaderboard via the JS bridge instead of Nethereum.
        // The React host will call back into a public method here (e.g. OnLeaderboardReceived)
        // once the on-chain data has been fetched.
    }
}