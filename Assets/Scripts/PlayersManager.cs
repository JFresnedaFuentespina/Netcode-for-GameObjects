using Unity.Netcode;
using DilmerGames.Core.Singletons;
using UnityEngine;

public class PlayersManager : NetworkSingleton<PlayersManager>
{
    private NetworkVariable<int> playersInGame = new NetworkVariable<int>(0);

    public int PlayersInGame()
    {
        return playersInGame.Value;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (NetworkManager.Singleton == null)
        {
            Debug.LogError("NetworkManager is null.");
            return;
        }

        if (IsServer)
        {
            Debug.Log("PlayersManager spawned on server.");

            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;

            // Cuenta el host al iniciar
            playersInGame.Value = NetworkManager.Singleton.ConnectedClientsList.Count;
        }
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        Debug.Log($"Client connected: {clientId}");

        playersInGame.Value = NetworkManager.Singleton.ConnectedClientsList.Count;

        Debug.Log($"Players in game: {playersInGame.Value}");
    }

    private void OnClientDisconnected(ulong clientId)
    {
        Debug.Log($"Client disconnected: {clientId}");

        playersInGame.Value = NetworkManager.Singleton.ConnectedClientsList.Count;

        Debug.Log($"Players in game: {playersInGame.Value}");
    }
}