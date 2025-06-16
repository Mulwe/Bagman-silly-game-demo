using TMPro;
using UnityEngine;


// Add effect when active
public class Greeting : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textMeshPro;

    private void Awake()
    {
        _textMeshPro = GetComponent<TextMeshProUGUI>();
        _textMeshPro.enabled = true;
    }


}
