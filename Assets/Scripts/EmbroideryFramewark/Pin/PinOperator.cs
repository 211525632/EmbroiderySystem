using System;
using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;

namespace EmbroideryFramewark
{
    /// <summary>
    /// 针线控制者。
    /// 通过这个类来操作一个pin。
    /// 不能够直接通过修改Transform来操作一个pin
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

        #region 显示相关

        [Header("操作器可视化：")]
        [SerializeField] GameObject model;
        /// <summary>
        /// 对操纵器进行可视化
        /// </summary>
        public void Virtualization()
        {
            if (object.ReferenceEquals(null, model))
            {
                return;
            }

            GameObject temp= Instantiate(model,this.transform);
            //保证正面朝前
            temp.transform.rotation = Quaternion.identity;
        }

        #endregion


        #region 同步相关

        private Transform _pinTransform;
        private void PinSynchronousRotation()
        {
            if (object.ReferenceEquals(_pinTransform, null))
            {
                Debug.LogError("没有找到pinTransform!");
                return;
            }
            _pinTransform.rotation = _transform.rotation;
        }

        private void PinSynchronousPosition()
        {
            if (object.ReferenceEquals(_pinTransform, null))
            {
                Debug.LogError("没有找到pinTransform!");
                return;
            }
            _pinTransform.position = _transform.position;
        }

        #endregion


        #region 设置事件

        private void SetEvent()
        {
            ///在没有check的时候就使用同步
            EventCenter<Action>.Instance.AddEvent(VirsualizationEventConst.OnBuLiaoDisCheck, PinSynchronousRotation);
        }

        #endregion
    }
}
