using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerControl : NetworkBehaviour
{
    [SerializeField]
    private float walkSpeed = 3.5f;

    [SerializeField]
    private Vector2 defaultPositionRange = new Vector2(-4, 4);

    [SerializeField]
    private NetworkVariable<float> forwardBackPosition = new NetworkVariable<float>();

    [SerializeField]
    private NetworkVariable<float> leftRightPosition = new NetworkVariable<float>();

    // client caching
    private float oldForwardBackPosition;
    private float oldLeftRightPosition;

    void Start()
    {
        transform.position = new Vector3(Random.Range(defaultPositionRange.x, defaultPositionRange.y), 0, Random.Range(defaultPositionRange.x, defaultPositionRange.y));
    }

    void Update()
    {
        if (IsServer)
        {
            UpdateServer();
        }
        else if (IsClient)
        {
            UpdateClient();
        }
    }

    void UpdateServer()
    {
        transform.position = new Vector3(transform.position.x + leftRightPosition.Value,
                                        transform.position.y,
                                        transform.position.z + forwardBackPosition.Value);
    }

    void UpdateClient()
    {
        float forwardBackward = 0;
        float leftRight = 0;

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
            // Update the server
            UpdateClientPositionServerRpc(forwardBackward, leftRight);
        }

    }

    [ServerRpc]
    public void UpdateClientPositionServerRpc(float forwardBackward, float leftRight)
    {
        forwardBackPosition.Value = forwardBackward;
        leftRightPosition.Value = leftRight;
    }


}
