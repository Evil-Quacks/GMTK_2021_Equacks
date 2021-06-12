using System;

[Serializable]
public class PlayerStats
{
    public int totalHealth;

    public int currentHealth;

    public float moveForce;

    public float maxMoveVelocity;

    public float jumpImpulseForce;

    public float airForce;

    PlayerStats(int _totalHealth, int _currentHealth, float _moveForce, float _maxMoveVelocity, float _jumpImpulseForce, float _airForce)
    {
        totalHealth = _totalHealth;
        currentHealth = _currentHealth;
        moveForce = _moveForce;
        maxMoveVelocity = _maxMoveVelocity;
        jumpImpulseForce = _jumpImpulseForce;
        airForce = _airForce;
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
