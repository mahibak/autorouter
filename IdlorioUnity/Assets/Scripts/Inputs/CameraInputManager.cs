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
        Vector3 areaSize = FactorySizeManager.GetCurrentSize();
        Vector3 worldCenter = areaSize * 0.5f;
        areaSize += new Vector3(5f, 0f, 5f); // The added tolerance lets us see some of the wall
        float vertFovTan = Mathf.Tan(Camera.main.fieldOfView * Mathf.Deg2Rad * 0.5f);
        float mouseToGroundScaleBefore = vertFovTan * _targetCameraPos.y * 2f / Screen.height;
        float mouseToGroundScaleAfter = mouseToGroundScaleBefore;

        // Zoom with scroll
        float scrollAmount = Input.mouseScrollDelta.y * -5f;
        if (Mathf.Abs(scrollAmount) > Mathf.Epsilon)
        {
            _targetCameraPos.y += Input.mouseScrollDelta.y * -5f;

            float maxYVert = (areaSize.z * 0.5f) / vertFovTan;
            float maxYHor = (areaSize.x * 0.5f) / (vertFovTan * Camera.main.aspect);
            _targetCameraPos.y = Mathf.Clamp(_targetCameraPos.y, 50f, Mathf.Min(maxYVert, maxYHor));

            // Center zoom around mouse pos
            mouseToGroundScaleAfter = vertFovTan * _targetCameraPos.y * 2f / Screen.height;
            Vector2 mouseDistFromCenter = new Vector2(Input.mousePosition.x - Screen.width * 0.5f, Input.mousePosition.y - Screen.height * 0.5f);
            Vector2 worldDistFromCenterDelta = mouseDistFromCenter * (mouseToGroundScaleBefore - mouseToGroundScaleAfter);

            _targetCameraPos.x += worldDistFromCenterDelta.x;
            _targetCameraPos.z += worldDistFromCenterDelta.y;
        }
        
        // Move with right click drag
        if (Input.GetMouseButton(1) && !Input.GetMouseButtonDown(1))
        {
            Vector3 mouseDelta = Input.mousePosition - _lastMousePos;
            _targetCameraPos.x += mouseDelta.x * -mouseToGroundScaleAfter;
            _targetCameraPos.z += mouseDelta.y * -mouseToGroundScaleAfter;
        }

        Vector3 worldSizeInScreen = new Vector3(Screen.width * mouseToGroundScaleAfter, 0f, Screen.height * mouseToGroundScaleAfter);
        Vector3 scrollableWorldSizeHalf = (areaSize - worldSizeInScreen) * 0.5f;
        _targetCameraPos.x = Mathf.Clamp(_targetCameraPos.x, worldCenter.x - scrollableWorldSizeHalf.x, worldCenter.x + scrollableWorldSizeHalf.x);
        _targetCameraPos.z = Mathf.Clamp(_targetCameraPos.z, worldCenter.z - scrollableWorldSizeHalf.z, worldCenter.z + scrollableWorldSizeHalf.z);

        _lastMousePos = Input.mousePosition;
        _vectorSmoother.SetTarget(_targetCameraPos);
        _vectorSmoother.Update();
        Camera.main.transform.position = _vectorSmoother.GetValue();
    }
}
