using UnityEngine;



public class FollowingCamera : MonoBehaviour
{
    [Header("Objects for following")]
    [SerializeField] private GameObject _target;

    [Range(0.0f, 1.0f)] public float smoothness = 0.5f;
    private float _speed = 8.0f;
    private float _z = -10.0f;

    private Vector3 _camera;
    private Vector3 _targetPos => _target.transform.position; //shorthan

    private void OnValidate()
    {
        if (!_target)
            Debug.LogError("Following Camera: _target GameObject not found! The component should be attached.");
    }

    void Start()
    {
        _camera = new Vector3(_targetPos.x, _targetPos.y, _z);
    }

    void Update()
    {
        CameraMove();
    }

    void CameraMove()
    {

        _camera = Vector3.Lerp(_camera, _targetPos, _speed * Time.deltaTime);
        _camera.z = _z;
        this.transform.position = _camera;
    }

}
