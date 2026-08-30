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

    void Start()
    {
        Web3Bridge.Instance.OnLeaderboardReceived += HandleLeaderboardReceived;
        Web3Bridge.Instance.RequestLeaderboard();
    }

    private void Oestroy()
    {
        if (Web3Bridge.Instance != null)
        {
            Web3Bridge.Instance.OnLeaderboardReceived -= HandleLeaderboardReceived;
        }       
    }

    private void HandleLeaderboardReceived(LeaderboardEntryData[] entries)
    {
        for (int i = 0; i < names.Length && i < entries.Length; i++)
        {
            names[i].text = string.IsNullOrEmpty(entries[i].player) ? "-" : entries[i].player;
            scores[i].text = entries[i].score.ToString();
        }
    }
}