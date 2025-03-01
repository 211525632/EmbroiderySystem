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
        /// CameraCtlFramewark��mouseControl�����һ���ְɣ�
        /// 
        /// �������������
        /// 1.��������������ƶ����λ��
        /// 2.�Ҽ������£�����ʼ����
        /// 
        /// ��Ҫ:
        /// 
        /// 1.��ͷ��������ת           ��ctl + ����� �������������λ��
        /// 
        /// 2.����Flagλ�ý�������     ������м�������-�����߸������λ�ý������� �� ������Ļ�����������
        /// 3.��ͷλ�ƣ���ͷ�϶���     ����������м���
        /// 
        /// 4.��ݱ�׼������λ��         ��ctl + wasd��-�����о�ͷ��ת����flag����
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

        [Header("��ͷ��ק��أ�")]
        public float _dragSpeedModel = 3f;

        /// <summary>
        /// ��ǰ����ק�ٶ�Ӧ���뾵ͷ��λ����һ����ϵ
        /// </summary>
        private float _currentDragSpeed { get => _dragSpeedModel * (_mainCamera.transform.position.y / _viewModel); }

        /// <summary>
        /// �����Ұ��ק
        /// </summary>
        /// <param name="moveDirection"> ����˶��ķ���</param>
        public void CameraDrag(Vector2 moveDirection)
        {
            moveDirection =  - moveDirection.normalized;
            Vector3 temp= (_mainCamerTrans.right * moveDirection.x + _mainCamerTrans.up * moveDirection.y);
            _mainCamerTrans.position += temp * Time.deltaTime * _currentDragSpeed;
        }





        [Header("���������أ�")]
        public float _scaleSpeed = 100f;

        private float _viewModel;

        /// <summary>
        /// ��ͷ����
        /// ��Ҫ������ͷ����Ƭ����
        /// </summary>
        /// <param name="scaleNum"> ����ֵ��С����Ϊ����������Ϊ��</param>
        public void CameraScale(float scaleNum)
        {
            if (scaleNum == 0)
                return;

            scaleNum = (scaleNum > 0 ? 1 : -1);

            this._mainCamerTrans.position += _mainCamerTrans.forward * scaleNum * _scaleSpeed * Time.deltaTime;
        }




        [Header("������ת��أ�")]

        public float CenterRotationSpeed = 3f;

        public void CenterRotation(Vector2 mouseMove)
        {
            //Quaternion xAxis_Rotation = Quaternion.AngleAxis(mouseMove.x * CenterRotationSpeed * Time.deltaTime, Vector3.up);

            //Quaternion yAxis_Rotation = Quaternion.AngleAxis(-mouseMove.y * CenterRotationSpeed * Time.deltaTime, Vector3.right);

            _mainCamerTrans.Rotate(Vector3.up, mouseMove.x * CenterRotationSpeed * Time.deltaTime,Space.World);
            _mainCamerTrans.Rotate(Vector3.right, -mouseMove.y * CenterRotationSpeed * Time.deltaTime,Space.Self);
        }



        /// <summary>
        /// ��������
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