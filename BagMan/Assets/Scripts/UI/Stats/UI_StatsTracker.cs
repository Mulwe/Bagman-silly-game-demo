using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//HUD Container
//хранит ссылки, другие родственные виджеты их достают.
public class UI_StatsTracker : MonoBehaviour
{
    List<Transform> AllProperties;
    private CharacterStats _playerStats;
    private EventBus _EventBus;
    private GameManager _gameManager;

    private bool _isInit = false;

    public bool IsInit => _isInit;
    public EventBus EventBus => _EventBus;
    public CharacterStats PlayerStats => _playerStats;

    public void Initialize(GameManager gm)
    {
        _EventBus = gm.EventBus;
        _gameManager = gm;
        //playerStats еще не инициализированы
        StartCoroutine(WaitInitialization());
    }

    IEnumerator WaitInitialization()
    {
        while (_playerStats == null || _isInit != true)
        {
            if (_gameManager != null)
            {
                if (_gameManager.PlayerStats != null)
                {
                    _playerStats = _gameManager.PlayerStats;
                    _isInit = true;
                }
            }
            yield return new WaitForSeconds(0.1f);
        }
    }


    private void OnValidate()
    {
        if (!this.gameObject.CompareTag("UI_HUD_Container"))
            Debug.LogError("Wrong HUD container");
    }

    //GetComponentsInChildren<Transform>(); все уровни вложений

    private void Start()
    {
        AllProperties = new List<Transform>();
        if (this.gameObject.CompareTag("UI_HUD_Container"))
        {
            foreach (Transform t in transform)
            {
                AllProperties.Add(t);
            }
        }
    }

    private void OnDestroy()
    {
        AllProperties.Clear();
        AllProperties = null;
    }

}
