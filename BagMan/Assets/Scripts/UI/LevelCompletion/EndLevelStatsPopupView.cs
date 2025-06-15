using TMPro;
using UnityEngine;

public class EndLevelStatsPopup : MonoBehaviour
{
    private EventBus _eventbus;
    private TextMeshProUGUI _dataScore;
    private TextMeshProUGUI _dataGreeting;

    private ulong _score = 0;
    private bool _isInit = false;

    private bool _goalAchieved = false;

    public bool IsInit => _isInit;


    public void Initialize(EventBus eventBus)
    {
        if (eventBus != null)
        {
            _eventbus = eventBus;
            AddListeners(_eventbus);
            _isInit = true;
        }
    }

    private void AddListeners(EventBus eventBus)
    {
        if (eventBus != null)
        {
            eventBus.GameCountScore.AddListener(OnScoreCount);
            eventBus.GameLevelComplete.AddListener(OnLevelCompleted);
            eventBus.GameLevelGoalComplete.AddListener(OnGoalAchieved);
            eventBus.HideLevelStats.AddListener(HidePopup);
        }
    }

    private void RemoveListeners(EventBus eventBus)
    {
        if (eventBus != null)
        {
            eventBus.GameCountScore.RemoveListener(OnScoreCount);
            eventBus.GameLevelComplete.RemoveListener(OnLevelCompleted);
            eventBus.GameLevelGoalComplete.RemoveListener(OnGoalAchieved);
            eventBus.HideLevelStats.RemoveListener(HidePopup);
        }
    }

    private void HidePopup()
    {
        this.gameObject.SetActive(false);
        _goalAchieved = false;
        if (_dataGreeting != null)
            _dataGreeting.gameObject.SetActive(false);
    }

    private void ShowPopup()
    {
        this.gameObject.SetActive(true);
    }

    private void OnShowPopUp()
    {
        if (_dataScore != null)
        {
            _dataScore.text = _score.ToString();
        }
        ShowPopup();
    }

    private void OnScoreCount(ulong score)
    {
        if (_dataScore != null)
        {
            _score = score;
        }
    }

    private void OnLevelCompleted()
    {
        //Debug.Log($"{this}: OnLevelCompleted()");

        OnShowPopUp();
    }

    private void OnGoalAchieved()
    {
        Debug.Log($"{this}: OnGoalAchieved()");
        _goalAchieved = true;
        if (_dataGreeting != null)
        {
            _dataGreeting.text = "Good Job!";
            _dataGreeting.gameObject.SetActive(true);
        }
    }


    private void GetTextMesh<T>(T component, out TextMeshProUGUI textMesh) where T : Component
    {
        if (component != null)
        {
            component.TryGetComponent<TextMeshProUGUI>(out var data);
            if (data != null)
            {
                textMesh = data;
            }
        }
        textMesh = null;
    }

    private void Awake()
    {
        var score_data = transform.gameObject.GetComponentInChildren<GlowingMaterial>(true);
        var greeting = transform.gameObject.GetComponentInChildren<Greeting>(true);

        GetTextMesh<GlowingMaterial>(score_data, out _dataScore);
        GetTextMesh<Greeting>(greeting, out _dataGreeting);

        /*
        if (score_data != null)
        {
            score_data.TryGetComponent<TextMeshProUGUI>(out var text);
            if (text != null)
                _data = text;
            else
                Debug.Log($"Score not found");
        }
        */

        HidePopup();
    }

    private void OnDestroy()
    {
        RemoveListeners(_eventbus);
    }


}
