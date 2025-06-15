using System.Collections;
using TMPro;
using UnityEngine;

public enum TemperatureUnit
{
    Celsius,
    Fahrenheit
}

[DisallowMultipleComponent]
public class UI_TextData : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textMeshProUGUI;
    private Transform _parent;
    public UI_Tags _tags;
    private string _parentName;

    public string ParentName => _parentName;

    private bool _isInit = false;
    private float _currentValue;
    private float _newValue;

    Coroutine _coroutine = null;

    //---------Names---------------------------------------
    [Header("UI HUD Container names of fields")]
    public const string HUD_Stamina = "HUD_Stamina";
    public const string HUD_Temerature = "HUD_Temperature";
    public const string HUD_Speed = "HUD_Speed";
    public const string HUD_Count = "HUD_Count";



    private void Awake()
    {
        _parent = transform.parent;
        if (_parent != null)
            _parentName = _parent.name;
        _isInit = false;

    }

    public void SetTemperature(float value, TemperatureUnit unit)
    {
        if (transform.parent.name.Equals(HUD_Temerature))
        {
            if (unit == TemperatureUnit.Celsius)
            {
                SetText($"{value}°C");
            }
            else if (unit == TemperatureUnit.Fahrenheit)
            {
                SetText($"{value}°F");
            }
        }
    }

    public void SetCount(ulong value)
    {
        if (transform.parent.name.Equals(HUD_Count))
        {
            SetText($"{value:F0}");
        }
    }

    public void SetSpeed(float value)
    {
        if (transform.parent.name.Equals(HUD_Speed))
        {
            _newValue = value;
            NormilizeValue();
        }
    }

    public void NormilizeValue()
    {
        if (_isInit && Mathf.Abs(_currentValue - _newValue) > 0.001f)
        {
            if (_coroutine != null)
                StopCoroutine(_coroutine);
            _coroutine = StartCoroutine(Normilize());
        }
        else
        {
            _currentValue = _newValue;
            SetText($"{_newValue:F1}");
        }
    }


    IEnumerator Normilize()
    {
        float smoothSpeed = 5f;
        float threshold = 0.05f; //порог или шаг

        while (Mathf.Abs(_currentValue - _newValue) > threshold)
        {

            _currentValue = Mathf.Lerp(_currentValue, _newValue, smoothSpeed * Time.deltaTime);
            SetText($"{_currentValue:F1}");
            yield return new WaitForSeconds(0.1f);
        }
        _currentValue = _newValue;
        SetText($"{_currentValue:F1}");
        _coroutine = null;
    }

    private void OnEnable()
    {
        _textMeshProUGUI = GetComponent<TextMeshProUGUI>();
        _isInit = false;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private void SetText(string text)
    {
        this._textMeshProUGUI.text = text;
        if (!_isInit)
        {
            _currentValue = float.Parse(text);
            if (_currentValue < 0) _currentValue = 0;
            _isInit = true;
        }
    }

}