using System;
using UnityEngine;
using System.Runtime.InteropServices;

public class Web3Bridge : MonoBehaviour
{
    public static Web3Bridge Instance { get; private set; }

    public event Action<LeaderboardEntryData[]> OnLeaderboardReceived;
    public event Action<bool, string, string> OnScoreSubmissionComplete;

#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")] private static extern void RequestLeaderboardJS();
    [DllImport("__Internal")] private static extern void SubmitScoreJS(string playerName, int score);
#endif

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void RequestLeaderboard()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        RequestLeaderboardJS();
#else
        Debug.Log("[Web3Bridge] RequestLeaderboard() - no-op outside a WebGL build.");
#endif
    }

    public void SubmitScore(string playerName, int score)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        SubmitScoreJS(playerName, score);
#else
        Debug.Log($"[Web3Bridge] SubmitScore({playerName}, {score}) - no-op outside a WebGL build.");
#endif
    }

    // Called by JavaScript via unityInstance.SendMessage("Web3Bridge", "ReceiveLeaderboard", json)
    public void ReceiveLeaderboard(string json)
    {
        LeaderboardResponse response = JsonUtility.FromJson<LeaderboardResponse>(json);
        OnLeaderboardReceived?.Invoke(response.entries);
    }

    // Called by JavaScript via unityInstance.SendMessage("Web3Bridge", "ReceiveScoreSubmissionResult", json)
    public void ReceiveScoreSubmissionResult(string json)
    {
        ScoreSubmissionResult result = JsonUtility.FromJson<ScoreSubmissionResult>(json);
        OnScoreSubmissionComplete?.Invoke(result.success, result.message, result.txHash);
    }
}

[Serializable]
public class LeaderboardEntryData
{
    public string player;
    public long score;
}

[Serializable]
public class LeaderboardResponse
{
    public LeaderboardEntryData[] entries;
}

[Serializable]
public class ScoreSubmissionResult
{
    public bool success;
    public string message;
    public string txHash;
}
