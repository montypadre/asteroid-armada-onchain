using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using Nethereum.ABI.Encoders;
using Nethereum.Signer;
using Nethereum.Hex.HexConvertors.Extensions;
using UnityEngine;

public class PingContractService
{
    // We define the ABI of the contract we are going to use.
    public static string ABI = @"[{""inputs"":[],""stateMutability"":""nonpayable"",""type"":""constructor""},{ ""inputs"":[{ ""internalType"":""string"",""name"":""user"",""type"":""string""},{ ""internalType"":""uint256"",""name"":""score"",""type"":""uint256""}],""name"":""addScore"",""outputs"":[{ ""internalType"":""bool"",""name"":"""",""type"":""bool""}],""stateMutability"":""nonpayable"",""type"":""function""},{ ""inputs"":[{ ""internalType"":""uint256"",""name"":"""",""type"":""uint256""}],""name"":""leaderboard"",""outputs"":[{ ""internalType"":""string"",""name"":""user"",""type"":""string""},{ ""internalType"":""uint256"",""name"":""score"",""type"":""uint256""}],""stateMutability"":""view"",""type"":""function""}]";

    // And we define the contract address here, in this case is a simple ping contract
    // (Remember this contract is deployed on the Binance Smart Chain testnet
    private static string contractAddress = "0x78f170c05b76cc53399410e952f2bd7b7ec59078";

    // We define a new contract (Nethereum.Contracts)
    private Contract contract;

    public PingContractService()
    {
        // Here we assign the contract as a new contract and we send it the ABI and contract address
        this.contract = new Contract(null, ABI, contractAddress);
        
    }

    public Function GetPingFunction()
    {
        return contract.GetFunction("addScore");
    }

    public TransactionInput CreatePingTransactionInput (
        // For this transaction to the acontract we are going to use
        // the address which is executing the transaction (addressFrom),
        // the private key of that address (privateKey),
        // the ping value we are going to send to this contract (pingValue),
        // the maximum amount of gas to consume,
        // the price you are willing to pay per each unit of gas consumed, (higher the price, faster the tx will be included)
        // and the valueAmount in ETH to send to this contract.
        // IMPORTANT: the PingContract doesn't accept eth transfers to this must be 0 or it will throw an error.
        string addressFrom,
        string privateKey,
        BigInteger pingValue,
        HexBigInteger gas = null,
        HexBigInteger gasPrice = null,
        HexBigInteger valueAmount = null)
    {
        var function = GetPingFunction();
        return function.CreateTransactionInput(addressFrom, gas, gasPrice, valueAmount, pingValue);
    }
}
