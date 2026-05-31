using UnityEngine;

public class BattleManager : MonoBehaviour
{
    private Gate enemyGate;
    private UnitSpawner spawner;
    private BattleUI battleUI;

    public bool IsBattleOver { get; private set; }

    public void Configure(Gate gate, UnitSpawner unitSpawner, BattleUI ui)
    {
        enemyGate = gate;
        spawner = unitSpawner;
        battleUI = ui;
    }

    private void Update()
    {
        if (IsBattleOver || enemyGate == null || spawner == null || battleUI == null)
        {
            return;
        }

        battleUI.SetGateHp(enemyGate.Health.CurrentHealth, enemyGate.Health.MaxHealth);

        if (enemyGate.IsDestroyed)
        {
            EndBattle(true);
            return;
        }

        if (spawner.RemainingDeploys <= 0 && CountLivingAllies() == 0)
        {
            EndBattle(false);
        }
    }

    public void TryDeploy()
    {
        if (IsBattleOver)
        {
            return;
        }

        spawner.Deploy();
    }

    private int CountLivingAllies()
    {
        int count = 0;

        for (int i = 0; i < Unit.ActiveUnits.Count; i++)
        {
            Unit unit = Unit.ActiveUnits[i];
            if (unit != null && unit.Team == Team.Ally && unit.IsAlive)
            {
                count++;
            }
        }

        return count;
    }

    private void EndBattle(bool victory)
    {
        IsBattleOver = true;
        battleUI.SetDeployInteractable(false);
        battleUI.ShowResult(victory);
    }
}
