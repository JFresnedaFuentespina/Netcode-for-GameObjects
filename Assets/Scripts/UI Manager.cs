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
    public TextMeshProUGUI playersInGameText;

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

    void Update()
    {
        playersInGameText.text = "Players in Game: " + PlayersManager.Instance.PlayersInGame();
    }
}
