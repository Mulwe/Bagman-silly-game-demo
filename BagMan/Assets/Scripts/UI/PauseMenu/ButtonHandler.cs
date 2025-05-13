using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GameObject))]
[RequireComponent(typeof(Button))]
public class ButtonHandler : MonoBehaviour
{
    [SerializeField] private Button _btnResume;
    [SerializeField] private Button _btnExit;

    private EventBus _bus;
    public event Action OnExitClicked;
    public event Action OnResumeClicked;

    private bool _timeStopped = false;

    public void Initialize(EventBus bus)
    {
        _bus = bus;
        if (_bus == null)
            Debug.LogError("EventBus is not init");
        _timeStopped = false;
    }


    public void OnResumeClick()
    {
        OnResumeClicked?.Invoke();
        _bus.TriggerResumeGame();
        //_bus.GameResume.Invoke();
    }

    public void OnExitClick()
    {
        OnExitClicked?.Invoke();
        _bus.TriggerResumeGame();
        //_bus.GameResume.Invoke();
        _bus.TriggerExitGame();
    }

    private void OnTimePauseChanged(bool state)
    {
        if (state)
            _timeStopped = state;
        else
            _timeStopped = state;
    }

    private void OnValidate()
    {
        if (_btnResume == null)
        {
            Debug.LogError($"{nameof(_btnResume)} the reference is not set up yet");
        }
        if (_btnExit == null)
        {
            Debug.LogError($"{nameof(_btnExit)} the reference is not set up yet");
        }
    }

    private void Awake()
    {
        var obj = this.gameObject;
        if (obj)
        {
            if (obj.CompareTag("Menu"))
            {
                Button[] buttons = obj.GetComponentsInChildren<Button>(true);
                _btnResume = buttons.FirstOrDefault(b => b.name == "ButtonResume");
                _btnExit = buttons.FirstOrDefault(b => b.name == "ButtonExit");
            }
        }
    }

    private void OnEnable()
    {
        _bus.GameRunTime.AddListener(OnTimePauseChanged);
    }
    private void OnDisable()
    {
        _bus.GameRunTime.RemoveListener(OnTimePauseChanged);
    }
}
