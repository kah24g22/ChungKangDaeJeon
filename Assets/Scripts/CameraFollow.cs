using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target Setting")]
    // 인스펙터에서 플레이어 또는 추적할 대상을 직접 할당합니다.
    public Transform target;

    [Header("Offset Setting")]
    [SerializeField]
    private Vector3 position = new Vector3(0, 4, -3);
    [SerializeField]
    private Vector3 rotation = new Vector3(45, 0, 0);

    private Vector3 targetPosition;

    void Start()
    {
        // 인스펙터에서 Target이 할당되었는지 확인합니다.
        if (target == null)
        {
            Debug.LogError("CameraFollow: Target is not assigned in the inspector!", this);
            this.enabled = false; // Target이 없으면 스크립트를 비활성화합니다.
            return;
        }

        // 초기 위치 설정
        transform.position = target.position + position;
        transform.rotation = Quaternion.Euler(rotation);
    }

    void LateUpdate()
    {
        if (target == null) return;

        // 타겟 위치 추적
        targetPosition = target.position + position;
        transform.position = targetPosition;
    }
}
