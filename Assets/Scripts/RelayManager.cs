using System.Threading.Tasks;
using DilmerGames.Core.Singletons;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using Unity.Services.Multiplayer;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class RelayManager : Singleton<RelayManager>
{
    [SerializeField] private string environment = "production";
    [SerializeField] private int maxConnections = 10;

    public async void StartHostWithMultiplayerSuite()
    {
        var options = new SessionOptions
        {
            MaxPlayers = maxConnections
        };

        var session = await MultiplayerService.Instance.CreateSessionAsync(options);
        string joinCode = session.Code;
        Debug.Log($"Codigo para compartir:{joinCode}");

        if (NetworkManager.Singleton.StartHost())
        {
            Debug.Log("Host started successfully.");
        }
        else
        {
            Debug.LogError("Failed to start host.");
        }
    }

    public async void StartClientWithMultiplayerSuite(string joinCode)
    {
        var session = await MultiplayerService.Instance.JoinSessionByCodeAsync(joinCode);
        if (NetworkManager.Singleton.StartClient())
        {
            Debug.Log("Client started successfully.");
        }
        else
        {
            Debug.LogError("Failed to start client.");
        }
    }
}
