using System;

public class HealthSystem
{
    public event Action OnHealthChanged;
    public event Action OnMaxHealthChanged;
    public event Action OnHeal;
    public event Action OnDead;
    public event Action OnDamage;


    private float _maxHealth;


    public HealthSystem(float maxHealth)
    {
        _maxHealth = maxHealth;
        CurrentHealth = _maxHealth;
        MaxHealth = _maxHealth;
    }

    public float CurrentHealth { get; private set; }

    public float MaxHealth { get; private set; }

    public float GetHealthNormalized()
    {
        return CurrentHealth / MaxHealth;
    }
    
    public void Heal(float amount)
    {
        CurrentHealth += amount;
        if (CurrentHealth > _maxHealth) CurrentHealth = _maxHealth;
        
        OnHeal?.Invoke();
        OnHealthChanged?.Invoke();
    }
    
    public void HealComplete()
    {
        CurrentHealth = _maxHealth;
        
        OnHealthChanged?.Invoke();
        OnHeal?.Invoke();
    }

    public void SetHealthMax(float amount, bool recover = false)
    {
        _maxHealth = amount;

        if (recover)
        {
            CurrentHealth = _maxHealth;
            OnHealthChanged?.Invoke();
        }
        OnMaxHealthChanged?.Invoke();
    }

    public void Damage(float amount)
    {
        CurrentHealth -= amount;
        
        OnDamage?.Invoke();
        OnHealthChanged?.Invoke();
        
        if (CurrentHealth <= 0)
        {
            CurrentHealth = 0;
            Die();
        }
        
    }


    public void Die() => OnDead?.Invoke();
    public bool IsDead() => CurrentHealth <= 0;
    


}
