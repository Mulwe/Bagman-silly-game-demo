using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class FollowingCamera : MonoBehaviour
{
    [Header("Objects for following")]
    [SerializeField] private GameObject _target;
    [Range(0.0f, 1.0f)] private float smoothness = 0.5f;
    private float _speed = 8.0f;
    private readonly float _z = -10.0f;
    private Vector3 _camera;
    private Vector3 _targetPos => _target.transform.position; //shorthan
    private bool _isFollowingPlayer = true;
    private Coroutine _coroutine;

    public event Action<FocusTarget> OnTargetReached;
    public event Action OnTargetReachedAll;
    public event Action<bool, float> OnStartPulseDropZone;

    public void StopTrackingTargets()
    {
        StopAllCoroutines();
        StartCoroutine(FocusOnPlayer());
    }

    public void ReceiveListOfTargets(List<FocusTarget> lstTargets, Tutorial t)
    {
        _coroutine ??= StartCoroutine(CameraTargets(lstTargets));
    }

    IEnumerator FocusOnPlayer()
    {
        yield return SmoothCameraFocus(_target, _speed);
        _isFollowingPlayer = true;
    }

    private void OnValidate()
    {
        if (!_target)
            Debug.LogError("Following Camera: _target GameObject not found! The component should be attached.");
    }

    private void Start()
    {
        if (_target != null)
        {
            _isFollowingPlayer = true;
            _camera = new Vector3(_targetPos.x, _targetPos.y, _z);
            this.transform.position = _target.transform.position;
        }
        _coroutine = null;

    }

    private void Update()
    {
        if (_isFollowingPlayer == true)
            CameraMove(_target);

    }

    private void CameraMove(GameObject target)
    {
        _camera = Vector3.Lerp(_camera, target.transform.position, _speed * smoothness * Time.deltaTime);
        _camera.z = _z;
        this.transform.position = _camera;
    }

    private IEnumerator CameraTargets(List<FocusTarget> lstTargets)
    {
        if (lstTargets == null || lstTargets.Count == 0) yield break;

        StopFollowingTarget();
        foreach (FocusTarget data in lstTargets)
        {
            if (data.target != null)
            {
                OnStartPulseDropZone.Invoke(true, data.delay);
                if (!data.target.Equals(_target))
                {
                    StopFollowingTarget();
                    yield return StartCoroutine(SmoothCameraFocus(data.target, data.speed));
                }
                else
                {
                    KeepFollowingTarget();
                }
                OnTargetReached.Invoke(data);
                yield return new WaitForSeconds(data.delay);
            }
        }
        yield return null;
        OnTargetReachedAll.Invoke();
        KeepFollowingTarget();
        _coroutine = null;
    }

    private IEnumerator SmoothCameraFocus(GameObject obj, float speed)
    {
        if (obj != null && _camera != null)
        {
            //ignores z pos
            while (Vector2.Distance(_camera, obj.transform.position) > 0.1f)
            {
                CameraMove(obj);
                yield return null;
            }
        }
    }

    void StopFollowingTarget()
    {
        _isFollowingPlayer = false;
    }

    void KeepFollowingTarget()
    {
        _isFollowingPlayer = true;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        _isFollowingPlayer = true;
    }


}
