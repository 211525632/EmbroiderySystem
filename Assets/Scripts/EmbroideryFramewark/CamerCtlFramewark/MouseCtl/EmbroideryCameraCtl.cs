using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace EmbroideryFramewark
{
    public class EmbroideryCameraCtl : MonoSingleton<EmbroideryCameraCtl>
    {

        /// <summary>
        /// CameraCtlFramewark（mouseControl里面的一部分吧）
        /// 
        /// 刺绣基础操作：
        /// 1.左键（长按）：移动针的位置
        /// 2.右键（按下）：开始刺绣
        /// 
        /// 需要:
        /// 
        /// 1.镜头的中心旋转           （ctl + 左键） ——》根据针的位置
        /// 
        /// 2.根据Flag位置进行缩放     （鼠标中键滑动）-》或者根据鼠标位置进行缩放 或 根据屏幕中央进行缩放
        /// 3.镜头位移（镜头拖动）     （长按鼠标中键）
        /// 
        /// 4.快捷标准化重置位置         （ctl + wasd）-》进行镜头旋转（按flag？）
        /// 5.
        /// </summary>

        private Transform _mainCamerTrans;

        private Camera _mainCamera;


        protected override void Awake()
        {
            base.Awake();
            
            _mainCamerTrans = Camera.main.transform;
            _mainCamera = Camera.main;

            _mainCamera.nearClipPlane = 0.01f;
        }

        private void Start()
        {
            _viewModel = _mainCamera.transform.position.y;
            
        }

        private void LateUpdate()
        {
            if (HeartBit())
            {
                _mainCamerTrans = Camera.main.transform;
            }

            if (m_MouseInput.IsDragCamera)
                CameraDrag(m_MouseInput.MouseMove);

            if (m_MouseInput.IsCenterRotation)
                CenterRotation(m_MouseInput.MouseMove.normalized);

            CameraScale(m_MouseInput.CameraScale);
        }

        [Header("镜头拖拽相关：")]
        public float _dragSpeedModel = 3f;

        /// <summary>
        /// 当前的拖拽速度应该与镜头的位置有一定关系
        /// </summary>
        private float _currentDragSpeed { get => _dragSpeedModel * (_mainCamera.transform.position.y / _viewModel); }

        /// <summary>
        /// 相机视野拖拽
        /// </summary>
        /// <param name="moveDirection"> 相机运动的方向</param>
        public void CameraDrag(Vector2 moveDirection)
        {
            moveDirection =  - moveDirection.normalized;
            Vector3 temp= (_mainCamerTrans.right * moveDirection.x + _mainCamerTrans.up * moveDirection.y);
            _mainCamerTrans.position += temp * Time.deltaTime * _currentDragSpeed;
        }





        [Header("相机缩放相关：")]
        public float _scaleSpeed = 100f;

        private float _viewModel;

        /// <summary>
        /// 镜头缩放
        /// 需要调整镜头的切片距离
        /// </summary>
        /// <param name="scaleNum"> 缩放值，小于零为缩，大于零为放</param>
        public void CameraScale(float scaleNum)
        {
            if (scaleNum == 0)
                return;

            scaleNum = (scaleNum > 0 ? 1 : -1);

            this._mainCamerTrans.position += _mainCamerTrans.forward * scaleNum * _scaleSpeed * Time.deltaTime;
        }




        [Header("中心旋转相关：")]

        public float CenterRotationSpeed = 3f;

        public void CenterRotation(Vector2 mouseMove)
        {
            //Quaternion xAxis_Rotation = Quaternion.AngleAxis(mouseMove.x * CenterRotationSpeed * Time.deltaTime, Vector3.up);

            //Quaternion yAxis_Rotation = Quaternion.AngleAxis(-mouseMove.y * CenterRotationSpeed * Time.deltaTime, Vector3.right);

            _mainCamerTrans.Rotate(Vector3.up, mouseMove.x * CenterRotationSpeed * Time.deltaTime,Space.World);
            _mainCamerTrans.Rotate(Vector3.right, -mouseMove.y * CenterRotationSpeed * Time.deltaTime,Space.Self);
        }



        /// <summary>
        /// 心跳测试
        /// </summary>
        /// <returns></returns>
        private bool HeartBit()
        {
            return object.ReferenceEquals(_mainCamerTrans, Camera.main);
        }


        private void OnEnable()
        {
            m_MouseInput.EmbroideryAction.Enable();
        }
        private void OnDisable()
        {
            m_MouseInput.EmbroideryAction.Disable();
        }
    }

}