using TMPro;
using UnityEngine;

public class Greeting : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textMeshPro;

    private void Awake()
    {
        _textMeshPro = GetComponent<TextMeshProUGUI>();
        _textMeshPro.enabled = true;
    }


}
