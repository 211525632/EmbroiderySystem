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
        /// 在进行布料check的时候
        /// </summary>
        public static string OnBuLiaoCheck = "OnBuLiaoCheck";


        public static string OnBuLiaoDisCheck = "OnBuLiaoDisCheck";
    }

    /// <summary>
    /// 针脚可视化
    /// 射线检测实现
    /// 
    /// 检测是基于布料不动，且处于水平状态的时候
    /// </summary>
    public class PinPointVisualization : MonoSingleton<PinPointVisualization>
    {
        /// <summary>
        /// 作为可视化标记的模型
        /// 后续使用AB包实现
        /// </summary>
        [Header("作为可视化标记的模型,后续使用AB包实现")]
        [SerializeField] GameObject flagModel;

        GameObject flag;

        public Transform FlagTransform { get; private set; }

        /// <summary>
        /// （防止flag被覆盖而看不见）
        /// </summary>
        [Header("生成标记时，标记与布料的垂直距离")]
        [Range(0f, 0.5f)]
        public float VirticalDistance = 0.01f;


        [Header("开始检测的距离限制：")]
        public float ActiveDistance = 0.5f;

        void Start()
        {
            if (object.ReferenceEquals(flagModel, null))
            {
                Debug.LogError("没有设置初始flag");
                return;
            }
            flag = GameObject.Instantiate(flagModel, this.transform);

            flag.gameObject.SetActive(false);


            this._halfUnitLength = this.UnitLength / 2;

            FlagTransform = flag.transform;

            SetEvent();
        }

        /// <summary>
        /// 每一帧检测当前针的位置与距离布料的距离
        /// 通过这个来判断是否要显示Flag
        /// 
        /// 按照PinOperation的朝向来确定flag 的位置
        /// </summary>

        public void FlagVisualization()
        {
            if (!_isSetBuLiao)
            {
                Debug.LogError("没有设置布料");
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


        #region 显示与销毁

        private Vector3 _temp = Vector3.zero;

        /// <summary>
        /// 创造一个flag
        /// </summary>
        /// <param name="poisition">与布料的接触点</param>
        public void MoveAndEnableFlag(Vector3 poisition)
        {
            _temp = poisition + Vector3.up * VirticalDistance;
            flag.transform.position = _temp;
            flag.gameObject.SetActive(true);
        }

        /// <summary>
        /// 只有一个flag
        /// 直接隐藏掉即可
        /// </summary>
        public void DisableFlag()
        {
            flag.gameObject.SetActive(false);
        }

        #endregion

        #region 设置布料位置

        private bool _isSetBuLiao = false;

        private Vector3 _buLiaoPosition = Vector3.zero;

        public void SetBuLiao(Vector3 buLiaoPosition)
        {
            _buLiaoPosition = this.transform.position;
            _isSetBuLiao = true;
        }


        #endregion

        #region 射线检测
        private RaycastHit[] _raycastHits = new RaycastHit[5];

        /// <summary>
        /// 使用PinOperator的Direction坐标
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

        #region  吸附坐标的计算


        [Header("坐标吸附单位长度：")]
        [SerializeField]
        [Range(0.001f, 1f)] private float UnitLength = 0.01f;

        //在start中自动计算
        private float _halfUnitLength;

        //存放吸附后的坐标值
        private Vector3 _adsorbedPosition = Vector3.zero;

        /// <summary>
        /// 计算吸附后的坐标值
        /// </summary>
        /// <param name="ordinaryPosition"> 初始坐标 </param>
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
        /// 计算一个轴的吸附值
        /// </summary>
        /// <param name="axis">x、z轴上的坐标值</param>
        /// <returns></returns>
        public float CalculateAdsorbedNumber(float axis)
        {
            float mode = axis % UnitLength;

            return (Mathf.Abs(mode) > this._halfUnitLength ? axis - mode + UnitLength * (mode < 0f ? -1f : 1f) : axis - mode);
        }

        #endregion

        #region 事件设置

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
        [Header("Gizmos：")]
        [SerializeField]private float _radius= 0.1f;

        public void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(_raycastHits[0].point, _radius);
        }


        #endregion
    }

}