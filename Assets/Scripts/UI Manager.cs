using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    public Button startServerButton;

    [SerializeField]
    public Button startHostButton;

    [SerializeField]
    public Button startClientButton;
    [SerializeField]
    public Button executePhyisicsButton;

    [SerializeField]
    public TextMeshProUGUI playersInGameText;

    private bool hasServerStarted = false;

    void Awake()
    {
        // Cursor.visible = true;
        // Cursor.lockState = CursorLockMode.Confined;
    }

    void Start()
    {
        startHostButton.onClick.AddListener(StartHost);
        startServerButton.onClick.AddListener(StartServer);
        startClientButton.onClick.AddListener(StartClient);
        executePhyisicsButton.onClick.AddListener(ExecutePhysics);

        NetworkManager.Singleton.OnServerStarted += () =>
        {
            hasServerStarted = true;
        };
    }

    void StartHost()
    {
        if (NetworkManager.Singleton.StartHost())
        {
            Debug.Log("Host started successfully.");
        }
        else
        {
            Debug.LogError("Failed to start host.");
        }
    }
    void StartServer()
    {
        if (NetworkManager.Singleton.StartServer())
        {
            Debug.Log("Server started successfully.");
        }
        else
        {
            Debug.LogError("Failed to start server.");
        }
    }

    void StartClient()
    {
        if (NetworkManager.Singleton.StartClient())
        {
            Debug.Log("Client started successfully.");
        }
        else
        {
            Debug.LogError("Failed to start client.");
        }
    }

    void ExecutePhysics()
    {
        if (!hasServerStarted)
        {
            Debug.LogWarning("Physics execution attempted without a started server.");
            return;
        }

        SpawnerControl.Instance.SpawnObjects();
    }

    void Update()
    {
        playersInGameText.text = "Players in Game: " + PlayersManager.Instance.PlayersInGame();
    }
}
