using System.Collections;
using UnityEngine;

public class UIWidgetStaminaBar : MonoBehaviour
{
    [SerializeField] private UI_ProgressBar _progressBar;
    private UI_StatsTracker _parent;


    private EventBus _eventBus;
    private CharacterStats _playerStats;


    private bool _isInit = false;

    private bool Initialized()
    {
        if (_parent == null)
            _parent = GetComponentInParent<UI_StatsTracker>();
        if (_parent != null && _parent.IsInit && _isInit == false)
        {
            _eventBus = _parent.EventBus;
            _playerStats = _parent.PlayerStats;
            _progressBar = GetComponent<UI_ProgressBar>();
            _eventBus.PlayerStaminaUpdateUI.AddListener(OnStaminaChanged);
            _isInit = true;
        }
        return _isInit;
    }

    //UiRoot может не успеть проинициализироваться, ждем инициализации
    IEnumerator WaitForInit()
    {
        while (Initialized() != true)
        {
            yield return null;
        }
    }

    private void OnStaminaChanged()
    {
        if (_progressBar != null)
        {
            _progressBar.SetValue(_playerStats.Stamina);
        }
    }

    private void Awake()
    {
        StartCoroutine(WaitForInit());
    }

    private void OnEnable()
    {
        _isInit = false;
        _parent = GetComponentInParent<UI_StatsTracker>();
    }

    private void OnDisable()
    {
        if (_isInit)
        {
            _isInit = false;
            _eventBus.PlayerStaminaUpdateUI.RemoveAllListeners();
        }
    }
}
