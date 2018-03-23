using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraInputManager : MonoBehaviour
{
    private static CameraInputManager _instance;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private const float _smoothTime = 0.05f;
    private VectorSmoother _vectorSmoother = new VectorSmoother(_smoothTime);
    private Vector3 _targetCameraPos;
    private Vector3 _lastMousePos;

    private void Start()
    {
        _targetCameraPos = Camera.main.transform.position;
        _vectorSmoother.Init(_targetCameraPos);
        _lastMousePos = Input.mousePosition;
    }

    private void Update()
    {
        _targetCameraPos.y += Input.mouseScrollDelta.y * -5f;

        if (Input.GetMouseButton(1))
        {
            Vector3 mouseDelta = Input.mousePosition - _lastMousePos;
            float mouseToGroundScale = Mathf.Tan(Camera.main.fieldOfView * Mathf.Deg2Rad * 0.5f) * _targetCameraPos.y * 2f / Screen.height;
            _targetCameraPos.x += mouseDelta.x * -mouseToGroundScale;
            _targetCameraPos.z += mouseDelta.y * -mouseToGroundScale;
        }

        _lastMousePos = Input.mousePosition;

        _vectorSmoother.SetTarget(_targetCameraPos);
        _vectorSmoother.Update();
        
        Camera.main.transform.position = _vectorSmoother.GetValue();
    }
}
