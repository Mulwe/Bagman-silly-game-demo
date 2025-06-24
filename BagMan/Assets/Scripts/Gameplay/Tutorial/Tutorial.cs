using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


[System.Serializable]
public class FocusTarget
{
    public GameObject target;
    public float delay;
    public float speed;
    public string text;
    //public float zoom;

    public FocusTarget(GameObject target, float speed, float delay)
    {
        this.target = target;
        this.delay = delay;
        this.speed = speed;
        this.text = target.name.ToString();
    }
}

public struct Tip
{
    public GameObject obj;
    public TextMeshPro text;
    public Tip(GameObject data, TextMeshPro text)
    {
        this.obj = data;
        this.text = text;
    }
}

public class Tutorial : MonoBehaviour
{
    [SerializeField] private Camera _camera;

    [SerializeField] private DropZoneController _zone;
    [Header("Gameobject List for tutorial:")]
    [SerializeField] private List<FocusTarget> _lst = new List<FocusTarget>();

    private Tip _tip;
    private FollowingCamera _followingCamera;
    private Coroutine _coroutine;

    public bool IsFinished { get; private set; }

    public event Action TutorialFinished;

    public void SetDropZoneController(DropZoneController zone)
    {
        this._zone = zone;
    }

    private void Start()
    {
        IsFinished = false;
        if (_camera != null)
        {
            _followingCamera = _camera.GetComponent<FollowingCamera>();
            GameObject obj = new("Tip");
            TextMeshPro text = SetupText(obj);
            _tip = new Tip(obj, text);
        }
    }

    public void StartTutorial()
    {
        if (_zone == null)
        {
            Debug.LogError("DropZone not init");
            return;
        }
        AddListeners();
        TrackTargets(_lst);
    }

    public void InterruptTutorial()
    {

        if (_followingCamera != null)
        {
            _followingCamera.StopTrackingTargets();
        }
        StopAllCoroutines();
        OnTutorialEnds();
    }

    private void TrackTargets(List<FocusTarget> _lst)
    {
        if (_lst != null && _followingCamera != null)
        {
            _followingCamera.ReceiveListOfTargets(_lst, this);
        }
        else
            Debug.Log("Error");
    }

    private void OnTutorialEnds()
    {
        HideTextBox();
        RemoveListeners();
        _zone?.OnTurnOffOutline();
        IsFinished = true;
        TutorialFinished?.Invoke();//startGameplay
    }

    private void OnFocusComplete(FocusTarget data)
    {
        _coroutine = StartCoroutine(WaitText(data, data.delay * 0.9f));
    }

    IEnumerator WaitText(FocusTarget data, float timespan)
    {
        ShowTextBox(data);
        yield return new WaitForSeconds(timespan);
        _zone.OnTurnOffOutline();
        HideTextBox();
    }



    private void OnDisable()
    {
        RemoveListeners();
        if (_coroutine != null)
            StopCoroutine(_coroutine);
        if (_tip.obj)
        {
            _tip.obj.SetActive(false);
            UnityEngine.Object.Destroy(_tip.obj);
            _tip.obj = null;
        }
        if (_lst != null && _lst.Count > 0)
            _lst.Clear();

    }

    private void AddListeners()
    {
        _followingCamera.OnTargetReached += OnFocusComplete;
        _followingCamera.OnStartPulseDropZone += OnZoneStartPulse;
        _followingCamera.OnTargetReachedAll += OnTutorialEnds;
    }
    private void RemoveListeners()
    {
        if (_followingCamera != null)
        {
            _followingCamera.OnTargetReached -= OnFocusComplete;
            _followingCamera.OnTargetReachedAll -= OnTutorialEnds;
            _followingCamera.OnStartPulseDropZone -= OnZoneStartPulse;
        }
    }

    private void OnZoneStartPulse(bool state, float secDelay)
    {
        if (_zone != null)
            _zone.StartOutlinePulse(state, secDelay);
    }

    //Tip object
    private TextMeshPro SetupText(GameObject obj)
    {

        TextMeshPro text = obj.AddComponent<TextMeshPro>();

        if (text != null)
        {
            obj.SetActive(false);
            text.enabled = true;
            text.color = Color.white;
            text.fontSize = 36;
            text.alignment = TextAlignmentOptions.Center;
            text.sortingOrder = 300;
            text.enableAutoSizing = false;
            text.transform.localScale = Vector3.one * 0.3f;

            Material mat = new Material(text.fontMaterial);
            mat.EnableKeyword("UNDERLAY_ON");

            mat.SetColor(ShaderUtilities.ID_UnderlayColor, new Color(0, 0, 0, 0.9f));
            mat.SetFloat(ShaderUtilities.ID_UnderlayOffsetX, 1f);
            mat.SetFloat(ShaderUtilities.ID_UnderlayOffsetY, -1f);
            mat.SetFloat(ShaderUtilities.ID_UnderlayDilate, 1f);
            text.text = "Test";
            text.fontMaterial = mat;
        }
        return text;
    }

    private void ShowTextBox(FocusTarget data)
    {
        if (_tip.obj != null)
        {
            Vector3 pos = data.target.transform.position;
            Bounds b = data.target.GetComponent<Renderer>().bounds;
            if (b != null)
                _tip.obj.transform.position = pos + new Vector3(0, b.size.y / 2, 0);
            else
                _tip.obj.transform.position = pos + new Vector3(0, 2f, 0);
            _tip.text.text = data.text;
            _tip.obj.SetActive(true);
        }
    }

    private void HideTextBox()
    {
        if (_tip.obj != null)
        {
            _tip.obj.SetActive(false);
        }
    }


}
