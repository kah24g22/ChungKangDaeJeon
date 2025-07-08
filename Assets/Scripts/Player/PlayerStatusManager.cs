using System.Collections;
using UnityEngine;

public class PlayerStatusManager : MonoBehaviour
{
    [Header("Status Data")]
    public SOPlayerStatus data;

    // EP 회복 관련 변수
    private float recoveryAccumulation;
    private Coroutine epRecoveryCoroutine;


    public void UseEp(int amount)
    {
        if (data.curEp < amount) return; // EP가 부족하면 사용하지 않음

        data.curEp -= amount;
        Debug.Log($"EP used: {amount}, Current EP: {data.curEp}/{data.maxEp}");

        // 만약 이미 회복 코루틴이 실행 중이라면 중지시킵니다.
        if (epRecoveryCoroutine != null)
        {
            StopCoroutine(epRecoveryCoroutine);
        }
        // 새로운 회복 코루틴을 시작합니다.
        epRecoveryCoroutine = StartCoroutine(RecoverEpAfterDelay());
    }

    private IEnumerator RecoverEpAfterDelay()
    {
        // 설정된 지연 시간만큼 기다립니다.
        yield return new WaitForSeconds(data.epReDelay);

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
