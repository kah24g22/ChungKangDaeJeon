using UnityEngine;

public class UnityChanStatusInit : MonoBehaviour
{
    public SOPlayerStatus status;

    void Start()
    {
        // Movement
        status.speed = 5;
        status.dashSpeed = 15;
        status.dashDuration = 0.3f;
        status.dashCooldown = 1f;

        // Health Points
        status.curHp = 100;
        status.maxHp = 100;

        // Energe Points
        status.curEp = 100;
        status.maxEp = 100;
        status.epReRate = 10f;
        status.epReDelay = 2f;
        status.dashEpCost = 20;

        // "Level System"
        status.level = 1;
        status.curExp = 0;
        status.maxExp = 50;
        status.levelUpMultiple = 2;

        // Attack Power"
        status.attackPower = 1;
    }
}
