using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using Nethereum RPC
using Nethereum.JsonRpc.UnityClient;
// using contract definition
using LeaderboardContract.Contracts.Leaderboard.ContractDefinition;

public class Leaderboard : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(FetchHighScores());
    }

   private IEnumerator FetchHighScores()
    {
        // connect to bsc testnet
        string url = "https://data-seed-prebsc-1-s2.binance.org:8545/";
        // leadereboard contract address
        string contractAddress = "0x78F170C05B76Cc53399410e952F2BD7b7EC59078";
        // fetch highest score
        //int leaderboardIndex = 0;
        var queryRequest = new QueryUnityRequest<LeaderboardFunction, LeaderboardOutputDTOBase>(url, contractAddress);
        // call LeaderboardFunctionBase with 1 param (leaderboardIndex) to get the user name score 
        for (int i = 0; i <= 2; i++)
        {
            yield return queryRequest.Query(new LeaderboardFunction() { ReturnValue1 = i }, contractAddress);
            // print in console
            print(queryRequest.Result.User);
            print(queryRequest.Result.Score);
        }
    }
}
