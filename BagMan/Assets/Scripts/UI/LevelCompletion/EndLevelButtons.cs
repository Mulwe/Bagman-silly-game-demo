using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class EndLevelButtons : MonoBehaviour
{
    [SerializeField] private Button _btnExit;
    [SerializeField] private Button _btnRestart;

    private EventBus _bus;


    public bool TimeStopped { get; private set; }


    public void Initialize(EventBus bus)
    {
        TimeStopped = false;
        _bus = bus;
        if (_bus == null)
            Debug.LogError("EventBus is not init");
    }

    public void OnRestartClick()
    {
        if (_btnRestart != null)
        {
            _bus?.TriggerRestartGame();
            //Debug.Log($"{this}:OnRestartClicked");
        }
    }

    public void OnExitClick()
    {
        if (_btnExit != null)
        {
            //Debug.Log($"{this}: OnExitClick");
            _bus.TriggerExitGame();
        }
    }

    private void Awake()
    {
        var obj = this.gameObject;
        if (obj)
        {
            if (obj.layer == LayerMask.NameToLayer("UI"))
            {
                Button[] buttons = obj.GetComponentsInChildren<Button>(true);
                if (buttons != null && buttons.Length > 0)
                {
                    _btnExit = buttons.FirstOrDefault(b => b.name == "ButtonExit");
                    _btnRestart = buttons.FirstOrDefault(b => b.name == "ButtonRestart");
                }
            }
        }
    }

}
