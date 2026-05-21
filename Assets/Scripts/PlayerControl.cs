using NUnit.Framework;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerControl : NetworkBehaviour
{
    public enum PlayerState
    {
        Idle,
        Walk,
        ReverseWalk,
        Jumping
    }

    [SerializeField] private float speed = 3.5f;
    [SerializeField] private float rotationSpeed = 1.5f;
    [SerializeField] private Vector2 defaultInitialPlanePosition = new Vector2(-4f, 4f);

    // variables del servidor
    [SerializeField] NetworkVariable<Vector3> networkPositionDirection = new NetworkVariable<Vector3>();
    [SerializeField] NetworkVariable<Vector3> networkRotationDirection = new NetworkVariable<Vector3>();
    [SerializeField] NetworkVariable<PlayerState> networkPlayerState = new NetworkVariable<PlayerState>();

    // client caching
    private Vector3 oldInputPosition;
    private Vector3 oldInputRotation;
    public CharacterController characterController;
    public Animator animator;

    void Awake()
    {
        // characterController = GetComponent<CharacterController>();
        // animator = GetComponent<Animator>();
    }

    void Start()
    {
        if (IsClient && IsOwner)
        {
            transform.position = new Vector3(
                Random.Range(defaultInitialPlanePosition.x, defaultInitialPlanePosition.y),
                0f,
                Random.Range(defaultInitialPlanePosition.x, defaultInitialPlanePosition.y)
            );
        }
    }

    void Update()
    {
        if (IsClient && IsOwner)
        {
            ClientInput();
        }
        ClientMoveAndRotate();
        ClientVisuals();
    }

    private void ClientInput()
    {
        // Player position & rotation changes
        Vector3 inputRotation = new Vector3(0, Input.GetAxis("Horizontal"), 0);

        Vector3 direction = transform.TransformDirection(Vector3.forward);
        float forwardInput = Input.GetAxis("Vertical");
        Vector3 inputPosition = direction * forwardInput;

        if (oldInputPosition != inputPosition || oldInputRotation != inputRotation)
        {
            oldInputPosition = inputPosition;
            oldInputRotation = inputRotation;
            UpdateClientPositionAndRotationServerRpc(inputPosition * speed, inputRotation * rotationSpeed);
        }

        // Player state changes
        if (forwardInput > 0)
        {
            UpdatePlayerStateServerRpc(PlayerState.Walk);
        }
        else if (forwardInput < 0)
        {
            UpdatePlayerStateServerRpc(PlayerState.ReverseWalk);
        }
        else
        {
            UpdatePlayerStateServerRpc(PlayerState.Idle);
        }
    }

    private void UpdateClient()
    {

    }

    private void ClientMoveAndRotate()
    {
        if (networkPositionDirection.Value != Vector3.zero)
        {
            characterController.SimpleMove(networkPositionDirection.Value);
        }
        if (networkRotationDirection.Value != Vector3.zero)
        {
            transform.Rotate(networkRotationDirection.Value);
        }
    }

    private void ClientVisuals()
    {
        if(networkPlayerState.Value == PlayerState.Walk)
        {
            animator.SetFloat("Walk", 1f);
        }
        else if(networkPlayerState.Value == PlayerState.ReverseWalk)
        {
            animator.SetFloat("Walk", -1f);
        }
        else
        {
            animator.SetFloat("Walk", 0f);
        }
    }

    [ServerRpc]
    private void UpdateClientPositionAndRotationServerRpc(Vector3 newPositionDirection, Vector3 newRotationDirection)
    {
        networkPositionDirection.Value = newPositionDirection;
        networkRotationDirection.Value = newRotationDirection;
    }

    [ServerRpc]
    private void UpdatePlayerStateServerRpc(PlayerState newState)
    {
        networkPlayerState.Value = newState;
    }
}
