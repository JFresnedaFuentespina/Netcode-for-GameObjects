using System.Threading.Tasks;
using DilmerGames.Core.Singletons;
using Unity.Networking.Transport.Relay;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Multiplayer;
using Unity.Netcode;
using TMPro;

public class RelayManager : Singleton<RelayManager>
{
    [SerializeField] private string environment = "production";
    [SerializeField] private int maxConnections = 10;
    private bool serveisInicialitzats = false;
    public TextMeshProUGUI joinCodeTextMeshPro;

    async void Start()
    {
        try
        {
            // 1. Inicialitza els serveis de Unity
            await UnityServices.InitializeAsync();

            // 2. Inicia sessió de forma anònima (obligatori per a Relay/Lobby)
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }

            serveisInicialitzats = true;
            Debug.Log("Unity Services i Autenticació preparats!");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error en la inicialització: {e.Message}");
        }
    }

    public async void StartHostWithMultiplayerSuite()
    {
        if (!serveisInicialitzats) return;

        var options = new SessionOptions
        {
            MaxPlayers = maxConnections
        };

        var session = await MultiplayerService.Instance.CreateSessionAsync(options);
        string joinCode = session.Code;
        Debug.Log($"Codigo para compartir:{joinCode}");
        joinCodeTextMeshPro.text = "Code: " + joinCode;

        if (NetworkManager.Singleton.StartHost())
        {
            Debug.Log("Host started successfully.");
        }
        else
        {
            Debug.LogError("Failed to start host.");
        }
    }

    public async void StartHostWithMultiplayerSuite2()
    {
        if (!serveisInicialitzats) return;
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            Debug.Log($"Código para compartir: {joinCode}");
            UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

            RelayServerData relayServerData = new RelayServerData(
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.ConnectionData,
                allocation.ConnectionData,
                allocation.Key,
                true,
                false
            );

            transport.SetRelayServerData(relayServerData);
            if (NetworkManager.Singleton.StartHost())
            {
                Debug.Log("Host started successfully.");
            }
            else
            {
                Debug.LogError("Failed to start host.");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
        }
    }

    public async void StartClientWithMultiplayerSuite(string joinCode)
    {
        if (!serveisInicialitzats) return;

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

    public async void StartClientWithMultiplayerSuite2(string joinCode)
    {
        if (!serveisInicialitzats) return;

        try
        {
            JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

            RelayServerData relayServerData = new RelayServerData(
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.ConnectionData,
                allocation.HostConnectionData,
                allocation.Key,
                true,
                false
            );

            transport.SetRelayServerData(relayServerData);

            if (NetworkManager.Singleton.StartClient())
            {
                Debug.Log("Client started successfully.");
            }
            else
            {
                Debug.LogError("Failed to start client.");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
        }
    }
}
