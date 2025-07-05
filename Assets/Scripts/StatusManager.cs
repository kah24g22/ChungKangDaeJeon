using UnityEngine;

public class StatusManager : MonoBehaviour
{
    [Header("Status Data")]
    public SOPlayerStatus data;

    // EP 회복 관련 변수
    private float lastEpUseTime;
    private float recoveryAccumulation;
    private bool isRecoveringEp = true;

    void Update()
    {
        HandleEpRecovery();
    }

    void HandleEpRecovery()
    {
        // EP가 최대치가 아니고, 마지막 사용 후 충분한 시간이 지났으면 회복 시작
        if (data.curEp < data.maxEp && Time.time - lastEpUseTime >= data.epReDelay)
        {
            if (!isRecoveringEp)
            {
                isRecoveringEp = true;
                Debug.Log("EP recovery started");
            }

            // EP 회복
            recoveryAccumulation += data.epReRate * Time.deltaTime;
            if (recoveryAccumulation >= 1)
            {
                int recovery = Mathf.FloorToInt(recoveryAccumulation);
                AddEp(recovery);
                recoveryAccumulation -= recovery;
            }
        }
    }

    

    public void UseEp(int amount)
    {
        data.curEp = Mathf.Max(0, data.curEp - amount);
        lastEpUseTime = Time.time;
        isRecoveringEp = false;

        Debug.Log($"EP used: {amount}, Current EP: {data.curEp}/{data.maxEp}");
    }

    public void AddEp(int amount)
    {
        float oldEp = data.curEp;
        data.curEp = Mathf.Min(data.maxEp, data.curEp + amount);
        
        if (data.curEp >= data.maxEp && oldEp < data.maxEp)
        {
            isRecoveringEp = false;
            Debug.Log("EP fully recovered");
        }
    }

    public void SetEp(int amount)
    {
        data.curEp = Mathf.Clamp(amount, 0, data.maxEp);
    }
}
