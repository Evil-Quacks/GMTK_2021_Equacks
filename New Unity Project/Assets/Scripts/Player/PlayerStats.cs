using System;

[Serializable]
public class PlayerStats
{
    public int totalHealth;

    public int currentHealth;

    public float moveSpeed;

    public float jumpSpeed;

    public float airSpeed;

    PlayerStats(int _totalHealth, int _currentHealth, float _moveSpeed, float _jumpSpeed, float _airSpeed)
    {
        totalHealth = _totalHealth;
        currentHealth = _currentHealth;
        moveSpeed = _moveSpeed;
        jumpSpeed = _jumpSpeed;
        airSpeed = _airSpeed;
    }

    public bool IsDead()
    {
        if (currentHealth <= 0)
        {
            return true;
        }
        return false;
    }

    public void ResetHealth()
    {
        currentHealth = totalHealth;
    }

}
