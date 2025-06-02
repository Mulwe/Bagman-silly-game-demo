using System.Linq;

public class LevelCompletion
{
    private readonly EventBus _eventBus;

    private ulong _score = 0;

    public ulong Score => _score;

    int[] t = new[] { 1, 2, 3, 4, 5, 6, 7 };



    public LevelCompletion(GameManager gm)
    {
        if (gm != null)
        {
            _eventBus = gm.EventBus;
            AddListeners();
        }
        t.Contains(0);

        //show score
        //send trigger to finish to close or move next level

    }

    public ulong GetScore() => _score;
    public void SetScore(ulong score) => _score = score;
    public void ResetScore() => _score = 0;


    private void AddListeners()
    {
        _eventBus.GameLevelComplete.AddListener(OnLevelCompleted);
    }

    private void RemoveListeners()
    {
        if (_eventBus != null)
            _eventBus.GameLevelComplete.AddListener(OnLevelCompleted);
    }

    private void OnLevelCompleted()
    {
        if (_eventBus != null)
        {

        }
    }

    ~LevelCompletion()
    {
        RemoveListeners();
    }
}
