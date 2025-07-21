public interface IAttackState
{
    // 상태에 진입할 때 호출될 함수
    void Enter(PlayerAttack player);

    // 공격 입력이 들어왔을 때 호출될 함수
    void OnAttack(PlayerAttack player);

    // 상태에서 빠져나갈 때 호출될 함수
    void Exit(PlayerAttack player);
}