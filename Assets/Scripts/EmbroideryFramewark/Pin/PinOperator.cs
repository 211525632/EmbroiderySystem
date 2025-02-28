using System;
using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;

namespace EmbroideryFramewark
{
    /// <summary>
    /// ���߿����ߡ�
    /// ͨ�������������һ��pin��
    /// ���ܹ�ֱ��ͨ���޸�Transform������һ��pin
    /// </summary>
    public class PinOperator : MonoBehaviour
    {
        public Transform _transform { get; private set; }

        public Vector3 Direction { get => _transform.forward; }

        void Start()
        {
            _transform = transform;
            _pinTransform = PinManager.Instance.CurrentPinHelper.transform;

            Virtualization();
            
            SetEvent();
        }

        // Update is called once per frame
        void Update()
        {
            this.PinSynchronousPosition();
        }

        #region ��ʾ���

        [Header("���������ӻ���")]
        [SerializeField] GameObject model;
        /// <summary>
        /// �Բ��������п��ӻ�
        /// </summary>
        public void Virtualization()
        {
            if (object.ReferenceEquals(null, model))
            {
                return;
            }

            GameObject temp= Instantiate(model,this.transform);
            //��֤���泯ǰ
            temp.transform.rotation = Quaternion.identity;
        }

        #endregion


        #region ͬ�����

        private Transform _pinTransform;
        private void PinSynchronousRotation()
        {
            if (object.ReferenceEquals(_pinTransform, null))
            {
                Debug.LogError("û���ҵ�pinTransform!");
                return;
            }
            _pinTransform.rotation = _transform.rotation;
        }

        private void PinSynchronousPosition()
        {
            if (object.ReferenceEquals(_pinTransform, null))
            {
                Debug.LogError("û���ҵ�pinTransform!");
                return;
            }
            _pinTransform.position = _transform.position;
        }

        #endregion


        #region �����¼�

        private void SetEvent()
        {
            ///��û��check��ʱ���ʹ��ͬ��
            EventCenter<Action>.Instance.AddEvent(VirsualizationEventConst.OnBuLiaoDisCheck, PinSynchronousRotation);
        }

        #endregion
    }
}
