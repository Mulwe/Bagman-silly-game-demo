using UnityEngine;

public class CharacterStats : ICharacterStats
{
    [SerializeField, Range(0, 100)] private int _maxHealth = 100;
    [SerializeField, Range(0, 100)] private int _maxStamina = 100;
    [SerializeField, Range(0, 100)] private float _staminaRate = 1;
    [SerializeField] private float staminaDrain = 5;

    public bool isLogging = false;
    private int _health;
    private int _stamina;

    private float _movementSpeed = 5f;
    public float MovementSpeed => _movementSpeed;

    public CharacterStats()
    {
        _health = 100;
        _stamina = 100;
        _staminaRate = (0.5f);
        Log("Stats have been set up.");
    }

    public CharacterStats(int health, int stamina, float rate)
    {
        Health = health;
        Stamina = stamina;
        SetStaminaRate(rate);
        Log("Stats have been set up.");
    }

    void Log(string message)
    {
        if (isLogging)
            Debug.Log(message);
    }

    //======================

    public int Health
    {
        get => _health;
        set => _health = (_health > _maxHealth) ? _maxHealth : value;
    }

    public int Stamina
    {
        get => _stamina;
        set => _stamina = (_stamina > _maxStamina) ? _maxStamina : value;
    }

    public void TakeDamage(int damage)
    {
        _health -= damage;
        _health = (_health < 0) ? 0 : _health;
    }

    public void Heal(int amount)
    {
        _health += amount;
        _health = (_health >= _maxHealth) ? _maxHealth : _health;
    }

    public void UseStamina(int amount)
    {
        _stamina -= amount;
        _stamina = (_stamina >= _maxStamina) ? _maxStamina : _stamina;
    }

    public void RestoreStamina(int amount)
    {
        float result = amount * _staminaRate;
        _stamina += (int)result;
        _stamina = (_stamina >= _maxStamina) ? _maxStamina : _stamina;
    }

    public bool TryUseStamina(int amount)
    {
        if ((_stamina - amount) >= 0)
        {
            UseStamina(amount);
            return true;
        }
        else
        {
            Log($"Stamina: not enough");
            return false;
        }
    }

    public void SetStaminaRate(float rate)
    {
        if (rate < 0)
            Log("Stamina: Rate is negative"); // add debuf
        else if (rate > 100)
            _staminaRate = 100;
        else
            _staminaRate = rate;
    }

    public float GetCharacterSpeed()
    {
        return _movementSpeed;
    }
    public void SetCharacterSpeed(float updatedSpeed)
    {
        if (updatedSpeed < 0)
            _movementSpeed = 0;
        else if (updatedSpeed > 50)
            _movementSpeed = 50;
        else
            _movementSpeed = updatedSpeed;
        Log($"{this} has this speed: {_movementSpeed} now.");
    }
}
