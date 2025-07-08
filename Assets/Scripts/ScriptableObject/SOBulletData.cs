using UnityEngine;

[CreateAssetMenu(fileName = "SOBulletData", menuName = "Scriptable Objects/SOBulletData")]
public class SOBulletData : ScriptableObject
{
    [Header("Stats")]
    public float speed = 20f;
    public int damage = 10;
    public float lifetime = 5f; // 총알이 파괴되기까지의 시간

    [Header("Visuals & Effects")]
    public GameObject impactEffect; // 충돌 시 생성될 이펙트 
}
