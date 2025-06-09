using TMPro;
using UnityEngine;

public class GlowingMaterial : MonoBehaviour
{
    [SerializeField] TMP_Text _text;
    private Material _material;

    void Start()
    {
        _text = GetComponent<TMP_Text>();
        _material = _text.fontMaterial;
    }

    void ChangeGlowPower()
    {
        float glow = Mathf.PingPong(Time.time, 0.5f);
        if (_material != null && _material.HasProperty("_GlowPower"))
        {
            _material.SetFloat("_GlowPower", glow);
        }
    }

    private void Update()
    {
        ChangeGlowPower();
    }

}
