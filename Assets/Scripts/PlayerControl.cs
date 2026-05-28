using Unity.Netcode;
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

    private PlayerState lastState = PlayerState.Idle;

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

    private const float INPUT_THRESHOLD = 0.01f;
    private float sendRate = 0.05f;
    private float sendTimer;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
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
        if (characterController == null || animator == null)
            return;

        if (IsOwner)
        {
            ClientInput();
        }

        ClientMoveAndRotate();

        ClientVisuals();
    }

    private void ClientInput()
    {
        sendTimer += Time.deltaTime;

        if (sendTimer < sendRate)
            return;

        sendTimer = 0f;

        Vector3 inputRotation = new Vector3(
            0,
            Input.GetAxisRaw("Horizontal"),
            0
        );

        Vector3 direction = transform.TransformDirection(Vector3.forward);

        float forwardInput = Input.GetAxisRaw("Vertical");

        Vector3 inputPosition = direction * forwardInput;

        bool positionChanged =
            Vector3.Distance(oldInputPosition, inputPosition) >
            INPUT_THRESHOLD;

        bool rotationChanged =
            Vector3.Distance(oldInputRotation, inputRotation) >
            INPUT_THRESHOLD;

        if (positionChanged || rotationChanged)
        {
            oldInputPosition = inputPosition;
            oldInputRotation = inputRotation;

            UpdateClientPositionAndRotationServerRpc(
                inputPosition * speed,
                inputRotation * rotationSpeed
            );
        }

        PlayerState newState;

        if (forwardInput > 0)
        {
            newState = PlayerState.Walk;
        }
        else if (forwardInput < 0)
        {
            newState = PlayerState.ReverseWalk;
        }
        else
        {
            newState = PlayerState.Idle;
        }

        if (newState != lastState)
        {
            lastState = newState;
            UpdatePlayerStateServerRpc(newState);
        }
    }

    private void ClientMoveAndRotate()
    {
        if (characterController == null)
            return;

        characterController.SimpleMove(
            networkPositionDirection.Value
        );

        transform.Rotate(
            networkRotationDirection.Value
        );
    }

    private void ClientVisuals()
    {
        if (animator == null) return;

        if (networkPlayerState.Value == PlayerState.Walk)
        {
            animator.SetFloat("Walk", 1f);
        }
        else if (networkPlayerState.Value == PlayerState.ReverseWalk)
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
