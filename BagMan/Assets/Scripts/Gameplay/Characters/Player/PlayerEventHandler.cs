using System.Collections;
using UnityEngine;

public class PlayerEventHandler : MonoBehaviour
{
    //Player
    [SerializeField] private PlayerController _control;
    private CharacterStats _playerStats;

    // Event system
    private EventBus _eventBus;
    private Gameplay _gameplay;
    private PlayerCartController _playerCartData;

    // conditions
    public int StaminaDrain = 10;
    private bool _canRepeat = false;
    private float _defaultSpeed = 5f;
    private float regenTimer = 0f;

    private bool _isInit = false;


    public void Initialize(GameManager gm)
    {
        _gameplay ??= gm.GamePlay;
        _control ??= gm?.PlayerController;
        _playerStats ??= gm?.PlayerStats;
        _eventBus ??= gm?.EventBus;
        if (_control != null && _playerStats != null
            && _eventBus != null && _isInit == false)
        {
            _control.UpdatePlayerSpeed(_playerStats.GetCharacterSpeed());
            _playerCartData = transform.GetComponent<PlayerCartController>();
            _defaultSpeed = _playerStats.GetCharacterSpeed();
            _isInit = true;
            _canRepeat = true;
        }
        else
        {
            Debug.LogError($"{this}: is not Init\n");
        }
    }


    public void Run()
    {
        // RefreshUI на всякий случай
        StartCoroutine(PlayerStatsCoroutine());
        //запуск условий
    }

    private void UpdatePlayerSpeed(float newSpeed)
    {
        if (_playerStats != null && _control)
        {
            _playerStats.SetCharacterSpeed(newSpeed);
            _control.UpdatePlayerSpeed(newSpeed);
        }
    }

    private void SpeedChangeCondition()
    {
        if (_playerCartData == null || _playerStats == null || _control == null)
            return;
        float k = 1.5f;
        var newSpeed = _defaultSpeed;
        var minSpeed = _defaultSpeed * 0.3f;
        if (_playerCartData.AttachedCarts > 0)
            newSpeed = Mathf.Max(minSpeed, _defaultSpeed - k * _playerCartData.AttachedCarts);
        if (_playerStats.Stamina == 0)
            newSpeed = minSpeed;
        UpdatePlayerSpeed(newSpeed);
    }


    private void StaminaDrainCondition()
    {
        float delay = 1.0f;

        if (!_canRepeat || _playerCartData == null)
            return;
        if (_canRepeat)
            StartCoroutine(WaitUntilRepeat(delay));
        Rigidbody2D rb = _control.GetComponent<Rigidbody2D>();
        if (rb == null || rb?.linearVelocity.SqrMagnitude() <= 0.01f)
            return;

        PlayerIsMovingWithCarts();
    }

    private void PlayerIsMovingWithCarts()
    {
        float staminaRateDrain = _playerCartData.AttachedCarts * (_playerStats.MaxStamina / 100);
        int staminaToDrain = Mathf.RoundToInt(StaminaDrain * staminaRateDrain);

        _playerStats.UseStamina(staminaToDrain);
    }

    IEnumerator WaitUntilRepeat(float delayInSeconds)
    {
        _canRepeat = false;
        yield return new WaitForSeconds(delayInSeconds);
        _canRepeat = true;
    }

    IEnumerator PlayerStatsCoroutine()
    {
        if (_isInit && _playerStats != null && _playerCartData != null && _control != null)
        {
            while (_isInit)
            {
                RefreshUI();
                yield return new WaitForSeconds(0.5f);
            }
        }
    }

    private void OnDisable()
    {
        if (_isInit)
        {
            //отписка
            _isInit = false;
        }
    }

    private void FixedUpdate()
    {
        if (_isInit)
        {
            SpeedChangeCondition();
            StaminaDrainCondition();
        }
    }

    private void Update()
    {
        regenTimer += Time.deltaTime;
        if (_isInit)
        {
            if (regenTimer >= 1f)
            {
                if (_playerCartData.AttachedCarts > 0)
                    _playerStats.RestoreStamina(
                        StaminaDrain * (StaminaDrain / _playerCartData.AttachedCarts));
                else
                    _playerStats.RestoreStamina(StaminaDrain * 2);
                regenTimer = 0f;
            }
        }
    }


    public void RefreshUI()
    {
        _eventBus.TriggerTemperatureChangedUI();
        _eventBus.TriggerPlayerStaminaUpdateUI();
        _eventBus.TriggerPlayerSpeedUpdateUI();
    }
}
