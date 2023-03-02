using UnityEngine;
using MoralisUnity;
using MoralisUnity.EvmApi;

public class MoralisPlaygroundManager : MonoBehaviour
{
    public string apiKey;
    public string walletAddress;
    public string contractAddress;
    public ChainList chain;

    private Client _evmClient;
    
    void Start()
    {
        Moralis.Start(apiKey);
        _evmClient = Moralis.CreateEvmClient();
    }

    public async void GetNativeBalance()
    {
        var response = await _evmClient.GetNativeBalanceAsync(walletAddress, chain);
        Debug.Log(response.Balance);
    }
    
    public async void GetContractNfts()
    {
        var response = await _evmClient.GetContractNFTsAsync(contractAddress, chain);

        foreach (var nft in response.Result)
        {
            Debug.Log(nft.Metadata);
        }
    }
    
    public async void GetNftMetadata()
    {
        var response = await _evmClient.GetNFTMetadataAsync(contractAddress, "1", chain);
        Debug.Log(response.Metadata);
    }
    
    public async void GetWalletTransactions()
    {
        var response = await _evmClient.GetWalletTransactionsAsync(walletAddress, chain);

        foreach (var transaction in response.Result)
        {
            Debug.Log(transaction.Block_hash);
        }
    }
}
