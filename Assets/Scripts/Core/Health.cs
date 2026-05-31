using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float maxHealth = 10f;
    [SerializeField] private float currentHealth = 10f;

    public event Action<Health> OnDeath;
    public event Action<Health, float, float> OnHealthChanged;

    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;
    public bool IsDead => currentHealth <= 0f;

    public void SetMaxHealth(float value, bool resetCurrent)
    {
        maxHealth = Mathf.Max(1f, value);
        if (resetCurrent)
        {
            currentHealth = maxHealth;
        }
        else
        {
            currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        }

        OnHealthChanged?.Invoke(this, currentHealth, maxHealth);
    }

    public void TakeDamage(float damage)
    {
        if (IsDead)
        {
            return;
        }

        currentHealth = Mathf.Max(0f, currentHealth - Mathf.Max(0f, damage));
        OnHealthChanged?.Invoke(this, currentHealth, maxHealth);

        if (currentHealth <= 0f)
        {
            OnDeath?.Invoke(this);
        }
    }
}
