using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoralisUnity.Tools;
using MoralisUnity.EvmApi;
using MoralisUnity.StreamsApi;

namespace MoralisUnity
{
    public static class Moralis
    {
        private static string _apiKey;
    
        public static void Start(string apiKey)
        {
            _apiKey = apiKey;
        }

        public static string GetApiKey()
        {
            if (!string.IsNullOrEmpty(_apiKey)) return _apiKey;
        
            Debug.LogWarning("You have not set the Moralis Api Key. Use 'Moralis.Start()' to do so");
            return null;
        }
        
        public static EvmApi.Client CreateEvmClient()
        {
            UwrHttpClient httpClient = new UwrHttpClient();
            httpClient.DefaultRequestHeaders.Add("X-API-Key", GetApiKey());

            var evmClient = new EvmApi.Client(httpClient);
            return evmClient;
        }
        
        public static StreamsApi.Client CreateStreamsClient()
        {
            UwrHttpClient httpClient = new UwrHttpClient();
            httpClient.DefaultRequestHeaders.Add("X-API-Key", GetApiKey());

            var streamsClient = new StreamsApi.Client(httpClient);
            return streamsClient;
        }
    }   
}
