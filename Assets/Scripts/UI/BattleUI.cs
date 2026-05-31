using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour
{
    [SerializeField] private Button deployButton;
    [SerializeField] private Text deployCountText;
    [SerializeField] private Text gateHpText;
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private Text resultText;
    [SerializeField] private Button restartButton;

    private BattleManager battleManager;
    private UnitSpawner spawner;

    public void SetReferences(
        Button configuredDeployButton,
        Text configuredDeployCountText,
        Text configuredGateHpText,
        GameObject configuredResultPanel,
        Text configuredResultText,
        Button configuredRestartButton)
    {
        deployButton = configuredDeployButton;
        deployCountText = configuredDeployCountText;
        gateHpText = configuredGateHpText;
        resultPanel = configuredResultPanel;
        resultText = configuredResultText;
        restartButton = configuredRestartButton;
    }

    public void Configure(BattleManager manager, UnitSpawner unitSpawner)
    {
        battleManager = manager;
        spawner = unitSpawner;

        if (deployButton != null)
        {
            deployButton.onClick.RemoveAllListeners();
            deployButton.onClick.AddListener(OnDeployPressed);
        }

        if (restartButton != null)
        {
            restartButton.onClick.RemoveAllListeners();
            restartButton.onClick.AddListener(RestartScene);
        }

        if (spawner != null)
        {
            spawner.OnDeploysChanged -= HandleDeploysChanged;
            spawner.OnDeploysChanged += HandleDeploysChanged;
            HandleDeploysChanged(spawner.RemainingDeploys, spawner.MaxDeploys);
        }

        if (resultPanel != null)
        {
            resultPanel.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        if (spawner != null)
        {
            spawner.OnDeploysChanged -= HandleDeploysChanged;
        }
    }

    public void SetGateHp(float current, float max)
    {
        if (gateHpText != null)
        {
            gateHpText.text = $"Gate HP: {Mathf.CeilToInt(current)}/{Mathf.CeilToInt(max)}";
        }
    }

    public void SetDeployInteractable(bool interactable)
    {
        if (deployButton != null)
        {
            deployButton.interactable = interactable;
        }
    }

    public void ShowResult(bool victory)
    {
        if (resultPanel != null)
        {
            resultPanel.SetActive(true);
        }

        if (resultText != null)
        {
            resultText.text = victory ? "Victory!\nGate destroyed." : "Defeat\nNo deploys and no allies left.";
        }
    }

    private void HandleDeploysChanged(int remaining, int max)
    {
        if (deployCountText != null)
        {
            deployCountText.text = $"Deploys: {remaining}/{max}";
        }
    }

    private void OnDeployPressed()
    {
        battleManager?.TryDeploy();
    }

    private void RestartScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }
}
