using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    // 싱글톤 인스턴스: 다른 스크립트에서 쉽게 접근하기 위함
    public static ObjectPooler Instance;

    [Header("Pool Settings")]
    public GameObject bulletPrefab; // 풀링할 총알 프리팹
    public int poolSize = 5;       // 초기 풀 크기

    // 오브젝트를 담아둘 큐(Queue)
    private Queue<GameObject> bulletPool;

    void Awake()
    {
        // 싱글톤 패턴 구현
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // 오브젝트 풀 초기화
        InitializePool();
    }

    private void InitializePool()
    {
        bulletPool = new Queue<GameObject>();

        for (int i = 0; i < poolSize; i++)
        {
            // 프리팹을 인스턴스화하여 풀에 추가
            GameObject bullet = Instantiate(bulletPrefab);
            bullet.SetActive(false); // 비활성화 상태로 생성
            bulletPool.Enqueue(bullet);
        }
    }

    // 풀에서 오브젝트를 가져오는 메서드
    public GameObject GetBullet()
    {
        // 풀에 사용 가능한 오브젝트가 있으면
        if (bulletPool.Count > 0)
        {
            GameObject bullet = bulletPool.Dequeue(); // 큐에서 하나 꺼냄
            bullet.SetActive(true); // 활성화
            return bullet;
        }
        // 풀이 비어있다면, 새로 생성 (확장성)
        else
        {
            Debug.LogWarning("Bullet pool is empty. Creating a new one.");
            GameObject bullet = Instantiate(bulletPrefab);
            // 이 총알은 사용 후 풀에 반납될 것임
            return bullet;
        }
    }

    // 사용이 끝난 오브젝트를 풀에 반납하는 메서드
    public void ReturnBullet(GameObject bullet)
    {
        bullet.SetActive(false); // 비활성화
        bulletPool.Enqueue(bullet); // 다시 큐에 넣음
    }
}