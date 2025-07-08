using UnityEngine;

public class PlayerDirection : MonoBehaviour
{
    [Header("Camera Setting")]
    public Camera playerCamera; // 에디터에서 할당 

    [Header("Debug")]
    public bool showDebugRay = false;

    public Vector3 aimDirection { get; private set; }

    void Start()
    {
        // 카메라가 인스펙터에서 할당되었는지 확인합니다.
        if (playerCamera == null)
        {
            Debug.LogError("PlayerDirection: Player Camera is not assigned in the inspector!", this);
            this.enabled = false;
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
            aimDirection = directionToMouse;

            // 즉시 회전
            Quaternion targetRotation = Quaternion.LookRotation(aimDirection);
            transform.rotation = targetRotation;
        }

        // 디버그 레이 표시
        if (showDebugRay)
        {
            Debug.DrawRay(transform.position, aimDirection * 3f, Color.red);
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
        if (Application.isPlaying && aimDirection != Vector3.zero)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, transform.position + aimDirection * 2f);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position + aimDirection * 2f, 0.2f);
        }
    }
}
