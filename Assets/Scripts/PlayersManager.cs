using Unity.Netcode;
using DilmerGames.Core.Singletons;
using UnityEngine;

public class PlayersManager : NetworkSingleton<PlayersManager>
{
    private NetworkVariable<int> playerInGame = new NetworkVariable<int>();

    public int PlayersInGame()
    {
        return playerInGame.Value;
    }

    void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += (id) =>
        {
            if(NetworkManager.Singleton.IsServer)
            {
                Debug.Log("Client connected: " + id);
                playerInGame.Value++;
            }
        };
        NetworkManager.Singleton.OnClientDisconnectCallback += (id) =>
        {
            if(NetworkManager.Singleton.IsServer)
            {
                Debug.Log("Client disconnected: " + id);
                playerInGame.Value--;
            }
        };
    }
}
