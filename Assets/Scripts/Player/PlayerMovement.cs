using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Player Status")]
    public PlayerStatusManager status;

    [Header("Animation Settings")]
    public float animationSmoothTime = 0.1f;

    [Header("Camera")]
    public Transform cameraTransform;

    [Header("Components")]
    public Animator animator;

    private LayerMask playerLayer = 6; // Player Layer (인덱스 5)
    private LayerMask dashingLayer = 7; // PlayerDashing Layer (인덱스 6)

    private CharacterController characterController;
    private Vector2 moveInput;
    private Vector3 moveDirection;
    private Vector3 localMoveDirection;

    // 대시 관련 변수
    private bool isDashing = false;
    private float dashTimer = 0f;
    private float dashCooldownTimer = 0f;
    private Vector3 dashDirection;

    // 부드러운 애니메이션을 위한 변수들
    private float currentVelocityX = 0f;
    private float currentVelocityZ = 0f;
    private float velocityXSmooth = 0f;
    private float velocityZSmooth = 0f;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        if (characterController == null)
        {
            Debug.LogError("CharacterController component is missing on this object!", this);
            this.enabled = false; // 스크립트 비활성화
            return;
        }

        // 카메라 Transform이 할당되었는지 확인합니다.
        if (cameraTransform == null)
        {
            Debug.LogError("PlayerMovement: Camera Transform is not assigned in the inspector!", this);
            this.enabled = false;
            return;
        }

        if (animator == null)
            animator = GetComponent<Animator>();
    }

    void Update()
    {
        HandleDash();
        HandleMovement();
        HandleAnimation();
        UpdateCooldowns();
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnDash(InputValue value)
    {
        if (value.isPressed && CanDash())
        {
            StartDash();
        }
    }

    void HandleDash()
    {
        if (isDashing)
        {
            dashTimer -= Time.deltaTime;

            // 대시 이동
            Vector3 dashMove = dashDirection * status.data.dashSpeed * Time.deltaTime;
            characterController.Move(dashMove);

            // 대시 종료 체크
            if (dashTimer <= 0f)
            {
                EndDash();
            }
        }
    }

    void StartDash()
    {
        // EP 소모 시도
        if (!UseDashEp())
        {
            Debug.Log("Not enough EP for dash!");
            return;
        }

        isDashing = true;                    // 대시 상태 활성화
        dashTimer = status.data.dashDuration;           // 대시 지속 시간 설정
        dashCooldownTimer = status.data.dashCooldown;   // 쿨다운 시작

        // 대시 레이어로 변경 (적과 충돌 무시)
        SetPlayerLayer(dashingLayer);

        // 이동 방향이 있으면 그 방향으로, 없으면 현재 바라보는 방향으로 대시
        if (moveDirection != Vector3.zero)
        {
            dashDirection = moveDirection.normalized;
        }
        else
        {
            // 현재 바라보는 방향으로 대시 (Y축 제거하여 수평 이동만)
            Vector3 forwardDirection = transform.forward;
            forwardDirection.y = 0;
            dashDirection = forwardDirection.normalized;
        }
    }

    public bool CanUseDash()
    {
        return status.data.curEp >= status.data.dashEpCost;
    }

    public bool UseDashEp()
    {
        if (CanUseDash())
        {
            status.UseEp(status.data.dashEpCost);
            return true;
        }
        return false;
    }

    void EndDash()
    {
        isDashing = false;

        // 일반 레이어로 복구 (적과 다시 충돌)
        SetPlayerLayer(playerLayer);
        Debug.Log($"Dash ended! Layer restored to: {LayerMask.LayerToName(playerLayer)} (Index: {playerLayer})");
    }

    void SetPlayerLayer(LayerMask layerIndex)
    {
        // 플레이어의 모든 자식 오브젝트들의 레이어도 함께 변경
        SetLayerRecursively(gameObject, layerIndex);
        Debug.Log($"Player layer set to: {LayerMask.LayerToName(layerIndex)} (Index: {layerIndex})");
    }

    void SetLayerRecursively(GameObject obj, LayerMask layerIndex)
    {
        obj.layer = layerIndex;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, layerIndex);
        }
    }

    bool CanDash()
    {
        // 대시 중이 아니고 쿨다운이 끝났으면 언제든 대시 가능
        return !isDashing && dashCooldownTimer <= 0f && CanUseDash();
    }

    void UpdateCooldowns()
    {
        if (dashCooldownTimer > 0f)
        {
            dashCooldownTimer -= Time.deltaTime;
        }
    }

    void HandleMovement()
    {
        // 대시 중에는 일반 이동 무시
        if (isDashing) return;

        // 카메라 기준 방향 벡터
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        // Y축 제거 (수평 이동만)
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        moveDirection = (forward * moveInput.y + right * moveInput.x).normalized;

        // 로컬 이동 방향 계산 (플레이어 바라보는 방향 기준)
        if (moveDirection != Vector3.zero)
        {
            localMoveDirection = transform.InverseTransformDirection(moveDirection);
        }
        else
        {
            localMoveDirection = Vector3.zero;
        }

        Vector3 move = moveDirection * status.data.speed * Time.deltaTime;
        characterController.Move(move);
    }

    void HandleAnimation()
    {
        if (animator != null)
        {
            // 대시 중에는 애니메이션 업데이트 생략
            if (isDashing) return;

            // 목표 값 설정
            float targetVelocityX = 0f;
            float targetVelocityZ = 0f;

            if (moveInput != Vector2.zero)
            {
                Vector3 localMove = transform.InverseTransformDirection(moveDirection);
                targetVelocityX = localMove.x;
                targetVelocityZ = localMove.z;
            }

            // SmoothDamp를 사용한 부드러운 전환
            currentVelocityX = Mathf.SmoothDamp(currentVelocityX, targetVelocityX,
                ref velocityXSmooth, animationSmoothTime);
            currentVelocityZ = Mathf.SmoothDamp(currentVelocityZ, targetVelocityZ,
                ref velocityZSmooth, animationSmoothTime);

            animator.SetFloat("velocityX", currentVelocityX);
            animator.SetFloat("velocityZ", currentVelocityZ);
            animator.SetBool("isRun", moveDirection != Vector3.zero);
        }
    }
}
