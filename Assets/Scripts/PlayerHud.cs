using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerHud : NetworkBehaviour
{
    private NetworkVariable<NetworkString> playerName = new NetworkVariable<NetworkString>();

    private bool overlaySet = false;
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            playerName.Value = "Player " + OwnerClientId;
        }
    }

    public void SetOverlay()
    {
        var localPlayerOverlay = gameObject.GetComponentInChildren<TextMeshProUGUI>();
        localPlayerOverlay.text = playerName.Value;
        overlaySet = true;
    }

    void Update()
    {
        if (!overlaySet && !string.IsNullOrEmpty(playerName.Value))
        {
            SetOverlay();
        }
    }
}



