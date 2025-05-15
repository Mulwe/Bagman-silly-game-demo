using UnityEngine;
using UnityEngine.UI;


[DisallowMultipleComponent]
public class UI_ProgressBar : MonoBehaviour
{
    [SerializeField] private Image imgFiller;
    //[SerializeField] private Text textValue;
    private float _prevValue = 0;


    private void OnEnable()
    {
        imgFiller = GetComponent<Image>();
    }

    public void SetValue(float valueNormilized)
    {

        this.imgFiller.fillAmount = valueNormilized;

        //var valueInPercent = Mathf.RoundToInt(valueNormilized * 100f);
        // this.textValue.text = $"{valueInPercent}%";
        // для плавного перхода моджно использовать update или корутину..
    }
}


