using System.Collections;
using UnityEngine;

public class Powerup
{

    public delegate void PowerupBehaviour();
    private PowerupBehaviour _startPowerupBehaviour;
    private PowerupBehaviour _tickPowerupBehaviour;
    private PowerupBehaviour _endPowerupBehaviour;

    public bool IsActive;
    public string Name;

    private SnakeHand _snakehand;
    private bool _isWeak;
    private float _timeLimit;

    private float _timeRemaining;

    public Powerup(SnakeHand snakeHand, bool isWeak, string name, float timeLimit, PowerupBehaviour startPowerupBehaviour, PowerupBehaviour tickPowerupBehaviour, PowerupBehaviour endPowerupBehaviour)
    {
        _snakehand = snakeHand;
        _isWeak = isWeak;
        Name = name;
        _timeLimit = timeLimit;

        _startPowerupBehaviour = startPowerupBehaviour;
        _tickPowerupBehaviour = tickPowerupBehaviour;
        _endPowerupBehaviour = endPowerupBehaviour;

        _timeRemaining = _timeLimit;
    }

    public void StartPowerup()
    {
        IsActive = true;

        _startPowerupBehaviour();
    }

    public void UpdatePowerup(float deltaTime)
    {
        if (IsActive)
        {
            _timeRemaining -= deltaTime;
            _tickPowerupBehaviour();
        }

        if (_timeRemaining <= 0f) EndPowerup();
    }

    private void EndPowerup()
    {
        IsActive = false;

        _endPowerupBehaviour();

        uint slot = _isWeak ? 1u : 0u;
        _snakehand.EndPowerup(slot);
    }

}
