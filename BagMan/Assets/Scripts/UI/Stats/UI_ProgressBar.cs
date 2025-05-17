using UnityEngine;
using UnityEngine.UI;


[DisallowMultipleComponent]
public class UI_ProgressBar : MonoBehaviour
{
    [SerializeField] private Image imgFiller;
    //[SerializeField] private Text textValue;

    private float _minStamina = 0f;
    private float _maxStamina;

    //normilized values
    private float _currentStamina;
    private float _displayedStamina;

    //
    private float _fillspeed = 3f;


    private void OnEnable()
    {
        imgFiller = GetComponent<Image>();
        if (imgFiller != null)
        {
            _displayedStamina = this.imgFiller.fillAmount;
            _currentStamina = this.imgFiller.fillAmount;
        }
    }

    //normilize - not-normilized values
    public void SetValue(float currentStamina, float maxStamina)
    {
        _currentStamina = NormilizeValue(currentStamina, _minStamina, maxStamina);
        _maxStamina = maxStamina;
        _displayedStamina = this.imgFiller.fillAmount;
    }

    private float NormilizeValue(float value, float min, float max)
    {
        if (min == max)
            return 0f;
        return Mathf.Clamp01((value - min) / (max - min));
    }

    public void InterpolateValue()
    {
        _displayedStamina = Mathf.Lerp(_displayedStamina, _currentStamina, Time.deltaTime * _fillspeed);
        imgFiller.fillAmount = _displayedStamina;
    }

    private void Update()
    {
        if (_displayedStamina != _currentStamina)
            InterpolateValue();
    }
}


