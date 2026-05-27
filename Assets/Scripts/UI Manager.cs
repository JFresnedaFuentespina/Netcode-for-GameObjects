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

    [SerializeField]
    public TMP_InputField joinCodeInputField;

    private bool hasServerStarted = false;

    void Awake()
    {
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

    async void StartHost()
    {
        RelayManager.Instance.StartHostWithMultiplayerSuite();
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

    async void StartClient()
    {
        if (!string.IsNullOrEmpty(joinCodeInputField.text))
        {
            RelayManager.Instance.StartClientWithMultiplayerSuite(joinCodeInputField.text);
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
