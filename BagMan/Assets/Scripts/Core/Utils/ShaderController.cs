using System.Collections;
using UnityEngine;

public class ShaderController : MonoBehaviour
{
    [SerializeField] private DropZoneController _dropzone;
    private OutlineFx.Outline _outline;
    private Coroutine _coroutine;
    private float _duration = 1f;


    private void Awake()
    {
        if (_dropzone != null)
            _dropzone.OnToogleOutline += OnOutlineIntensity;

        _coroutine = null;

    }

    private void OnDisable()
    {
        if (_coroutine != null)
            StopCoroutine(_coroutine);
        _coroutine = null;
        if (_dropzone != null)
            _dropzone.OnToogleOutline -= OnOutlineIntensity;

    }

    private void OnOutlineIntensity(bool state, float duration)
    {
        _duration = duration;
        if (state && _coroutine == null)
            _coroutine = StartCoroutine(PulseAlpha());
        if (!state && _coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
            ChangeAlpha(_outline.Color, 0f);
        }
    }

    IEnumerator PulseAlpha()
    {
        while (true)
        {
            yield return StartCoroutine(FadeAlpha(1f, 0f, _duration / 2f));
            yield return StartCoroutine(FadeAlpha(0f, 1f, _duration / 2f));
        }
    }

    IEnumerator FadeAlpha(float start, float end, float seconds)
    {
        if (_outline != null)
        {
            //Color origColor = _outline.Color;
            float elapsed = 0f;
            float alpha = 0;
            while (elapsed < seconds)
            {
                alpha = Mathf.Lerp(start, end, elapsed / seconds);
                _outline.Color = ChangeAlpha(_outline.Color, alpha);
                elapsed += Time.deltaTime;
                yield return null;
            }
            _outline.Color = ChangeAlpha(_outline.Color, end);
        }
    }

    private Color ChangeAlpha(Color current, float alpha)
    {
        return new Color(current.r, current.g, current.b, alpha);
    }


}
