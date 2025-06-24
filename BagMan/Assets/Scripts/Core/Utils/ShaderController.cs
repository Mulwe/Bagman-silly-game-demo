using System.Collections;
using UnityEngine;


public class ShaderController : MonoBehaviour
{
    [SerializeField] private DropZoneController _dropzone;
    [SerializeField] private GameObject _outline;
    [Header("Max & min Alpha color:")]
    [SerializeField] private const float _min = 0f;
    [SerializeField] private const float _max = 0.8f;
    private SpriteRenderer _sp;

    private Coroutine _coroutine;
    private float _duration = 1f;

    private void OnValidate()
    {
        if (_dropzone == null)
            Debug.LogError("No reference for DropZoneController  ");
        if (_outline == null)
            Debug.LogError("No reference for Outline: GameObject with SpriteRenderer");
    }

    private void Awake()
    {
        AddListeners();
        if (_outline == null)
            _outline = transform.GetChild(0).gameObject;
        if (_outline != null)
            _sp = _outline.GetComponent<SpriteRenderer>();
        _coroutine = null;
    }

    private void OnDisable()
    {
        if (_coroutine != null)
            StopCoroutine(_coroutine);
        _coroutine = null;
        RemoveListeners();
    }


    private void DeactivateOutlineSprite()
    {
        if (_sp != null)
            ChangeAlpha(_sp.color, 0f);
    }

    private void OnStopOutline()
    {
        if (_coroutine != null)
            StopCoroutine(_coroutine);
        _coroutine = null;
        DeactivateOutlineSprite();
        if (!_outline.activeSelf)
            _outline.SetActive(true);
    }

    private void AddListeners()
    {
        if (_dropzone != null)
        {
            _dropzone.ToogleOutline += OnOutlineIntensity;
            _dropzone.TurnOffOutline += OnStopOutline;
        }
    }
    private void RemoveListeners()
    {
        if (_dropzone != null)
        {
            _dropzone.ToogleOutline -= OnOutlineIntensity;
            _dropzone.TurnOffOutline -= OnStopOutline;
        }
    }

    private void OnOutlineIntensity(bool state, float duration)
    {
        if (!_outline.activeSelf)
            _outline.SetActive(true);
        _duration = duration;
        if (state && _coroutine == null)
            _coroutine = StartCoroutine(PulseAlpha());
        if (!state && _coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
            DeactivateOutlineSprite();
            _outline.SetActive(false);
        }
    }

    IEnumerator PulseAlpha()
    {
        while (true)
        {
            yield return StartCoroutine(FadeAlpha(_max, _min, _duration / 2f));
            yield return StartCoroutine(FadeAlpha(_min, _max, _duration / 2f));
        }
    }

    IEnumerator FadeAlpha(float start, float end, float seconds)
    {
        if (_sp != null)
        {
            float elapsed = 0f;
            float alpha = 0;
            while (elapsed < seconds)
            {
                alpha = Mathf.Lerp(start, end, elapsed / seconds);
                _sp.color = ChangeAlpha(_sp.color, alpha);
                elapsed += Time.deltaTime;
                yield return null;
            }
            _sp.color = ChangeAlpha(_sp.color, end);
        }
    }

    private UnityEngine.Color ChangeAlpha(UnityEngine.Color current, float alpha)
    {
        return new UnityEngine.Color(current.r, current.g, current.b, alpha);
    }

}
