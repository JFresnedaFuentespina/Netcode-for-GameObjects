using Unity.Netcode;
using UnityEngine;

namespace DilmerGames.Core.Singletons
{
    public class NetworkSingleton<T> : NetworkBehaviour where T : NetworkBehaviour
    {
        private static T _instance;

        public static T Instance => _instance;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (_instance == null)
            {
                _instance = this as T;
            }
            else if (_instance != this)
            {
                Debug.LogWarning($"Duplicate singleton {typeof(T).Name}");
                Destroy(gameObject);
            }
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();

            if (_instance == this)
            {
                _instance = null;
            }
        }
    }
}