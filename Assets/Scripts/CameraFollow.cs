using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target Setting")]
    public Transform target;

    [Header("Offset Setting")]
    [SerializeField] 
    private Vector3 position = new Vector3(0, 4, -3);
    [SerializeField]
    private Vector3 rotation = new Vector3(45, 0, 0);

    private Camera cameraComponent;
    private Vector3 targetPosition;

    void Start()
    {
        cameraComponent = GetComponent<Camera>();

        // 타겟이 없으면 플레이어 태그로 찾기
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
            else
            {
                Debug.LogWarning("CameraFollow: Can not find target!");
            }

            // 초기 위치 설정
            if (target != null)
            {
                transform.position = target.position + position;
                transform.rotation = Quaternion.Euler(rotation);
                transform.LookAt(target.position);
            }
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        // 타겟 위치 계산
        targetPosition = target.position + position;

        transform.position = targetPosition;
    }
}
