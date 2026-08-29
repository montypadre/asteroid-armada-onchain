using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LB_Controller : MonoBehaviour
{
    [SerializeField] GameObject leaderboardStoreScore;
    [SerializeField] string API_KEY;
    [SerializeField] int boardid;

   
    private LB_Entry[] leaderboardEntries = new LB_Entry[0];

    public static LB_Controller instance;

    public delegate void OnAllScoresUpdated(LB_Entry[] entries);
    public static OnAllScoresUpdated OnUpdatedScores;

    private void Awake() {
        if (instance == null) {
            instance = this;
            if (instance == null) {
                instance = new LB_Controller();
            }
            instance.ReloadLeaderboard(boardid);
        }
    }

    // Start is called before the first frame update
    void Start() {
        DontDestroyOnLoad(this.gameObject);
    }

    public void StoreScore(float score, string username, int boardid) {
        GameObject lbInstance = Instantiate(leaderboardStoreScore, new Vector3(0, 0, 0), Quaternion.identity);
        LB_StoreScore storeScore = lbInstance.GetComponent<LB_StoreScore>();
        storeScore.StoreScore(score, username, boardid, API_KEY);
    }

    public void ReloadLeaderboard(int boardid) {
        LB_GetAllScores request = gameObject.GetComponent<LB_GetAllScores>();
        LB_GetAllScores.OnFinishedDelegate += OnRequestFinished;
        request.GetAllScores(boardid, API_KEY); 
    }

    private void OnRequestFinished(LB_Entry[] entries) {
        leaderboardEntries = entries; 
        LB_GetAllScores.OnFinishedDelegate -= OnRequestFinished;
        OnUpdatedScores?.Invoke(leaderboardEntries); 
    }

    public LB_Entry[] Entries() {
        return leaderboardEntries; 
    }

    private void OnDestroy() {
        
    }
}
