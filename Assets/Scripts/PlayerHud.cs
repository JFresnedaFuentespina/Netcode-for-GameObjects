using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerHud : NetworkBehaviour
{
    private NetworkVariable<FixedString64Bytes> playerName =
        new NetworkVariable<FixedString64Bytes>();

    private bool overlaySet = false;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsServer)
        {
            playerName.Value = $"Player {OwnerClientId}";
        }
    }

    private void SetOverlay()
    {
        var localPlayerOverlay =
            GetComponentInChildren<TextMeshProUGUI>();

        if (localPlayerOverlay != null)
        {
            localPlayerOverlay.text = playerName.Value.ToString();
            overlaySet = true;
        }
    }

    private void Update()
    {
        if (!overlaySet && !playerName.Value.IsEmpty)
        {
            SetOverlay();
        }
    }
}