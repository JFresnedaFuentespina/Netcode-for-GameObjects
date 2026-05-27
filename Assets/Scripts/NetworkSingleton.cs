namespace DilmerGames.Core.Singletons
{
    using Unity.Netcode;
    using UnityEngine;

    /// <summary>
    /// Base class for a singleton NetworkBehaviour.
    /// </summary>
    /// <typeparam name="T">The type of the singleton.</typeparam>
    public class NetworkSingleton<T> : NetworkBehaviour where T : NetworkBehaviour
    {
        private static T _instance;
        public static T Instance
        {
            get
            {
                if(_instance == null)
                {
                    var objs = FindObjectsByType<T>(FindObjectsSortMode.None);
                    if(objs.Length > 0)
                    {
                        _instance = objs[0];
                    }   
                    if(objs.Length > 1)
                    {
                        Debug.LogWarning("Multiple instances of singleton " + typeof(T).Name + " found. Using the first one.");
                    }
                    if(_instance == null)
                    {
                        GameObject obj = new GameObject();
                        obj.name = string.Format("_{0}", typeof(T).Name);
                        _instance = obj.AddComponent<T>();
                    }
                }
                return _instance;
            }
        }
    }
}