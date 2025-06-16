using TMPro;
using UnityEngine;

public class NameTitles : MonoBehaviour
{
    [SerializeField] private bool _autoNaming = true;
    private TextMeshProUGUI _text;



    private void Awake()
    {
        _text = gameObject.GetComponent<TextMeshProUGUI>();
        if (_text != null)
        {
            string s = transform.parent.name;
            _text.text = s.Substring(0, s.IndexOf(' '));
        }
    }
}
