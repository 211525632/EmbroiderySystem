using EmbroideryFramewark;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Pipes;
using UnityEngine;

namespace EmbroideryFramewark
{
    public class VirsualizationEventConst
    {
        /// <summary>
        /// �ڽ��в���check��ʱ��
        /// </summary>
        public static string OnBuLiaoCheck = "OnBuLiaoCheck";


        public static string OnBuLiaoDisCheck = "OnBuLiaoDisCheck";
    }

    /// <summary>
    /// ��ſ��ӻ�
    /// ���߼��ʵ��
    /// 
    /// ����ǻ��ڲ��ϲ������Ҵ���ˮƽ״̬��ʱ��
    /// </summary>
    public class PinPointVisualization : MonoSingleton<PinPointVisualization>
    {
        /// <summary>
        /// ��Ϊ���ӻ���ǵ�ģ��
        /// ����ʹ��AB��ʵ��
        /// </summary>
        [Header("��Ϊ���ӻ���ǵ�ģ��,����ʹ��AB��ʵ��")]
        [SerializeField] GameObject flagModel;

        GameObject flag;

        public Transform FlagTransform { get; private set; }

        /// <summary>
        /// ����ֹflag�����Ƕ���������
        /// </summary>
        [Header("���ɱ��ʱ������벼�ϵĴ�ֱ����")]
        [Range(0f, 0.5f)]
        public float VirticalDistance = 0.01f;


        [Header("��ʼ���ľ������ƣ�")]
        public float ActiveDistance = 0.5f;

        void Start()
        {
            if (object.ReferenceEquals(flagModel, null))
            {
                Debug.LogError("û�����ó�ʼflag");
                return;
            }
            flag = GameObject.Instantiate(flagModel, this.transform);

            flag.gameObject.SetActive(false);


            this._halfUnitLength = this.UnitLength / 2;

            FlagTransform = flag.transform;

            SetEvent();
        }

        /// <summary>
        /// ÿһ֡��⵱ǰ���λ������벼�ϵľ���
        /// ͨ��������ж��Ƿ�Ҫ��ʾFlag
        /// 
        /// ����PinOperation�ĳ�����ȷ��flag ��λ��
        /// </summary>

        public void FlagVisualization()
        {
            if (!_isSetBuLiao)
            {
                Debug.LogError("û�����ò���");
                return;
            }

            if (Mathf.Abs(PinManager.Instance.CurrentPinHelper._beginTransform.position.y - _buLiaoPosition.y)
                < this.ActiveDistance)
            {
                this.CheckBuLiao();
            }
            else
            {
                this.DisableFlag();
                EventCenter<Action>.Instance.GetAction(VirsualizationEventConst.OnBuLiaoDisCheck)?.Invoke();
            }
        }


        #region ��ʾ������

        private Vector3 _temp = Vector3.zero;

        /// <summary>
        /// ����һ��flag
        /// </summary>
        /// <param name="poisition">�벼�ϵĽӴ���</param>
        public void MoveAndEnableFlag(Vector3 poisition)
        {
            _temp = poisition + Vector3.up * VirticalDistance;
            flag.transform.position = _temp;
            flag.gameObject.SetActive(true);
        }

        /// <summary>
        /// ֻ��һ��flag
        /// ֱ�����ص�����
        /// </summary>
        public void DisableFlag()
        {
            flag.gameObject.SetActive(false);
        }

        #endregion

        #region ���ò���λ��

        private bool _isSetBuLiao = false;

        private Vector3 _buLiaoPosition = Vector3.zero;

        public void SetBuLiao(Vector3 buLiaoPosition)
        {
            _buLiaoPosition = this.transform.position;
            _isSetBuLiao = true;
        }


        #endregion

        #region ���߼��
        private RaycastHit[] _raycastHits = new RaycastHit[5];

        /// <summary>
        /// ʹ��PinOperator��Direction����
        /// </summary>
        private void CheckBuLiao()
        {
            int colliderNum = Physics.RaycastNonAlloc(
                PinManager.Instance.CurrentPinOperator.transform.position,
                PinManager.Instance.CurrentPinOperator._transform.forward,
                _raycastHits, ActiveDistance, LayerMask.GetMask("BuLiao"));

            if ( colliderNum != 0)
            {
                InternalCulculateAdsorbedPosition(_raycastHits[0].point);
                EventCenter<Action>.Instance.GetAction(VirsualizationEventConst.OnBuLiaoCheck)?.Invoke();
                this.MoveAndEnableFlag(this._adsorbedPosition);
            }
            else
            {
                this.DisableFlag();
                EventCenter<Action>.Instance.GetAction(VirsualizationEventConst.OnBuLiaoDisCheck)?.Invoke();
            }
        }

        #endregion

        #region  ��������ļ���


        [Header("����������λ���ȣ�")]
        [SerializeField]
        [Range(0.001f, 1f)] private float UnitLength = 0.01f;

        //��start���Զ�����
        private float _halfUnitLength;

        //��������������ֵ
        private Vector3 _adsorbedPosition = Vector3.zero;

        /// <summary>
        /// ���������������ֵ
        /// </summary>
        /// <param name="ordinaryPosition"> ��ʼ���� </param>
        private void InternalCulculateAdsorbedPosition(Vector3 ordinaryPosition)
        {
            _adsorbedPosition.x = CalculateAdsorbedNumber(ordinaryPosition.x);
            _adsorbedPosition.z = CalculateAdsorbedNumber(ordinaryPosition.z);
        }

        public Vector3 CulculateAdsorbedPosition(Vector3 ordinaryPosition)
        {
            return new Vector3(
                CalculateAdsorbedNumber(ordinaryPosition.x),
                ordinaryPosition.y,
                CalculateAdsorbedNumber(ordinaryPosition.z));
        }

        /// <summary>
        /// ����һ���������ֵ
        /// </summary>
        /// <param name="axis">x��z���ϵ�����ֵ</param>
        /// <returns></returns>
        public float CalculateAdsorbedNumber(float axis)
        {
            float mode = axis % UnitLength;

            return (Mathf.Abs(mode) > this._halfUnitLength ? axis - mode + UnitLength * (mode < 0f ? -1f : 1f) : axis - mode);
        }

        #endregion

        #region �¼�����

        private void SetEvent()
        {
            //BeginExit
            EventCenter<Action>.Instance.AddEvent(EventConst.OnPinBeginExit, FlagVisualization);
            //EventCenter<Action>.Instance.AddEvent(EventConst.OnPinBeginExit, () => this.flag.GetComponent<Material>())


            //EndEnter
            EventCenter<Action>.Instance.AddEvent(EventConst.OnPinEndEnter, DisableFlag);
        }

        #endregion

        #region Gizmos
        [Header("Gizmos��")]
        [SerializeField]private float _radius= 0.1f;

        public void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(_raycastHits[0].point, _radius);
        }


        #endregion
    }

}