using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class UIWidgetTextField : MonoBehaviour
{
    // обновление поля с текстом
    [SerializeField] private UI_TextData _childTextData;
    private UI_StatsTracker _parent;

    private EventBus _eventBus;
    private CharacterStats _playerStats;
    private bool _isInit = false;

    //---------Names-----------------------------------
    [Header("UI HUD Container names of fields")]
    public const string HUD_Temerature = "HUD_Temperature";
    public const string HUD_Speed = "HUD_Speed";
    public const string HUD_Count = "HUD_Count";

    enum ListenerActions
    {
        Remove = 0,
        Add = 1
    }

    private bool Initialized()
    {
        if (_parent == null)
            _parent = GetComponentInParent<UI_StatsTracker>();

        if (_parent != null && _parent.IsInit && _isInit == false)
        {
            _eventBus = _parent.EventBus;
            _playerStats = _parent.PlayerStats;

            //Наш компонент
            _childTextData = this.GetComponentInChildren<UI_TextData>();

            // подключить Listener в сависимости от того какой компонент
            AddOrRemoveListenerByType(_childTextData, _eventBus, ListenerActions.Add);
            _isInit = true;
        }
        return _isInit;
    }



    void AddOrRemoveListenerByType(UI_TextData obj, EventBus eventBus, ListenerActions act)
    {
        bool state = false;
        if (act == ListenerActions.Add)
            state = true;

        if (obj != null && eventBus != null)
        {

            switch (obj.ParentName)
            {
                case HUD_Count:
                    {
                        ModifyUnityEvent(eventBus.PlayerCountUpdateUI, OnPlayerCountChanged, state);
                    }
                    break;
                case HUD_Temerature:
                    {
                        ModifyUnityEvent(eventBus.TemperatureChangedUI, OnTemperatureChanged, state);
                    }
                    break;
                case HUD_Speed:
                    {
                        ModifyUnityEvent(eventBus.PlayerSpeedUpdateUI, OnPlayerSpeedChanged, state);
                    }
                    break;
                default:
                    Debug.LogError($"{this}: Wrong parent name");
                    break;
            }
        }
    }

    private void ModifyUnityEvent(UnityEvent unityEvent, UnityAction callback, bool add)
    {
        if (unityEvent == null || callback == null) return;

        if (add)
            unityEvent.AddListener(callback);
        else
            unityEvent.RemoveListener(callback);
    }

    private void ModifyUnityEvent(UnityEvent<float> unityEvent, UnityAction<float> callback, bool add)
    {
        if (unityEvent == null || callback == null) return;

        if (add)
            unityEvent.AddListener(callback);
        else
            unityEvent.RemoveListener(callback);
    }

    private void OnPlayerCountChanged(float count)
    {
        if (_childTextData != null)
        {
            _childTextData.SetCount(count);
        }
    }
    private void OnTemperatureChanged()
    {
        if (_childTextData != null)
        {
            //Debug.Log("Tempreture is triggered");
            //_textData.SetTemperature();
        }
    }

    private void OnPlayerSpeedChanged()
    {
        if (_childTextData != null && _playerStats != null)
        {
            _childTextData.SetSpeed(_playerStats.GetCharacterSpeed());
        }
    }

    IEnumerator WaitForInit()
    {
        while (Initialized() != true)
            yield return null;
    }

    private void Awake()
    {
        StartCoroutine(WaitForInit());
    }

    private void OnEnable()
    {
        _isInit = false;
        if (_parent == null)
            _parent = GetComponentInParent<UI_StatsTracker>();
        if (_childTextData == null)
            _childTextData = GetComponentInChildren<UI_TextData>();
    }

    private void OnDisable()
    {
        if (_isInit)
        {
            _isInit = false;
            //remove listeners
            AddOrRemoveListenerByType(_childTextData, _eventBus, ListenerActions.Remove);
        }
    }
}
