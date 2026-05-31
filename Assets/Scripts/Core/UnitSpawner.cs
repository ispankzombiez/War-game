using System;
using UnityEngine;

public class UnitSpawner : MonoBehaviour
{
    [SerializeField] private GameObject allyUnitPrefab;
    [SerializeField] private Gate enemyGate;
    [SerializeField] private int maxDeploys = 8;
    [SerializeField] private int unitsPerDeploy = 3;
    [SerializeField] private Vector2 spawnCenter = new Vector2(0f, -6.5f);
    [SerializeField] private float horizontalSpread = 1.8f;
    [SerializeField] private Unit.CombatStats allyStats;

    private int remainingDeploys;

    public event Action<int, int> OnDeploysChanged;

    public int RemainingDeploys => remainingDeploys;
    public int MaxDeploys => maxDeploys;

    public void Configure(GameObject prefab, Gate gate, int deployCount, int deployGroupSize, Vector2 spawnPoint, float spread, Unit.CombatStats stats)
    {
        allyUnitPrefab = prefab;
        enemyGate = gate;
        maxDeploys = Mathf.Max(1, deployCount);
        unitsPerDeploy = Mathf.Max(1, deployGroupSize);
        spawnCenter = spawnPoint;
        horizontalSpread = Mathf.Max(0f, spread);
        allyStats = stats;

        remainingDeploys = maxDeploys;
        OnDeploysChanged?.Invoke(remainingDeploys, maxDeploys);
    }

    public bool Deploy()
    {
        if (remainingDeploys <= 0 || allyUnitPrefab == null)
        {
            return false;
        }

        remainingDeploys--;
        OnDeploysChanged?.Invoke(remainingDeploys, maxDeploys);

        for (int i = 0; i < unitsPerDeploy; i++)
        {
            float t = unitsPerDeploy == 1 ? 0.5f : i / (float)(unitsPerDeploy - 1);
            float xOffset = Mathf.Lerp(-horizontalSpread, horizontalSpread, t);
            Vector3 spawnPos = new Vector3(spawnCenter.x + xOffset + UnityEngine.Random.Range(-0.3f, 0.3f), spawnCenter.y + UnityEngine.Random.Range(-0.2f, 0.2f), 0f);

            GameObject spawned = Instantiate(allyUnitPrefab, spawnPos, Quaternion.identity);
            if (!spawned.activeSelf)
            {
                spawned.SetActive(true);
            }

            Unit unit = spawned.GetComponent<Unit>();
            unit.Initialize(Team.Ally, allyStats, enemyGate);
        }

        return true;
    }
}
