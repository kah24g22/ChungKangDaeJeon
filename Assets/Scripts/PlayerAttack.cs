using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    public Animator animator;

    private readonly int attackTriggerHash = Animator.StringToHash("Attack");
    private const string ATTACK_LAYER_NAME = "Attack Layer"; // 공격 애니메이션이 있는 레이어의 이름
    private int attackLayerIndex = -1;

    // 공격 상태와 입력 스택 문제를 해결하기 위한 플래그
    public bool isAttacking = false;

    [Header("Melee Range")]
    [SerializeField] private float radius = 1.5f; // 부채꼴의 반지름 (최대 거리)
    [Range(0, 360)]
    [SerializeField] private float angle = 90f;  // 부채꼴의 각도 (시야각)

    [Header("Target Setting")]
    [SerializeField] private LayerMask enemyLayer = 9;   // 감지할 타겟의 레이어 (예: "Enemy")
    [SerializeField] private LayerMask objectLayer = 11;   // 감지할 타겟의 레이어 (예: "Enemy")

    // 범위 내에 감지된 타겟들을 저장할 리스트
    public List<Transform> enemyList = new List<Transform>();

    void Awake()
    {
        // 게임 시작 시 레이어 인덱스를 미리 찾아 저장해 둡니다.
        attackLayerIndex = animator.GetLayerIndex(ATTACK_LAYER_NAME);
        if (attackLayerIndex == -1)
        {
            Debug.LogError($"애니메이터에 '{ATTACK_LAYER_NAME}' 레이어가 존재하지 않습니다!");
            // 이 컴포넌트를 비활성화하여 추가 오류를 방지할 수 있습니다.
            this.enabled = false;
        }
    }

    void Update()
    {
        FindVisibleTargets();
    }

    void OnAttack()
    {
        // 이미 공격 중이면(StateMachineBehaviour가 아직 false로 바꾸기 전이면) 입력을 무시합니다
        if (isAttacking)
        {
            return;
        }

        // 공격 트리거만 설정합니다.
        // isAttacking 플래그를 true/false로 바꾸는 것은 이제 AttackStateBehaviour의 역할입니다.
        animator.SetTrigger(attackTriggerHash);
        Debug.Log("공격!");

        // 감지된 모든 적에게 대미지 전달
        foreach (Transform enemy in enemyList)
        {
            Debug.Log($"Attack to {enemy.name}! temp");
        }
    }

    void FindVisibleTargets()
    {
        // 이전 프레임에서 감지된 타겟 리스트를 초기화
        enemyList.Clear();

        // 1. 거리 검사 (OverlapSphere)
        // viewRadius 거리 내에 있는 targetMask 레이어를 가진 모든 콜라이더를 가져옴
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, radius, enemyLayer);

        // 2. 각도 검사 (Dot Product)
        foreach (Collider targetCollider in targetsInViewRadius)
        {
            // 현재 검사할 타겟의 콜라이더와 트랜스폼을 가져옴
            Transform target = targetCollider.transform;

            // 2. 가장 가까운 지점 계산 (핵심 변경점)
            // 플레이어 위치(transform.position)를 기준으로 타겟 콜라이더의 가장 가까운 지점을 찾음
            Vector3 closestPoint = targetCollider.ClosestPoint(transform.position);

            // 3. 각도 및 장애물 검사 (가장 가까운 지점 기준)
            // 플레이어로부터 '가장 가까운 지점'을 향하는 방향 벡터를 계산
            Vector3 dirToClosestPoint = (closestPoint - transform.position).normalized;

            // 플레이어의 정면 벡터와 타겟 방향 벡터의 내적(Dot Product) 계산
            // Vector3.Dot()의 결과값은 두 벡터가 같은 방향일 때 1, 90도일 때 0, 반대 방향일 때 -1
            // viewAngle/2 보다 작은 각도에 있는지 검사
            // 플레이어의 정면과 '가장 가까운 지점'의 방향이 시야각 내에 있는지 검사
            if (Vector3.Dot(transform.forward, dirToClosestPoint) > Mathf.Cos((angle / 2) * Mathf.Deg2Rad))
            {
                // 플레이어와 '가장 가까운 지점' 사이의 거리를 계산
                float distToClosestPoint = Vector3.Distance(transform.position, closestPoint);

                // 플레이어와 '가장 가까운 지점' 사이에 장애물이 없는지 확인
                if (!Physics.Raycast(transform.position, dirToClosestPoint, distToClosestPoint, objectLayer))
                {
                    // 모든 조건을 통과하면 이 타겟을 리스트에 추가
                    enemyList.Add(target);
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        // 부채꼴의 반지름(거리)를 원으로 표시
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, radius);

        // 부채꼴의 양쪽 끝 각도를 계산
        Vector3 viewAngleA = DirFromAngle(-angle / 2, false);
        Vector3 viewAngleB = DirFromAngle(angle / 2, false);

        // 부채꼴의 양쪽 끝 선을 그림
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + viewAngleA * radius);
        Gizmos.DrawLine(transform.position, transform.position + viewAngleB * radius);

        // 현재 감지된 타겟들을 붉은 색으로 표시
        Gizmos.color = Color.red;
        foreach (Transform target in enemyList)
        {
            Gizmos.DrawLine(transform.position, target.position);
        }
    }

    // 각도를 방향 벡터로 변환해주는 헬퍼 함수
    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
