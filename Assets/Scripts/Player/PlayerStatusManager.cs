using System.Collections;
using UnityEngine;

public class PlayerStatusManager : MonoBehaviour
{
    [Header("Status Data")]
    public SOPlayerStatus data;

    // EP 회복 지연 시간을 추적하기 위한 타이머
    private float epRecoveryTimer;
    // 소수점 단위의 EP 회복량을 누적하기 위한 변수
    private float recoveryAccumulation;

    void Update()
    {
        // EP가 이미 가득 차 있다면 아무것도 하지 않음
        if (data.curEp >= data.maxEp)
        {
            return;
        }

        // EP 회복 지연 타이머가 아직 남아있다면 시간을 감소시킴
        if (epRecoveryTimer > 0)
        {
            epRecoveryTimer -= Time.deltaTime;
        }
        // 타이머가 0이 되면 EP를 회복 시작
        else
        {
            RecoverEp();
        }
    }

    // EP를 회복하는 함수
    private void RecoverEp()
    {
        // 1. 초당 회복량을 누적 변수에 더합니다.
        recoveryAccumulation += data.epReRate * Time.deltaTime;

        // 2. 누적치가 1 이상이 되면
        if (recoveryAccumulation >= 1f)
        {
            // 3. 누적치의 정수 부분만큼 실제 EP를 회복시킵니다.
            int amountToRecover = Mathf.FloorToInt(recoveryAccumulation);
            AddEp(amountToRecover);

            // 4. 회복시킨 양만큼 누적치에서 뺍니다. (소수점 부분만 남김)
            recoveryAccumulation -= amountToRecover;
        }
    }

    // EP를 사용할 수 있는지 확인하는 메서드
    public bool CanUseEp(int amount)
    {
        return data.curEp >= amount;
    }

    // EP를 사용하는 메서드 (성공 여부 반환)
    public bool UseEp(int amount)
    {
        if (!CanUseEp(amount))
        {
            return false; // EP 부족
        }

        data.curEp -= amount;
        Debug.Log($"EP used: {amount}, Current EP: {data.curEp}/{data.maxEp}");

        // EP를 사용했으므로, 회복 지연 타이머를 다시 설정
        epRecoveryTimer = data.epReDelay;

        return true;
    }

    // (디버깅 또는 특정 이벤트용) EP를 즉시 추가하는 함수
    public void AddEp(int amount)
    {
        data.curEp = Mathf.Min(data.maxEp, data.curEp + amount);
    }

    // (디버깅 또는 특정 이벤트용) EP를 특정 값으로 설정하는 함수
    public void SetEp(int amount)
    {
        data.curEp = Mathf.Clamp(amount, 0, data.maxEp);
        // EP가 수동으로 변경되었을 때도 회복 딜레이를 적용하고 싶다면 아래 줄의 주석을 해제하세요.
        // epRecoveryTimer = data.epReDelay;
    }
}
