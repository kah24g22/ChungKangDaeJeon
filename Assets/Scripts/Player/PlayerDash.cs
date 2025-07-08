using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDash : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private PlayerStatusManager status;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private PlayerMovement playerMovement; // PlayerMovement 참조 추가

    [Header("Dash Settings")]
    [Tooltip("플레이어의 일반 상태 레이어 번호 (예: 6)")]
    [SerializeField] private int playerLayer = 6;
    [Tooltip("대시 중일 때 사용될 레이어 번호 (예: 7)")]
    [SerializeField] private int dashingLayer = 7;

    public bool isDashing { get; private set; } = false;
    private float dashTimer = 0f;
    private float dashCooldownTimer = 0f;
    private Vector3 dashDirection;

    void Awake()
    {
        if (status == null) status = GetComponent<PlayerStatusManager>();
        if (characterController == null) characterController = GetComponent<CharacterController>();
        // PlayerMovement 컴포넌트 참조를 가져옵니다.
        if (playerMovement == null) playerMovement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        HandleDash();
        UpdateCooldowns();
    }

    public void OnDash(InputValue value)
    {
        if (value.isPressed && CanDash())
        {
            StartDash();
        }
    }

    private void HandleDash()
    {
        if (isDashing)
        {
            dashTimer -= Time.deltaTime;
            Vector3 dashMove = dashDirection * status.data.dashSpeed * Time.deltaTime;
            characterController.Move(dashMove);

            if (dashTimer <= 0f)
            {
                EndDash();
            }
        }
    }

    private void StartDash()
    {
        if (!status.UseEp(status.data.dashEpCost))
        {
            Debug.Log("Not enough EP for dash!");
            return;
        }

        isDashing = true;
        dashTimer = status.data.dashDuration;
        dashCooldownTimer = status.data.dashCooldown;

        SetPlayerLayer(dashingLayer);

        // --- 대시 방향 결정 로직 변경 ---
        // 이동 입력이 있는지 확인합니다.
        if (playerMovement.moveDirection.sqrMagnitude > 0.01f)
        {
            // 이동 방향으로 대시합니다.
            dashDirection = playerMovement.moveDirection;
        }
        else
        {
            // 이동 입력이 없으면(제자리에 서 있으면) 현재 바라보는 정면 방향으로 대시합니다.
            Vector3 forwardDirection = transform.forward;
            forwardDirection.y = 0;
            dashDirection = forwardDirection.normalized;
        }
    }

    private void EndDash()
    {
        isDashing = false;
        SetPlayerLayer(playerLayer);
    }

    private bool CanDash()
    {
        return !isDashing && dashCooldownTimer <= 0f && status.CanUseEp(status.data.dashEpCost);
    }

    private void UpdateCooldowns()
    {
        if (dashCooldownTimer > 0f)
        {
            dashCooldownTimer -= Time.deltaTime;
        }
    }

    private void SetPlayerLayer(int layerIndex)
    {
        if (layerIndex < 0 || layerIndex > 31)
        {
            Debug.LogWarning($"SetPlayerLayer: Invalid layer index {layerIndex}. Make sure it's between 0 and 31.");
            return;
        }
        gameObject.layer = layerIndex;
        foreach (Transform child in transform)
        {
            SetLayerRecursively(child.gameObject, layerIndex);
        }
    }

    private void SetLayerRecursively(GameObject obj, int layerIndex)
    {
        obj.layer = layerIndex;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, layerIndex);
        }
    }
}