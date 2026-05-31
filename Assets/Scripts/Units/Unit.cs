using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [System.Serializable]
    public struct CombatStats
    {
        public float maxHealth;
        public float moveSpeed;
        public float attackDamage;
        public float attackRange;
        public float attackCooldown;
    }

    public static readonly List<Unit> ActiveUnits = new List<Unit>();

    [SerializeField] private Team team;
    [SerializeField] private CombatStats stats;
    [SerializeField] private float targetScanInterval = 0.2f;

    private Health health;
    private Health currentTarget;
    private Gate enemyGate;
    private float nextAttackAt;
    private float nextTargetScanAt;

    public Team Team => team;
    public bool IsAlive => health != null && !health.IsDead;

    private void OnEnable()
    {
        if (!ActiveUnits.Contains(this))
        {
            ActiveUnits.Add(this);
        }
    }

    private void OnDisable()
    {
        ActiveUnits.Remove(this);
    }

    public void Initialize(Team assignedTeam, CombatStats assignedStats, Gate assignedEnemyGate)
    {
        team = assignedTeam;
        stats = assignedStats;
        enemyGate = assignedEnemyGate;

        health = GetComponent<Health>();
        if (health == null)
        {
            health = gameObject.AddComponent<Health>();
        }

        health.SetMaxHealth(stats.maxHealth, true);
        health.OnDeath -= HandleDeath;
        health.OnDeath += HandleDeath;
    }

    private void Update()
    {
        if (health == null || health.IsDead)
        {
            return;
        }

        if (Time.time >= nextTargetScanAt || !IsTargetValid())
        {
            AcquireTarget();
            nextTargetScanAt = Time.time + targetScanInterval;
        }

        if (currentTarget == null)
        {
            return;
        }

        Vector3 toTarget = currentTarget.transform.position - transform.position;
        float distance = toTarget.magnitude;

        if (distance > stats.attackRange)
        {
            transform.position += toTarget.normalized * stats.moveSpeed * Time.deltaTime;
            return;
        }

        if (Time.time >= nextAttackAt)
        {
            currentTarget.TakeDamage(stats.attackDamage);
            nextAttackAt = Time.time + stats.attackCooldown;
        }
    }

    private bool IsTargetValid()
    {
        if (currentTarget == null || currentTarget.IsDead)
        {
            return false;
        }

        Unit targetUnit = currentTarget.GetComponent<Unit>();
        if (targetUnit != null)
        {
            return targetUnit.Team != team;
        }

        Gate targetGate = currentTarget.GetComponent<Gate>();
        return team == Team.Ally && targetGate != null && !targetGate.IsDestroyed;
    }

    private void AcquireTarget()
    {
        currentTarget = FindNearestEnemyUnit();

        if (currentTarget == null && team == Team.Ally && enemyGate != null && !enemyGate.IsDestroyed)
        {
            currentTarget = enemyGate.Health;
        }
    }

    private Health FindNearestEnemyUnit()
    {
        Health nearest = null;
        float nearestDistance = float.MaxValue;

        for (int i = 0; i < ActiveUnits.Count; i++)
        {
            Unit other = ActiveUnits[i];
            if (other == null || other == this || other.Team == team || !other.IsAlive)
            {
                continue;
            }

            float distance = Vector2.Distance(transform.position, other.transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearest = other.health;
            }
        }

        return nearest;
    }

    private void HandleDeath(Health _)
    {
        Destroy(gameObject);
    }
}
