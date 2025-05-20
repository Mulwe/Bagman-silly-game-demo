using System.Collections;
using TMPro;
using UnityEngine;

public class UITimer : MonoBehaviour
{
    [SerializeField] private UI_StatsTracker _ui_StatsTracker;
    private TextMeshProUGUI _textMeshProUGUI;


    private EventBus _eventBus;
    private Timer _timer;

    void Start()
    {
        _ui_StatsTracker ??= transform.parent.GetComponentInChildren<UI_StatsTracker>();
        _textMeshProUGUI ??= this.GetComponent<TextMeshProUGUI>();
        if (_textMeshProUGUI != null)
        {
            ChangeAlpha(_textMeshProUGUI, 0.0f);
        }
        Debug.Log("UITimer Start()");

    }

    private void OnEnable()
    {
        if (_eventBus == null)
            StartCoroutine(WaitInit());

    }

    IEnumerator WaitInit()
    {
        yield return null;
        while (_eventBus == null)
        {
            if (_ui_StatsTracker != null)
                _eventBus = _ui_StatsTracker.EventBus;
            yield return new WaitForSeconds(0.1f);
        }
        _eventBus.Timer.AddListener(ShowTimerUI);
    }

    private void ShowTimerUI(Timer t)
    {
        if (t != null)
        {
            _timer = t;
            _timer.OnTimerComplete += HandleTimerCompletion;
            if (_textMeshProUGUI != null)
            {
                var (min, sec) = NormilizeTime(_timer.GetRemainingTime());
                _textMeshProUGUI.text = $"{min:00}:{sec:00}";
                StartCoroutine(FadeIn(_textMeshProUGUI, 1.0f));
            }
        }
    }

    void HandleTimerCompletion()
    {
        if (_timer != null)
        {
            //Trigger подсчет очков конец игры
            Debug.Log("Time's up");
        }
    }

    IEnumerator FadeIn(TextMeshProUGUI target, float seconds)
    {
        if (target != null)
        {
            float time = 0f;
            while (time < seconds)
            {
                time += Time.deltaTime;
                float alpha = Mathf.Clamp01(time / seconds);
                ChangeAlpha(target, alpha);
                yield return null;
            }
            ChangeAlpha(target, 1.0f);
        }

    }

    void Update()
    {
        if (_textMeshProUGUI != null && _timer != null && _timer.IsRunning)
        {
            var (min, sec) = NormilizeTime(_timer.GetRemainingTime());
            _textMeshProUGUI.text = $"{min:00}:{sec:00}";
        }

    }

    private void OnDisable()
    {
        _eventBus?.Timer.RemoveListener(ShowTimerUI);
        if (_timer != null)
            _timer.OnTimerComplete -= HandleTimerCompletion;
    }

    private (int minutes, int seconds) NormilizeTime(float totalSeconds)
    {
        int minutes = Mathf.FloorToInt(totalSeconds / 60);
        int seconds = Mathf.FloorToInt(totalSeconds % 60);
        return (minutes, seconds);
    }
    private void ChangeAlpha(TextMeshProUGUI target, float alpha)
    {
        target.color = new Color(target.color.r, target.color.g, target.color.b, alpha);
    }

}
