using UnityEngine;

[CreateAssetMenu(fileName = "SOPlayerStatus", menuName = "Scriptable Objects/SOPlayerStatus")]
public class SOPlayerStatus : ScriptableObject
{
    [Header("Movement")]
    public int speed = 5;
    public int dashSpeed = 15;
    public float dashDuration = 0.3f;
    public float dashCooldown = 1f;

    [Header("Health Points")]
    public int curHp = 100;
    public int maxHp = 100;

    [Header("Energe Points")]
    public int curEp = 100;
    public int maxEp = 100;
    public float epReRate = 1f;
    public float epReDelay = 2f;
    public int dashEpCost = 20;

    [Header("Level System")]
    public int level = 1;
    public int curExp = 0;
    public int maxExp = 50;
    public int levelUpMultiple = 2;

    [Header("Attack Power")]
    public int attackPower = 1;
}
