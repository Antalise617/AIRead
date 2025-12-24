using UnityEngine;
using GameFramework.CameraSystem;
using Lean.Touch; // 引用 LeanTouch

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
        public float maxPitch = 30f;

        [Header("Zoom Settings")]
        public float zoomSpeed = 10f;
        public float minFOV = 20f;
        public float maxFOV = 80f;

        [Header("Boundaries")]
        public bool useBounds = true;
        public Vector2 minBounds = new Vector2(-50, -50);
        public Vector2 maxBounds = new Vector2(50, 50);

        [Header("Mobile / Edge Settings (新增设置)")]
        [Tooltip("启用鼠标边缘滚动")]
        public bool enableEdgeScroll = true;
        [Tooltip("边缘检测的宽度（像素）")]
        public float edgeBorder = 0;
        [Tooltip("触摸移动的灵敏度")]
        public float touchPanSensitivity = 0.25f; // 根据需要调整，越小移动越慢
        [Tooltip("触摸旋转的灵敏度")]
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
            // 确保 InputManager 或 LeanTouch 已初始化，如果没有特殊依赖可直接运行
            HandleMovement();
            HandleRotation();
            HandleZoom();
        }

        private void HandleMovement()
        {
            // 1. 获取基础 WASD 输入
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");

            // 2. 处理鼠标边缘滚动 (Edge Scrolling)
            if (enableEdgeScroll)
            {
                // 只有在鼠标在窗口内时才检测
                if (Input.mousePosition.x >= 0 && Input.mousePosition.y >= 0 &&
                    Input.mousePosition.x <= Screen.width && Input.mousePosition.y <= Screen.height)
                {
                    if (Input.mousePosition.x > Screen.width - edgeBorder) h = 1;       // 右边缘
                    else if (Input.mousePosition.x < edgeBorder) h = -1;                // 左边缘

                    if (Input.mousePosition.y > Screen.height - edgeBorder) v = 1;      // 上边缘
                    else if (Input.mousePosition.y < edgeBorder) v = -1;                // 下边缘
                }
            }

            // 3. 计算移动方向 (键盘/边缘)
            var camTransform = UnityEngine.Camera.main.transform;
            var forward = camTransform.forward;
            var right = camTransform.right;
            forward.y = 0;
            right.y = 0;
            forward.Normalize();
            right.Normalize();

            Vector3 direction = (forward * v + right * h).normalized;
            float speed = Input.GetKey(KeyCode.LeftShift) ? moveSpeed * boostMultiplier : moveSpeed;

            // 4. 处理 LeanTouch 触摸移动 (Pan)
            // 获取所有手指
            var fingers = LeanTouch.GetFingers(true, false); // true: 忽略UI上的点击? 根据需求调整; false: 不模拟鼠标

            // 如果有手指触摸且没有在做旋转/缩放操作(通常旋转缩放需要2个手指，这里可以简单判断)
            // 或者你可以允许同时移动和旋转，这里演示叠加逻辑
            if (fingers.Count > 0)
            {
                // 获取手指在屏幕上的平均移动增量
                Vector2 screenDelta = LeanGesture.GetScreenDelta(fingers);

                // 将屏幕增量转换为世界空间的移动
                // 注意：通常拖拽是“拖地”，所以摄像机移动方向与手指方向相反
                if (screenDelta.sqrMagnitude > 0.0f)
                {
                    // 屏幕Y轴对应世界的前后(Forward)，X轴对应世界的左右(Right)
                    // 需要乘以灵敏度和时间
                    // 注意：LeanGesture.GetScreenDelta 返回的是像素值，不需要再乘 Time.deltaTime，因为它是每帧的变化
                    Vector3 touchMove = -(forward * screenDelta.y + right * screenDelta.x) * touchPanSensitivity * 0.1f;

                    // 将触摸移动叠加到目标位置
                    _targetPosition += touchMove;
                }
            }

            // 应用键盘/边缘移动
            if (direction.magnitude >= 0.1f)
            {
                _targetPosition += direction * speed * Time.deltaTime;
            }

            // 限制范围
            if (useBounds)
            {
                _targetPosition.x = Mathf.Clamp(_targetPosition.x, minBounds.x, maxBounds.x);
                _targetPosition.z = Mathf.Clamp(_targetPosition.z, minBounds.y, maxBounds.y);
            }

            // 应用平滑移动
            transform.position = Vector3.SmoothDamp(transform.position, _targetPosition, ref _currentVelocity, smoothTime);
        }

        private void HandleRotation()
        {
            float yawDelta = 0f;
            float pitchDelta = 0f;

            // 1. 鼠标右键旋转 (PC)
            if (Input.GetMouseButton(1))
            {
                yawDelta += Input.GetAxis("Mouse X") * rotationSpeed;
                pitchDelta -= Input.GetAxis("Mouse Y") * pitchSpeed;
            }

            // 2. LeanTouch 触摸旋转 (Mobile - 双指扭动)
            var fingers = LeanTouch.GetFingers(true, false);
            if (fingers.Count >= 2)
            {
                // 获取扭动角度
                float twistDegrees = LeanGesture.GetTwistDegrees(fingers);
                yawDelta += twistDegrees * touchRotSensitivity;

                // 也可以在这里加入双指上下滑动来控制 Pitch (如果需要)
                // float heightDelta = LeanGesture.GetScreenDelta(fingers).y;
                // pitchDelta -= heightDelta * touchRotSensitivity * 0.1f;
            }

            // 应用旋转
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
            // 1. 鼠标滚轮缩放
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            float zoomChange = 0f;

            if (Mathf.Abs(scroll) > 0.01f)
            {
                zoomChange = -scroll * zoomSpeed * 100f * Time.deltaTime;
            }

            // 2. LeanTouch 捏合缩放 (Mobile)
            var fingers = LeanTouch.GetFingers(true, false);
            if (fingers.Count >= 2)
            {
                // 获取捏合比例 (大于1放大，小于1缩小)
                float pinchScale = LeanGesture.GetPinchScale(fingers);

                if (Mathf.Abs(pinchScale - 1.0f) > 0.001f)
                {
                    // PinchScale 是比例，我们需要将其转换为 FOV 的增量
                    // 例如：比例 1.1 表示放大，FOV 应该变小? 或者根据习惯
                    // 通常捏合(pinch in, scale < 1)是缩小视野(拉远?)，扩张(pinch out, scale > 1)是放大视野(拉近?)
                    // 这里的逻辑：Pinch > 1 (手指张开) -> Zoom In (FOV减小)
                    // Pinch < 1 (手指捏合) -> Zoom Out (FOV增大)

                    float scaleFactor = 1.0f / pinchScale;
                    // 将比例应用到当前目标FOV
                    _targetFOV *= scaleFactor;

                    // 这里不需要 zoomChange 累加，直接修改了 _targetFOV
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