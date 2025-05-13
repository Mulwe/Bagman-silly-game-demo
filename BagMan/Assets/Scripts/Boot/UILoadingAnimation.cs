using System;
using TMPro;
using UnityEngine;


[RequireComponent(typeof(TextMeshProUGUI))]
public class UILoadingAnimation : MonoBehaviour
{
    [Header("TextMeshProUGUI:")]
    public TextMeshProUGUI _TextMeshProUGUI;
    string _text => _TextMeshProUGUI.text;
    private float _timer = 0.0f;
    private float _interval = 0.8f;
    private int _loadFull = "Loading...".Length;

    void Start()
    {
        _TextMeshProUGUI = GetComponent<TextMeshProUGUI>();
        _TextMeshProUGUI.text = "Loading";
    }

    // Update is called once per frame
    void Update()
    {
        if (_TextMeshProUGUI != null)
        {
            ChangeText();
        }

    }

    void ChangeText()
    {
        _timer += Time.deltaTime;
        if (_timer >= _interval)
        {
            _TextMeshProUGUI.text += ".";
            _timer = 0f;
        }
        if (_TextMeshProUGUI.text.Length > _loadFull)
            _TextMeshProUGUI.text = _TextMeshProUGUI.text.AsSpan(0, _TextMeshProUGUI.text.Length - 4).ToString();
    }
}
