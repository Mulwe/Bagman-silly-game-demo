using System.Collections;
using TMPro;
using UnityEngine;

public class FloatingTip : MonoBehaviour
{
    [SerializeField] private PlayerCartController _player;

    private TextMeshPro _text;
    private MeshRenderer _meshRenderer;
    private Animator _animator;

    private Coroutine _coroutine;
    private string _defaultText;


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
        _defaultText = "E";
        _text.text = _defaultText;
        _meshRenderer = _text.GetComponent<MeshRenderer>();
        _meshRenderer.enabled = false;
        _animator.enabled = true;
        _coroutine = null;
    }


    public void HandleShowRequest()
    {
        if (_meshRenderer != null)
        {
            if (!_meshRenderer.enabled && _coroutine == null)
            {
                _meshRenderer.enabled = true;
                _coroutine = StartCoroutine(TimeOut());
            }
        }
    }

    public void HandleShowRequest(string msg)
    {
        if (_meshRenderer != null)
        {
            if (!_meshRenderer.enabled && _coroutine == null)
            {
                if (_text != null)
                    _text.text = msg;
                _meshRenderer.enabled = true;
                _coroutine = StartCoroutine(TimeOut());
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

    private IEnumerator TimeOut()
    {
        yield return new WaitForSeconds(5f);
        if (_text != null && !_text.text.Equals(_defaultText))
        {
            _text.text = _defaultText;
        }
        _meshRenderer.enabled = false;
        _coroutine = null;
    }

}


