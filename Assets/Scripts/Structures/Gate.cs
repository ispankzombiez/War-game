using UnityEngine;

public class Gate : MonoBehaviour
{
    [SerializeField] private Health health;
    [SerializeField] private Transform healthFill;
    [SerializeField] private SpriteRenderer bodyRenderer;

    public Health Health => health;
    public bool IsDestroyed => health == null || health.IsDead;

    public void Configure(Health configuredHealth, Transform configuredHealthFill, SpriteRenderer configuredRenderer)
    {
        health = configuredHealth;
        healthFill = configuredHealthFill;
        bodyRenderer = configuredRenderer;

        health.OnHealthChanged += HandleHealthChanged;
        health.OnDeath += HandleDeath;

        UpdateVisuals();
    }

    private void OnDestroy()
    {
        if (health != null)
        {
            health.OnHealthChanged -= HandleHealthChanged;
            health.OnDeath -= HandleDeath;
        }
    }

    private void HandleHealthChanged(Health _, float __, float ___)
    {
        UpdateVisuals();
    }

    private void HandleDeath(Health _)
    {
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        if (health == null)
        {
            return;
        }

        float ratio = Mathf.Clamp01(health.CurrentHealth / health.MaxHealth);

        if (healthFill != null)
        {
            Vector3 scale = healthFill.localScale;
            scale.x = Mathf.Max(0f, ratio);
            healthFill.localScale = scale;
        }

        if (bodyRenderer != null)
        {
            bodyRenderer.color = Color.Lerp(new Color(0.55f, 0.15f, 0.15f), new Color(0.85f, 0.3f, 0.2f), ratio);
        }
    }
}
