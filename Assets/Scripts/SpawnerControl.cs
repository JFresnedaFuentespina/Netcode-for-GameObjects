using DilmerGames.Core.Singletons;
using Netcode.Extensions;
using Unity.Netcode;
using UnityEngine;

public class SpawnerControl : NetworkSingleton<SpawnerControl>
{
    [SerializeField] private GameObject objectPrefab;
    [SerializeField] private int maxObjectInstanceCount = 3;

    void Awake()
    {
    }

    void Start()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnServerStarted += () =>
            {
                NetworkObjectPool.Instance.InitializePool();
            };
        }
    }

    public void SpawnObjects()
    {
        if (!IsServer) return;

        for (int i = 0; i < maxObjectInstanceCount; i++)
        {
            // GameObject obj = Instantiate(objectPrefab, new Vector3(Random.Range(-10f, 10f), 10.0f, Random.Range(-10f, 10f)), Quaternion.identity);
            // obj.GetComponent<NetworkObject>().Spawn();
            // pool instantiation
            GameObject obj = NetworkObjectPool.Instance.GetNetworkObject(objectPrefab).gameObject;
            obj.transform.position = new Vector3(Random.Range(-10f, 10f), 10.0f, Random.Range(-10f, 10f));
            obj.GetComponent<NetworkObject>().Spawn();
        }
    }
}
