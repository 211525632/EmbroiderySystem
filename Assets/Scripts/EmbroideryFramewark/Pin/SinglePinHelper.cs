using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EmbroideryFramewark
{
    /// <summary>
    /// �뵱ʱ������״��
    /// </summary>
    public enum PinState
    {
        BeginExit, BeginEnter, EndEnter
    }

    /// <summary>
    /// ������ȡ��pin��ص��ص���Ϣ
    /// </summary>
    public class SinglePinHelper : MonoBehaviour
    {
        /// <summary>
        /// ͨ���������Physics��check��ʵ�ּ��
        /// </summary>
        public Transform _beginTransform { get; private set; }
        public Transform _endTransform { get; private set; }

        /// <summary>
        /// ��ǰ���״̬
        /// </summary>
        [SerializeField]
        public PinState CurrentPinState { get; private set; } = PinState.BeginExit;



        public bool _isShrink = false;


        void Awake()
        {
            _beginTransform = GameObject.Find(this.name + "/begin")?.transform;
            _endTransform = GameObject.Find(this.name + "/end")?.transform;

            if (object.ReferenceEquals(_beginTransform, null))
            {
                Debug.LogError("�Ҳ���Pin::BeginTransform,name:" + this.name + "/begin");
            }
            if (object.ReferenceEquals(_endTransform, null))
            {
                Debug.LogError("�Ҳ���Pin::EndTransform,name:" + this.name + "/end");
            }
        }

        void Start()
        {
            _beginColliders = new Collider[MaxColliderNum];

            _endColliders = new Collider[MaxColliderNum];

            SetCheckNum();
            SetEvent();
        }

        /// <summary>
        /// ������ǰ��״̬
        /// </summary>
        private void Update()
        {
            if (CurrentPinState == PinState.BeginEnter
                && CheckEnd() != 0)
            {
                this.CurrentPinState = PinState.EndEnter;
                return;
            }

            ///��enter��ʱ��
            if (CheckBegin() != 0)
                this.CurrentPinState = PinState.BeginEnter;
            else
                this.CurrentPinState = PinState.BeginExit;
        }

        #region �˵���

        [Header("�˵�����أ�")]

        [SerializeField] private float CheckRadius = 0.01f;

        public List<string> checkLayerName = new();

        [SerializeField] private int MaxColliderNum = 10;

        private int _checkNum = 0;

        private Collider[] _beginColliders = null;

        private Collider[] _endColliders = null;

        
        
        /// <summary>
        /// ͨ������checkLayerName������checkNumber��
        /// </summary>
        private void SetCheckNum()
        {
            for (int i = 0; i < checkLayerName.Count; i++)
            {
                _checkNum |= LayerMask.GetMask(checkLayerName[i]);
            }
        }

        /// <summary>
        /// ʹ�ý�����ײ�������м��BeginEnter���
        /// </summary>
        /// <returns></returns>
        public int CheckBegin()
        {
            int checkNum = Physics.OverlapCapsuleNonAlloc(
                _beginTransform.position, _endTransform.position,
                CheckRadius, _beginColliders, _checkNum);

            return checkNum;
        }

        public int CheckEnd()
        {
            int checkNum = Physics.OverlapCapsuleNonAlloc(
                _endTransform.position, _endTransform.position - _endTransform.forward * 0.2f,
                CheckRadius, _beginColliders, _checkNum);

            return checkNum;

        }

        #endregion

        #region �����¼�

        private void SetEvent()
        {
            ///OnShrinkComplete
            EventCenter<Action>.Instance.AddEvent(EventConst.OnRopeShrinkComplete, () => this._isShrink = false);
        }

        #endregion

        #region Gizmos
        public float redius = 0.01f;
        public void OnDrawGizmos()
        {
            Gizmos.color = Color.red;

            Gizmos.DrawSphere(this._beginTransform.position, CheckRadius);

            Gizmos.color = Color.green;

            Gizmos.DrawSphere(this._endTransform.position, CheckRadius);
        }

        #endregion
    }
}