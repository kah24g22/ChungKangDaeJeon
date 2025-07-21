using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    // --- 상태 패턴 관련 ---
    private IAttackState currentState;

    // --- 컴포넌트 및 설정 ---
    public Animator animator;
    public PlayerDirection playerDirection { get; private set; } // 다른 클래스에서 참조할 수 있도록 public으로 변경
    public bool isGun = false; // 현재 상태를 간단히 확인하기 위한 플래그

    // --- 애니메이션 해시 및 인덱스 ---
    public readonly int meleeTriggerHash = Animator.StringToHash("Melee");
    private const string MELEE_LAYER_NAME = "Melee Layer";
    private const string RANGED_LAYER_NAME = "Ranged Layer";
    public int meleeLayerIndex { get; private set; } = -1;
    public int rangedLayerIndex { get; private set; } = -1;

    // 공격 상태 플래그 (StateMachineBehaviour가 제어)
    public bool isAttacking = false;

    [Header("Melee Range")]
    [SerializeField] private float radius = 1.5f;
    [Range(0, 360)]
    [SerializeField] private float angle = 90f;

    [Header("GunsReference")]
    public GameObject bulletPrefab;
    public Transform firePoint;

    [Header("Target Setting")]
    [SerializeField] private LayerMask enemyLayer = 9;
    [SerializeField] private LayerMask objectLayer = 11;

    // 감지된 타겟 리스트
    public List<Transform> enemyList = new List<Transform>();

    void Awake()
    {
        playerDirection = GetComponent<PlayerDirection>();

        // 레이어 인덱스 초기화
        meleeLayerIndex = animator.GetLayerIndex(MELEE_LAYER_NAME);
        rangedLayerIndex = animator.GetLayerIndex(RANGED_LAYER_NAME);

        if (meleeLayerIndex == -1) Debug.LogError($"애니메이터에 '{MELEE_LAYER_NAME}' 레이어가 존재하지 않습니다!");
        if (rangedLayerIndex == -1) Debug.LogError($"애니메이터에 '{RANGED_LAYER_NAME}' 레이어가 존재하지 않습니다!");
    }

    void Start()
    {
        // 초기 상태를 Melee로 설정
        TransitionToState(new MeleeAttackState());
    }

    void Update()
    {
        // 'Q' 키를 눌러 상태 전환 (테스트용)
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (currentState is MeleeAttackState)
            {
                TransitionToState(new RangedAttackState());
            }
            else
            {
                TransitionToState(new MeleeAttackState());
            }
        }
    }

    // 상태를 전환하는 메서드
    public void TransitionToState(IAttackState nextState)
    {
        if (currentState != null)
        {
            currentState.Exit(this);
        }
        currentState = nextState;
        currentState.Enter(this);
    }

    // Input System에 의해 호출되는 공격 메서드
    void OnAttack()
    {
        // 모든 공격 로직을 현재 상태 객체에 위임
        if (currentState != null)
        {
            currentState.OnAttack(this);
        }
    }

    // MeleeAttackState에서 호출할 수 있도록 public으로 변경
    public void FindAttackableTargets()
    {
        enemyList.Clear();
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, radius, enemyLayer);

        foreach (Collider targetCollider in targetsInViewRadius)
        {
            Transform target = targetCollider.transform;
            Vector3 closestPoint = targetCollider.ClosestPoint(transform.position);
            Vector3 dirToClosestPoint = (closestPoint - transform.position).normalized;

            if (Vector3.Dot(transform.forward, dirToClosestPoint) > Mathf.Cos((angle / 2) * Mathf.Deg2Rad))
            {
                float distToClosestPoint = Vector3.Distance(transform.position, closestPoint);
                if (!Physics.Raycast(transform.position, dirToClosestPoint, distToClosestPoint, objectLayer))
                {
                    enemyList.Add(target);
                }
            }
        }
    }

    // --- 기즈모 및 헬퍼 함수 ---
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, radius);

        Vector3 viewAngleA = DirFromAngle(-angle / 2, false);
        Vector3 viewAngleB = DirFromAngle(angle / 2, false);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + viewAngleA * radius);
        Gizmos.DrawLine(transform.position, transform.position + viewAngleB * radius);

        Gizmos.color = Color.red;
        foreach (Transform target in enemyList)
        {
            Gizmos.DrawLine(transform.position, target.position);
        }
    }

    private Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}