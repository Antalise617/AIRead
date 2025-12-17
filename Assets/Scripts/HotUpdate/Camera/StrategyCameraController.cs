using UnityEngine;
using GameFramework.CameraSystem;

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

        [Header("Pitch Settings (新增：上下视野)")]
        public float pitchSpeed = 2f;
        public float minPitch = 0f; // 负值表示抬头（摄像机降低）
        public float maxPitch = 30f;  // 正值表示低头（摄像机升高/俯视）

        [Header("Zoom Settings")]
        public float zoomSpeed = 10f;
        public float minFOV = 20f;
        public float maxFOV = 80f;

        [Header("Boundaries")]
        public bool useBounds = true;
        public Vector2 minBounds = new Vector2(-50, -50);
        public Vector2 maxBounds = new Vector2(50, 50);

        private Vector3 _targetPosition;
        private Vector3 _currentVelocity;
        private float _targetFOV = 60f;

        // 新增：内部记录当前的旋转角度
        private float _currentYaw;   // Y轴旋转 (左右)
        private float _currentPitch; // X轴旋转 (上下)

        private void Start()
        {
            _targetPosition = transform.position;
            if (UnityEngine.Camera.main != null)
                _targetFOV = UnityEngine.Camera.main.fieldOfView;

            // 初始化旋转角度为当前的物体角度
            _currentYaw = transform.eulerAngles.y;
            _currentPitch = transform.eulerAngles.x;
        }

        private void Update()
        {
            HandleMovement();
            HandleRotation();
            HandleZoom();
        }

        private void HandleMovement()
        {
            // ... 保持之前的移动代码不变 ...
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");

            var camTransform = UnityEngine.Camera.main.transform;
            var forward = camTransform.forward;
            var right = camTransform.right;
            forward.y = 0;
            right.y = 0;
            forward.Normalize();
            right.Normalize();

            Vector3 direction = (forward * v + right * h).normalized;
            float speed = Input.GetKey(KeyCode.LeftShift) ? moveSpeed * boostMultiplier : moveSpeed;

            if (direction.magnitude >= 0.1f)
            {
                _targetPosition += direction * speed * Time.deltaTime;
            }

            if (useBounds)
            {
                _targetPosition.x = Mathf.Clamp(_targetPosition.x, minBounds.x, maxBounds.x);
                _targetPosition.z = Mathf.Clamp(_targetPosition.z, minBounds.y, maxBounds.y);
            }

            // 这里建议把 SmoothDamp 改为直接赋值或极小的 smoothTime，以配合 RTS 手感
            transform.position = Vector3.SmoothDamp(transform.position, _targetPosition, ref _currentVelocity, smoothTime);
        }

        // ★★★ 核心修改部分 ★★★
        private void HandleRotation()
        {
            // 检测鼠标右键按住
            if (Input.GetMouseButton(1))
            {
                float mouseX = Input.GetAxis("Mouse X");
                float mouseY = Input.GetAxis("Mouse Y");

                // 1. 处理 Yaw (左右旋转)
                _currentYaw += mouseX * rotationSpeed;

                // 2. 处理 Pitch (上下旋转)
                // 注意：通常鼠标往上推是看上面，但在这种控制模式下：
                // 增加 X 角度 = 物体低头 = 摄像机升高 = 更俯视
                // 减少 X 角度 = 物体抬头 = 摄像机降低 = 更平视
                // 根据习惯，鼠标往下拉(负值)通常期望是拉起视角(变平视)，或者反之。
                // 你可以根据手感调整这里的 +/- 号
                _currentPitch -= mouseY * pitchSpeed;

                // 3. 限制上下角度范围
                _currentPitch = Mathf.Clamp(_currentPitch, minPitch, maxPitch);

                // 4. 统一应用旋转
                // 使用 Quaternion.Euler 一次性设置，避免万向节锁
                transform.rotation = Quaternion.Euler(_currentPitch, _currentYaw, 0f);
            }
        }

        private void HandleZoom()
        {
            // ... 保持之前的缩放代码不变 ...
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(scroll) > 0.01f)
            {
                _targetFOV -= scroll * zoomSpeed * 100f * Time.deltaTime;
                _targetFOV = Mathf.Clamp(_targetFOV, minFOV, maxFOV);
                CameraController.Instance.ZoomCamera(_targetFOV, 0.1f);
            }
        }
    }
}