using NUnit.Framework;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerControl : NetworkBehaviour
{
    [SerializeField] private float walkSpeed = 3.5f;
    [SerializeField] private Vector2 defaultPositionRange = new Vector2(-4f, 4f);

    // variables del servidor
    [SerializeField] private NetworkVariable<float> forwardBackPosition = new NetworkVariable<float>();
    [SerializeField] private NetworkVariable<float> leftRightPosition = new NetworkVariable<float>();

    // client caching
    private float oldForwardBackPosition;
    private float oldLeftRightPosition;

    void Start()
    {
        transform.position = new Vector3(
            Random.Range(defaultPositionRange.x, defaultPositionRange.y),
            0f,
            Random.Range(defaultPositionRange.x, defaultPositionRange.y)
        );
    }

    void Update()
    {
        if (IsServer)
        {
            UpdateServer();
        }
        if (IsClient && IsOwner)
        {
            UpdateClient();
        }
    }

    private void UpdateServer()
    {
        transform.position = new Vector3(transform.position.x + leftRightPosition.Value, transform.position.y, transform.position.z + forwardBackPosition.Value);
    }

    private void UpdateClient()
    {
        float forwardBackward = 0f;
        float leftRight = 0f;

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            forwardBackward += walkSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            forwardBackward -= walkSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            leftRight -= walkSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            leftRight += walkSpeed * Time.deltaTime;
        }

        if (oldForwardBackPosition != forwardBackward || oldLeftRightPosition != leftRight)
        {
            oldForwardBackPosition = forwardBackward;
            oldLeftRightPosition = leftRight;
            // Update the server variables
            UpdateServerRpc(forwardBackward, leftRight);
        }
    }

    [ServerRpc]
    private void UpdateServerRpc(float forwardBackward, float leftRight)
    {
        forwardBackPosition.Value = forwardBackward;
        leftRightPosition.Value = leftRight;
    }
}
