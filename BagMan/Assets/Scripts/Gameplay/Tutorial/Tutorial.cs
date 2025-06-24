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
    [Header("Gameobject List for tutorial:")]
    [SerializeField] private List<FocusTarget> _lst = new List<FocusTarget>();

    private Tip _tip;
    // [SerializeField] private List<GameObject> _objs;
    private FollowingCamera _followingCamera;
    private Coroutine _coroutine;
    OutlineFx.Outline _outline;

    private void Start()
    {
        if (_camera != null)
        {
            _followingCamera = _camera.GetComponent<FollowingCamera>();
            GameObject obj = new("Tip");
            TextMeshPro text = SetupText(obj);
            _outline = GetComponent<OutlineFx.Outline>();
            _tip = new Tip(obj, text);
        }
    }

    public void StartTutorial()
    {
        TrackTargets(_lst);
    }

    private TextMeshPro SetupText(GameObject obj)
    {
        TextMeshPro text = obj.AddComponent<TextMeshPro>();


        if (text != null)
        {
            text.enabled = false;
            text.text = "Test";
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
            text.fontMaterial = mat;

        }
        return text;
    }


    private void TrackTargets(List<FocusTarget> _lst)
    {
        if (_lst != null && _followingCamera != null)
        {
            _followingCamera.OnTargetReached += OnFocusComplete;
            _followingCamera.ReceiveListOfTargets(_lst, this);
        }
        else
            Debug.Log("Error");
    }

    IEnumerator WaitText(float timespan)
    {
        yield return new WaitForSeconds(timespan);
        HideTextBox();
    }

    private void OnFocusComplete(FocusTarget data)
    {
        ShowTextBox(data);
        StartCoroutine(WaitText(data.delay - 0.1f));
    }

    private void OnDisable()
    {
        if (_followingCamera != null)
        {
            _followingCamera.OnTargetReached -= OnFocusComplete;
        }
        if (_tip.obj)
        {
            _tip.obj.SetActive(false);
            Object.Destroy(_tip.obj);
            _tip.obj = null;
        }
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
            _tip.text.enabled = true;
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
