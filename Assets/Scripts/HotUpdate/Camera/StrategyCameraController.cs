using UnityEngine;
using GameFramework.CameraSystem;
using Lean.Touch;

namespace GameFramework.Gameplay
{
    public class StrategyCameraController : MonoBehaviour
    {
        [Header("Movement Settings")]
        public float moveSpeed = 20f;
        public float boostMultiplier = 2f;
        public float smoothTime = 0.1f;

        [Header("Rotation Settings")]
        public float rotationSpeed = 2f;

        [Header("Pitch Settings")]
        public float pitchSpeed = 2f;
        public float minPitch = 0f;
        public float maxPitch = 30f; // 初始加载时将使用此最大角度

        [Header("Zoom Settings")]
        public float zoomSpeed = 10f;
        public float minFOV = 20f;
        public float maxFOV = 80f; // 初始加载时将使用此最大FOV

        [Header("Boundaries")]
        public bool useBounds = true;
        public Vector2 minBounds = new Vector2(-50, -50);
        public Vector2 maxBounds = new Vector2(50, 50);

        [Header("Mobile / Edge Settings")]
        public bool enableEdgeScroll = true;
        public float edgeBorder = 0;
        public float touchPanSensitivity = 0.25f;
        public float touchRotSensitivity = 0.5f;

        private Vector3 _targetPosition;
        private Vector3 _currentVelocity;
        private float _targetFOV = 60f;

        private float _currentYaw;
        private float _currentPitch;

        private void Start()
        {
            _targetPosition = transform.position;
            if (UnityEngine.Camera.main != null)
                _targetFOV = UnityEngine.Camera.main.fieldOfView;

            _currentYaw = transform.eulerAngles.y;
            _currentPitch = transform.eulerAngles.x;
        }

        private void Update()
        {
            HandleMovement();
            HandleRotation();
            HandleZoom();
        }

        // ===================================================================================
        // [新增] 1. 初始化视角设置 (视野最大，俯瞰全局)
        // ===================================================================================
        public void SetInitialView(Vector3 targetPos)
        {
            // 1. 位置限制与设定
            if (useBounds)
            {
                targetPos.x = Mathf.Clamp(targetPos.x, minBounds.x, maxBounds.x);
                targetPos.z = Mathf.Clamp(targetPos.z, minBounds.y, maxBounds.y);
            }
            targetPos.y = transform.position.y; // 保持高度不变

            // 瞬间移动
            transform.position = targetPos;
            _targetPosition = targetPos;
            _currentVelocity = Vector3.zero;

            // 2. 视野(FOV)设为最大 (广角)
            _targetFOV = maxFOV;
            CameraController.Instance.ZoomCamera(_targetFOV, 0f); // 0秒瞬间切换

            // 3. 俯仰角(Pitch)设为最大 (高角度俯视)
            _currentPitch = maxPitch;
            transform.rotation = Quaternion.Euler(_currentPitch, _currentYaw, 0f);

            Debug.Log($"[StrategyCamera] 初始化视角完成: Pos:{targetPos}, FOV:{maxFOV}, Pitch:{maxPitch}");
        }

        // ===================================================================================
        // [修改] 2. 普通聚焦 (视野缩小，特写细节)
        // ===================================================================================
        public void FocusOn(Vector3 targetPos)
        {
            if (useBounds)
            {
                targetPos.x = Mathf.Clamp(targetPos.x, minBounds.x, maxBounds.x);
                targetPos.z = Mathf.Clamp(targetPos.z, minBounds.y, maxBounds.y);
            }
            targetPos.y = transform.position.y;

            // 设置目标位置 (Update 中会平滑移动过去)
            _targetPosition = targetPos;

            // 聚焦时，将视野调整到较小值 (例如 minFOV 和 60 的中间值，或者直接 minFOV)
            // 这里取一个舒适的聚焦值，比如 minFOV + 10
            float focusFOV = Mathf.Min(minFOV + 15f, maxFOV);
            _targetFOV = focusFOV;

            // 平滑变焦 (1秒)
            CameraController.Instance.ZoomCamera(_targetFOV, 1f);

            Debug.Log($"[StrategyCamera] 聚焦目标: {targetPos}, FOV缩放至: {focusFOV}");
        }

        private void HandleMovement()
        {
            // (保持原有逻辑不变)
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");

            if (enableEdgeScroll)
            {
                if (Input.mousePosition.x >= 0 && Input.mousePosition.y >= 0 &&
                    Input.mousePosition.x <= Screen.width && Input.mousePosition.y <= Screen.height)
                {
                    if (Input.mousePosition.x > Screen.width - edgeBorder) h = 1;
                    else if (Input.mousePosition.x < edgeBorder) h = -1;
                    if (Input.mousePosition.y > Screen.height - edgeBorder) v = 1;
                    else if (Input.mousePosition.y < edgeBorder) v = -1;
                }
            }

            var camTransform = UnityEngine.Camera.main.transform;
            var forward = camTransform.forward;
            var right = camTransform.right;
            forward.y = 0;
            right.y = 0;
            forward.Normalize();
            right.Normalize();

            Vector3 direction = (forward * v + right * h).normalized;
            float speed = Input.GetKey(KeyCode.LeftShift) ? moveSpeed * boostMultiplier : moveSpeed;

            var fingers = LeanTouch.GetFingers(true, false);
            if (fingers.Count > 0)
            {
                Vector2 screenDelta = LeanGesture.GetScreenDelta(fingers);
                if (screenDelta.sqrMagnitude > 0.0f)
                {
                    Vector3 touchMove = -(forward * screenDelta.y + right * screenDelta.x) * touchPanSensitivity * 0.1f;
                    _targetPosition += touchMove;
                }
            }

            if (direction.magnitude >= 0.1f)
            {
                _targetPosition += direction * speed * Time.deltaTime;
            }

            if (useBounds)
            {
                _targetPosition.x = Mathf.Clamp(_targetPosition.x, minBounds.x, maxBounds.x);
                _targetPosition.z = Mathf.Clamp(_targetPosition.z, minBounds.y, maxBounds.y);
            }

            transform.position = Vector3.SmoothDamp(transform.position, _targetPosition, ref _currentVelocity, smoothTime);
        }

        private void HandleRotation()
        {
            // (保持原有逻辑不变)
            float yawDelta = 0f;
            float pitchDelta = 0f;

            if (Input.GetMouseButton(1))
            {
                yawDelta += Input.GetAxis("Mouse X") * rotationSpeed;
                pitchDelta -= Input.GetAxis("Mouse Y") * pitchSpeed;
            }

            var fingers = LeanTouch.GetFingers(true, false);
            if (fingers.Count >= 2)
            {
                float twistDegrees = LeanGesture.GetTwistDegrees(fingers);
                yawDelta += twistDegrees * touchRotSensitivity;
            }

            if (Mathf.Abs(yawDelta) > 0.001f || Mathf.Abs(pitchDelta) > 0.001f)
            {
                _currentYaw += yawDelta;
                _currentPitch += pitchDelta;
                _currentPitch = Mathf.Clamp(_currentPitch, minPitch, maxPitch);
                transform.rotation = Quaternion.Euler(_currentPitch, _currentYaw, 0f);
            }
        }

        private void HandleZoom()
        {
            // (保持原有逻辑不变)
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            float zoomChange = 0f;

            if (Mathf.Abs(scroll) > 0.01f)
            {
                zoomChange = -scroll * zoomSpeed * 100f * Time.deltaTime;
            }

            var fingers = LeanTouch.GetFingers(true, false);
            if (fingers.Count >= 2)
            {
                float pinchScale = LeanGesture.GetPinchScale(fingers);
                if (Mathf.Abs(pinchScale - 1.0f) > 0.001f)
                {
                    float scaleFactor = 1.0f / pinchScale;
                    _targetFOV *= scaleFactor;
                    goto ApplyZoom;
                }
            }

            _targetFOV += zoomChange;

        ApplyZoom:
            _targetFOV = Mathf.Clamp(_targetFOV, minFOV, maxFOV);
            CameraController.Instance.ZoomCamera(_targetFOV, 0.1f);
        }
    }
}