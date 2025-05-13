/*
 *  Interfaces
 * */

public interface ICharacterStats : IHealth, IStamina
{
    // IHealth and IStamina are already inheriting 
    //  some more stuff
}

public interface IHealth
{
    public int Health { get; set; }
    public void TakeDamage(int damage);
    public void Heal(int damage);
}
public interface IStamina
{
    public int Stamina { get; set; }
    public void UseStamina(int amount);
    public void RestoreStamina(int amount);
    public bool TryUseStamina(int amount);
    public void SetStaminaRate(float rate);
}
