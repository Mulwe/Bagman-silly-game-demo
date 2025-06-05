using TMPro;
using UnityEngine;

public class FloatingTip : MonoBehaviour
{
    [SerializeField] private PlayerCartController _player;

    private TextMeshPro _text;
    private MeshRenderer _meshRenderer;
    private Animator _animator;
    private float _fadeDelay = 5.0f;


    private Coroutine _coroutine;


    private void OnValidate()
    {
        if (_player == null)
            Debug.LogError($"{nameof(_player)} is not assigned in the Inspector on {gameObject.name}");
    }

    private void Start()
    {
        _text = transform.GetComponent<TextMeshPro>();
        _animator = transform.GetComponent<Animator>();
        _animator.enabled = true;
        Initialization();
    }


    public void ChangeMessage(string msg)
    {
        if (_text != null && !_text.text.Equals(msg))
            _text.text = msg;
    }

    private void Initialization()
    {
        if (_player == null)
            _player = transform.parent.GetComponent<PlayerCartController>();
        if (_text == null || _animator == null || _player == null)
        {
            Debug.LogError($":null");
            return;
        }

        _meshRenderer = _text.GetComponent<MeshRenderer>();
        _meshRenderer.enabled = false;
        _animator.enabled = true;
    }

    //дочинить
    //не пропадает



    public void HandleShowRequest()
    {

        if (_meshRenderer != null)
        {
            if (!_meshRenderer.enabled)
            {
                _meshRenderer.enabled = true;
            }
        }
    }

    public void HandleHideRequest()
    {

        if (_meshRenderer != null)
        {
            if (_meshRenderer.enabled && _coroutine == null)
            {
                _meshRenderer.enabled = false;
            }
        }
    }


}


