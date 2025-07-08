using System.Collections;
using UnityEngine;

public class PlayerStatusManager : MonoBehaviour
{
    [Header("Status Data")]
    public SOPlayerStatus data;

    private Coroutine epRecoveryCoroutine;

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

        // EP 사용 시, 기존 회복 코루틴을 멈추고 지연 시간 후 다시 시작
        RestartEpRecovery();

        return true;
    }

    // EP 회복 코루틴을 안전하게 재시작하는 메서드
    public void RestartEpRecovery()
    {
        if (epRecoveryCoroutine != null)
        {
            StopCoroutine(epRecoveryCoroutine);
        }
        epRecoveryCoroutine = StartCoroutine(RecoverEpAfterDelay());
    }

    private IEnumerator RecoverEpAfterDelay()
    {
        // 설정된 지연 시간만큼 기다립니다.
        yield return new WaitForSeconds(data.epReDelay);

        float recoveryAccumulation = 0;
        // EP가 최대치에 도달할 때까지 계속 회복합니다.
        while (data.curEp < data.maxEp)
        {
            // EP 회복
            recoveryAccumulation += data.epReRate * Time.deltaTime;
            if (recoveryAccumulation >= 1)
            {
                int recovery = Mathf.FloorToInt(recoveryAccumulation);
                AddEp(recovery);
                recoveryAccumulation -= recovery;
            }
            yield return null; // 다음 프레임까지 대기
        }
        Debug.Log("EP fully recovered");
        epRecoveryCoroutine = null; // 코루틴 종료
    }

    // (기존의 AddEp, SetEp 등 다른 메서드는 그대로 둡니다)

    public void AddEp(int amount)
    {
        float oldEp = data.curEp;
        data.curEp = Mathf.Min(data.maxEp, data.curEp + amount);

        if (data.curEp >= data.maxEp && oldEp < data.maxEp)
        {
            Debug.Log("EP fully recovered");
        }
    }

    public void SetEp(int amount)
    {
        data.curEp = Mathf.Clamp(amount, 0, data.maxEp);
    }
}
