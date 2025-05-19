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

    public void SetSpeed(float value)
    {
        if (transform.parent.name.Equals(HUD_Speed))
            SetText($"{value:F1}");
    }

    public void SetCount(float value)
    {
        if (transform.parent.name.Equals(HUD_Count))
        {
            SetText($"{value:F0}");
        }
    }
    private void OnEnable()
    {
        _textMeshProUGUI = GetComponent<TextMeshProUGUI>();
    }

    private void SetText(string text)
    {
        this._textMeshProUGUI.text = text;
    }

}