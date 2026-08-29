using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OldHighScoreController : MonoBehaviour
{ 
    public Text[] names;
    public Text[] scores;
    [SerializeField] int boardID;

    // Start is called before the first frame update
    void Start() {
        LB_Controller.OnUpdatedScores += OnLeaderboardUpdated;
        StartCoroutine(DownloadScores());
    }

    IEnumerator DownloadScores() {
        yield return new WaitForSeconds(0.5f);
        LB_Controller.instance.ReloadLeaderboard(boardID); // parameter -> board-id
    }

    private void OnLeaderboardUpdated(LB_Entry[] entries) {
        if (entries != null && entries.Length > 0) {
            for (int i = 0; i <= 4; i++)
            {
                names[i].text = entries[i].name;
                scores[i].text = entries[i].points.ToString();
            }
        } else if (entries == null) {
            Debug.Log("Something went wrong");
        }
    }

    private void OnDestroy() {
        LB_Controller.OnUpdatedScores -= OnLeaderboardUpdated;
    }

}
