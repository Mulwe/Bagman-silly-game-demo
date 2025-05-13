using UnityEngine;

public class PlayerEventHandler : MonoBehaviour
{
    //Player
    [SerializeField] private PlayerController _control;
    private CharacterStats _playerStats;

    // Event system
    private EventBus _eventBus;

    // �� ������� ��� ����� ���� ���� ������ �� gm
    private Transform _parent;
    private Gameplay _gameplay;
    //

    bool _isInit = false;


    // ���� ���������, � ���� ������� ������� ��������.
    // ����� ���������� ���� ����� ���� ������� � ����� �� ���� ������� ����� ������ ����������� �������
    // ��� ������ ��� ������ ���

    // ��������� ������������� �� ������� 
    // ������� ������� �������
    // ������ �� ����� � ����������
    // ��������� �� ����� � ��������


    // ����� ��� ����� ������ ������ �������
    // � �������� ���� ����� ���� ��� �� ��� � ����������� ����������


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

            //�������
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

    // ����� UI
    // ����� ������ ������
    // ��� �������
    // UI ������ ����������� �� ����� �������. � UI ��� ������� � ������


    /*
        UI ������������� �� �������
        PlayeHandler - ��������� ��� � ��������� ��������� ������
        � UI ������ �������� � �������. ������� ���������� �����
        
     */



    public void RefreshUI()
    {
        _eventBus.TriggerPlayerHealthUpdateUI();
        _eventBus.TriggerTemperatureChangedUI();
        _eventBus.TriggerPlayerStaminaUpdateUI();
        _eventBus.TriggerPlayerSpeedUpdateUI();
    }



}
