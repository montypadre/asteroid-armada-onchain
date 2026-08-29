using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
// using Nethereum RPC
using Nethereum.JsonRpc.UnityClient;
// using contract definition
using LeaderboardContract.Contracts.Leaderboard.ContractDefinition;

public class HighScoreController : MonoBehaviour
{
    public Text[] names;
    public Text[] scores;
    string url;
    string contractAddress;

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
        url = "https://data-seed-prebsc-1-s2.binance.org:8545/";
        contractAddress = "0x8c8559286612050B75a232e1ccDA5bEC8d771a5b";

        StartCoroutine(FetchHighScores());
    }

    private IEnumerator FetchHighScores()
    {
        while (true)
        {
            var queryRequest = new QueryUnityRequest<LeaderboardFunction, LeaderboardOutputDTOBase>(url, contractAddress);
            // call LeaderboardFunctionBase with 1 param (leaderboardIndex) to get the user name & score 
            for (int i = 0; i <= 4; i++)
            {
                yield return queryRequest.Query(new LeaderboardFunction() { ReturnValue1 = i }, contractAddress);
                names[i].text = queryRequest.Result.User;
                scores[i].text = queryRequest.Result.Score.ToString();
            }
        }
    }
}
