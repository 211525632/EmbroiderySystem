using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace EmbroideryFramewark
{
    /// <summary>
    /// 针脚自动吸附
    /// （只能在单位位置上进行刺绣）
    /// 
    /// 1.在没有接触到PinBeginPoint之前，就直接向着Flag进行旋转
    /// 2.在接触到Flag之后，就按照Flag点进行中心旋转。
    /// 3.直到End接触之后接触吸附，最后
    /// 4.（需要控制Rope begin和Rope End）
    /// 
    /// 
    /// 应该加上控制器，使用控制器来操作pin。
    /// 使用控制器的rotation来作为起始rotation
    /// 
    /// </summary>
    public class PinAdsorption : MonoBehaviour
    {

        [SerializeField] private bool _enableAbility = false;

        ///------------------------------

        private Transform _flagTransform;

        private SinglePinHelper _targetPinHelper;

        /// <summary>
        /// 从针到flag的方向
        /// 
        /// 针的位置要靠后一些
        /// 
        /// </summary>
        private Vector3 towards
        {
            get => _flagTransform.position - 
                (_targetPinHelper.transform.position - _targetPinHelper.transform.forward * 0.3f);
        }

        /// <summary>
        /// 与布料的距离
        /// </summary>
        private float distance
        {
            get => Mathf.Abs(_flagTransform.position.y - _targetPinHelper.transform.position.y);
        }


        void Start()
        {
            _flagTransform = PinPointVisualization.Instance.FlagTransform;
            _targetPinHelper = PinManager.Instance.CurrentPinHelper;

            SetEvent();
        }


        #region 针吸附到Flag的功能实现

        /// <summary>
        /// 用于事件设置
        /// </summary>
        private void AdsordePinWithDirection()
        {
            AdsordePinWithDirection(towards);
        }

        /// <summary>
        /// 转向到指定"方向"
        /// </summary>
        /// <param name="towards">  旋转后的朝向 </param>
        /// <param name="axis">     转向的力度（计划与针尖与布料的距离有关）   </param>
        public void AdsordePinWithDirection(Vector3 towards)
        {
            _targetPinHelper.transform.rotation = Quaternion.LookRotation(towards, Vector3.up);
        }

        public void ObjLookAtTarget(Transform objTransform,Vector3 targetPosition)
        {
            objTransform.rotation = Quaternion.LookRotation(targetPosition - objTransform.position, Vector3.up);
        }


        /// <summary>
        /// TODO：需要区分beginEnter和beginExit
        /// 
        /// 跟随某个点进行旋转
        /// 
        /// 根据center与operation的两点的position来计算旋转坐标
        /// 
        /// 只在beginPoint被enter之后才能使用
        /// （某个轴不被影响）
        /// </summary>

        private void AdsordeToPoint()
        {
            //ObjLookAtTarget(PinManager.Instance.CurrentPinHelper.transform,
            //    this._flagTransform.position - Vector3.up* this._flagTransform.position.y);

            PinManager.Instance.CurrentPinHelper.transform.rotation = Quaternion.LookRotation(towards, Vector3.up);
        }


        #endregion

        #region 设置功能开闭

        public void EnableAbility()
        { 
            this._enableAbility = true;
        }

        public void DisableAbility()
        {
            this._enableAbility = false;
        }

        #endregion

        #region 事件设置

        private void SetEvent()
        {
            ///BeginExit
            EventCenter<Action>.Instance.AddEvent(EventConst.OnPinBeginExit, AdsordePinWithDirection);

            //BeginEnter
            //EventCenter<Action>.Instance.AddEvent(EventConst.OnPinBeginEnter, AdsordePinWithDirection);
            EventCenter<Action>.Instance.AddEvent(EventConst.OnPinBeginEnter, AdsordeToPoint);
        }

        #endregion


        #region Gizmos

        [SerializeField] float radurs = 0.1f;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(_targetPinHelper.transform.position - _targetPinHelper.transform.forward * 0.3f, radurs);
        }

        #endregion
    }

}