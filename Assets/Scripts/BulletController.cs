using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BulletController : MonoBehaviour
{
    public SOBulletData bulletData;
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (bulletData == null)
        {
            Debug.LogError("BulletData가 할당되지 않았습니다!", this);
        }
    }

    // OnEnable은 오브젝트가 활성화될 때마다 호출됩니다.
    void OnEnable()
    {
        // 활성화될 때마다 lifetime 후에 비활성화되도록 코루틴 시작
        StartCoroutine(DeactivateAfterTime(bulletData.lifetime));
    }

    // 발사체(PlayerAttack)가 호출하는 초기화 함수
    public void Initialize(Vector3 direction)
    {
        if (bulletData == null) return;
        rb.linearVelocity = direction * bulletData.speed;
    }

    // 일정 시간 후에 오브젝트를 풀에 반환하는 코루틴
    private IEnumerator DeactivateAfterTime(float time)
    {
        yield return new WaitForSeconds(time);

        // 시간이 다 되면 ObjectPooler를 통해 풀에 반환
        ObjectPooler.Instance.ReturnBullet(gameObject);
    }
    /*
    void OnCollisionEnter(Collision collision)
    {
        // 충돌 시에도 즉시 풀에 반환
        // 여기서 impact effect를 생성할 수 있습니다.
        if (bulletData.impactEffect != null)
        {
            // 이펙트도 풀링하는 것이 좋지만, 일단은 Instantiate 사용
            Instantiate(bulletData.impactEffect, transform.position, Quaternion.identity);
        }

        // 코루틴을 멈추고 즉시 반환
        StopAllCoroutines();
        ObjectPooler.Instance.ReturnBullet(gameObject);
    }*/
}