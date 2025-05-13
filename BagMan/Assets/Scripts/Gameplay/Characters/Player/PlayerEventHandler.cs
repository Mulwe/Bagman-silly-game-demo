using UnityEngine;

public class PlayerEventHandler : MonoBehaviour
{
    //Player
    [SerializeField] private PlayerController _control;
    private CharacterStats _playerStats;

    // Event system
    private EventBus _eventBus;

    // не уверена что нужно если есть ссылка на gm
    private Transform _parent;
    private Gameplay _gameplay;
    //

    bool _isInit = false;


    // есть подписчик, а есть события которые тригерят.
    // можно отправлять сюда какие либо события и потом от этих событий будут дальше срабатывать тригеры
    // или делать это только тут

    // подписчик подписывается на событие 
    // триггер вызвает событие
    // тригер не знает о подписчике
    // подписчик не знает о триггере


    // думаю тут можно писать логику событий
    // а тригерры либо извне либо тут но уже в минимальном количестве


    public void Run()
    {

    }

    public void Initialize(GameManager gm)
    {
        _control = gm?.PlayerController;
        _playerStats = gm?.PlayerStats;
        _eventBus = gm?.EventBus;
        if (_control != null && _playerStats != null
            && _eventBus != null && _isInit == false)
        {
            _control.UpdatePlayerSpeed(_playerStats.GetCharacterSpeed());
            _isInit = true;
        }
        else
            Debug.LogError($"{this}: is not Init");
    }


    private void AddListeners()
    {
        //_eventBus
    }

    private void OnDisable()
    {
        if (_isInit)
        {

            //отписка
        }
    }

    private void FixedUpdate()
    {
        if (_isInit)
        {

        }

    }

    private void Update()
    {


    }

    // нужен UI
    // нужны данные игрока
    // или события
    // UI должен реагировать на такие события. у UI нет доступа к игроку


    /*
        UI подписывается на событие
        PlayeHandler - тригеррит его и мониторит остальные данные
        в UI только подписка и отписка. тригеры происходят извне
        
     */



    public void RefreshUI()
    {
        _eventBus.TriggerPlayerHealthUpdateUI();
        _eventBus.TriggerTemperatureChangedUI();
        _eventBus.TriggerPlayerStaminaUpdateUI();
        _eventBus.TriggerPlayerSpeedUpdateUI();
    }



}
