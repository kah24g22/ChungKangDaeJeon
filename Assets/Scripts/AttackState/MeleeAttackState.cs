using UnityEngine;

public class MeleeAttackState : IAttackState
{
    public void Enter(PlayerAttack player)
    {
        // 근접 상태로 진입하면 Melee 레이어의 가중치를 1로, Ranged는 0으로 설정
        player.animator.SetLayerWeight(player.meleeLayerIndex, 1f);
        player.animator.SetLayerWeight(player.rangedLayerIndex, 0f);
        player.isGun = false;
        Debug.Log("상태 변경: 근접");
    }

    public void OnAttack(PlayerAttack player)
    {
        // 이미 공격 중이면 입력을 무시
        if (player.isAttacking)
        {
            return;
        }

        // 공격 범위 내의 적을 찾음
        player.FindAttackableTargets();

        // 애니메이션 트리거 설정
        player.animator.SetTrigger(player.meleeTriggerHash);
        Debug.Log("근접 공격!");

        // 감지된 모든 적에게 대미지 전달 (기존 로직)
        foreach (Transform enemy in player.enemyList)
        {
            Debug.Log($"Attack to {enemy.name}! temp");
        }
    }

    public void Exit(PlayerAttack player)
    {
        // 상태를 나갈 때 특별히 처리할 내용이 있다면 여기에 작성
    }
}