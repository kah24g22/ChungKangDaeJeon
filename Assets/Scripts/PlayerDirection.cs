using UnityEngine;

public class PlayerDirection : MonoBehaviour
{
    [Header("Camera Setting")]
    public Camera playerCamera;

    [Header("Debug")]
    public bool showDebugRay = false;

    private Vector3 targetDirection;

    void Start()
    {
        // 카메라가 할당되지 않았다면 메인 카메라를 사용
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }

        // 카메라를 찾을 수 없다면 경고
        if (playerCamera == null)
        {
            Debug.LogWarning("PlayerDirection: Can't find camera!");
        }
    }

    void Update()
    {
        // 카메라가 없으면 실행하지 않음
        if (playerCamera == null) return;

        // 마우스 위치를 월드 좌표로 변환
        Vector3 mouseWorldPosition = GetMouseWorldPosition();

        // 캐릭터에서 마우스 방향으로의 벡터 계산
        Vector3 directionToMouse = (mouseWorldPosition - transform.position).normalized;

        // Y축 회전만 적용 (쿼터뷰에서는 위아래 회전 불필요)
        directionToMouse.y = 0f;

        // 방향이 유효한지 확인
        if (directionToMouse.magnitude > 0.1f)
        {
            targetDirection = directionToMouse;

            // 즉시 회전
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            transform.rotation = targetRotation;
        }

        // 디버그 레이 표시
        if (showDebugRay)
        {
            Debug.DrawRay(transform.position, targetDirection * 3f, Color.red);
            Debug.DrawLine(transform.position, mouseWorldPosition, Color.blue);
        }
    }

    Vector3 GetMouseWorldPosition()
    {
        // 마우스 스크린 좌표
        Vector3 mouseScreenPosition = Input.mousePosition;

        // 캐릭터와 같은 높이의 평면으로 레이캐스트
        Plane groundPlane = new Plane(Vector3.up, transform.position.y);
        Ray cameraRay = playerCamera.ScreenPointToRay(mouseScreenPosition);

        // 레이와 평면의 교차점 계산
        if (groundPlane.Raycast(cameraRay, out float distance))
        {
            return cameraRay.GetPoint(distance);
        }

        // 교차점을 찾을 수 없으면 캐릭터 앞쪽 반환
        return transform.position + transform.forward;
    }

    void OnDrawGizmosSelected()
    {
        // 씬 뷰에서 방향 표시
        if (Application.isPlaying && targetDirection != Vector3.zero)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, transform.position + targetDirection * 2f);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position + targetDirection * 2f, 0.2f);
        }
    }
}
