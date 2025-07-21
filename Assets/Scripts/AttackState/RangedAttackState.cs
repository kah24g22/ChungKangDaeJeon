using UnityEngine;

public class RangedAttackState : IAttackState
{
    public void Enter(PlayerAttack player)
    {
        // 원거리 상태로 진입하면 Ranged 레이어의 가중치를 1로, Melee는 0으로 설정
        player.animator.SetLayerWeight(player.meleeLayerIndex, 0f);
        player.animator.SetLayerWeight(player.rangedLayerIndex, 1f);
        player.isGun = true;
        Debug.Log("상태 변경: 원거리");
    }

    public void OnAttack(PlayerAttack player)
    {
        // PlayerDirection.cs가 계산해 둔 최종 수평 방향을 가져옴
        Vector3 fireDirection = player.playerDirection.aimDirection;

        if (fireDirection.sqrMagnitude > 0.01f)
        {
            // 1. ObjectPooler에서 총알을 가져옴
            GameObject bulletObject = ObjectPooler.Instance.GetBullet();

            // 2. 총알의 위치와 회전을 설정
            Quaternion bulletRotation = Quaternion.LookRotation(fireDirection);
            bulletObject.transform.position = player.firePoint.position;
            bulletObject.transform.rotation = bulletRotation;

            // 3. 총알 스크립트를 초기화
            BulletController bulletScript = bulletObject.GetComponent<BulletController>();
            if (bulletScript != null)
            {
                bulletScript.Initialize(fireDirection);
            }
            else
            {
                Debug.LogError("총알 프리팹에 BulletController 스크립트가 없습니다!");
            }
        }
    }

    public void Exit(PlayerAttack player)
    {
        // 상태를 나갈 때 특별히 처리할 내용이 있다면 여기에 작성
    }
}