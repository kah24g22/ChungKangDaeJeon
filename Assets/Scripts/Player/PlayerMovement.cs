using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private PlayerStatusManager status;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private PlayerDash playerDash; // PlayerDash 스크립트 참조

    [Header("Animation Settings")]
    public float animationSmoothTime = 0.1f;

    private Vector2 moveInput;
    // moveDirection을 외부에서 읽을 수 있도록 public 프로퍼티로 변경합니다.
    public Vector3 moveDirection { get; private set; }

    // 부드러운 애니메이션 전환을 위한 변수
    private float currentVelocityX = 0f;
    private float currentVelocityZ = 0f;
    private float velocityXSmooth = 0f;
    private float velocityZSmooth = 0f;

    void Awake()
    {
        // 컴포넌트 자동 할당
        if (status == null) status = GetComponent<PlayerStatusManager>();
        if (characterController == null) characterController = GetComponent<CharacterController>();
        if (animator == null) animator = GetComponent<Animator>();
        if (playerDash == null) playerDash = GetComponent<PlayerDash>();
        if (cameraTransform == null) cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        // 대시 중에는 이동 및 애니메이션 처리를 하지 않음
        if (playerDash != null && playerDash.isDashing)
        {
            // 대시 중에는 이동 애니메이션을 끔
            animator.SetBool("isRun", false);
            return;
        }

        HandleMovement();
        HandleAnimation();
    }

    // Input System에 의해 호출될 메서드
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    private void HandleMovement()
    {
        // 카메라 기준 방향 계산
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        moveDirection = (forward * moveInput.y + right * moveInput.x).normalized;

        // CharacterController를 이용한 이동
        Vector3 move = moveDirection * status.data.speed * Time.deltaTime;
        characterController.Move(move);
    }

    private void HandleAnimation()
    {
        float targetVelocityX = 0f;
        float targetVelocityZ = 0f;

        if (moveDirection.sqrMagnitude > 0.01f)
        {
            // 월드 방향을 로컬 방향으로 변환
            Vector3 localMove = transform.InverseTransformDirection(moveDirection);
            targetVelocityX = localMove.x;
            targetVelocityZ = localMove.z;
        }

        // SmoothDamp를 이용해 부드럽게 값 변경
        currentVelocityX = Mathf.SmoothDamp(currentVelocityX, targetVelocityX, ref velocityXSmooth, animationSmoothTime);
        currentVelocityZ = Mathf.SmoothDamp(currentVelocityZ, targetVelocityZ, ref velocityZSmooth, animationSmoothTime);

        animator.SetFloat("velocityX", currentVelocityX);
        animator.SetFloat("velocityZ", currentVelocityZ);
        animator.SetBool("isRun", moveDirection != Vector3.zero);
    }
}