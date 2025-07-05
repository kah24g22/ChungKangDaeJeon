using UnityEngine;

public class AttackStateBehaviour : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // 공격 상태에 진입하면 isAttacking 플래그를 true로 설정합니다.
        // animator.GetComponent<PlayerAttack>()를 통해 PlayerAttack 스크립트에 접근합니다.
        animator.GetComponent<PlayerAttack>().isAttacking = true;
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // 공격 상태에서 빠져나오면 isAttacking 플래그를 false로 설정합니다.
        // 이것이 쿨다운의 종료를 의미합니다.
        animator.GetComponent<PlayerAttack>().isAttacking = false;
    }
}