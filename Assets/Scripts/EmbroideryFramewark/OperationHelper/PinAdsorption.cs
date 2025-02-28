using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace EmbroideryFramewark
{
    /// <summary>
    /// ����Զ�����
    /// ��ֻ���ڵ�λλ���Ͻ��д��壩
    /// 
    /// 1.��û�нӴ���PinBeginPoint֮ǰ����ֱ������Flag������ת
    /// 2.�ڽӴ���Flag֮�󣬾Ͱ���Flag�����������ת��
    /// 3.ֱ��End�Ӵ�֮��Ӵ����������
    /// 4.����Ҫ����Rope begin��Rope End��
    /// 
    /// 
    /// Ӧ�ü��Ͽ�������ʹ�ÿ�����������pin��
    /// ʹ�ÿ�������rotation����Ϊ��ʼrotation
    /// 
    /// </summary>
    public class PinAdsorption : MonoBehaviour
    {

        [SerializeField] private bool _enableAbility = false;

        ///------------------------------

        private Transform _flagTransform;

        private SinglePinHelper _targetPinHelper;

        /// <summary>
        /// ���뵽flag�ķ���
        /// 
        /// ���λ��Ҫ����һЩ
        /// 
        /// </summary>
        private Vector3 towards
        {
            get => _flagTransform.position - 
                (_targetPinHelper.transform.position - _targetPinHelper.transform.forward * 0.3f);
        }

        /// <summary>
        /// �벼�ϵľ���
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


        #region ��������Flag�Ĺ���ʵ��

        /// <summary>
        /// �����¼�����
        /// </summary>
        private void AdsordePinWithDirection()
        {
            AdsordePinWithDirection(towards);
        }

        /// <summary>
        /// ת��ָ��"����"
        /// </summary>
        /// <param name="towards">  ��ת��ĳ��� </param>
        /// <param name="axis">     ת������ȣ��ƻ�������벼�ϵľ����йأ�   </param>
        public void AdsordePinWithDirection(Vector3 towards)
        {
            _targetPinHelper.transform.rotation = Quaternion.LookRotation(towards, Vector3.up);
        }

        public void ObjLookAtTarget(Transform objTransform,Vector3 targetPosition)
        {
            objTransform.rotation = Quaternion.LookRotation(targetPosition - objTransform.position, Vector3.up);
        }


        /// <summary>
        /// TODO����Ҫ����beginEnter��beginExit
        /// 
        /// ����ĳ���������ת
        /// 
        /// ����center��operation�������position��������ת����
        /// 
        /// ֻ��beginPoint��enter֮�����ʹ��
        /// ��ĳ���᲻��Ӱ�죩
        /// </summary>

        private void AdsordeToPoint()
        {
            //ObjLookAtTarget(PinManager.Instance.CurrentPinHelper.transform,
            //    this._flagTransform.position - Vector3.up* this._flagTransform.position.y);

            PinManager.Instance.CurrentPinHelper.transform.rotation = Quaternion.LookRotation(towards, Vector3.up);
        }


        #endregion

        #region ���ù��ܿ���

        public void EnableAbility()
        { 
            this._enableAbility = true;
        }

        public void DisableAbility()
        {
            this._enableAbility = false;
        }

        #endregion

        #region �¼�����

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