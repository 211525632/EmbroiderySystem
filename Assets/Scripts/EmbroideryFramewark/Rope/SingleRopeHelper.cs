using Obi;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace EmbroideryFramewark
{


    /// <summary>
    /// 放在单个rope的父物体上
    /// 作用是获取控制一个rope所需的所有变量和绳子的所有属性
    /// </summary>
    public class SingleRopeHelper : MonoBehaviour
    {
        [Header("绳子两端的锁定")]
        public ObiParticleAttachment RopeBeginAttachment;

        public ObiParticleAttachment RopeEndAttachment;


        //--------------------private------------------

        private ObiRopeCursor _cursor;

        private ObiRope _rope;

        #region 生命周期函数

        private void Awake()
        {
            _cursor = GetComponentInChildren<ObiRopeCursor>();
            _rope = _cursor.GetComponent<ObiRope>();

            _beginTransform = GameObject.Find(this.name + "/begin").transform;

            _endTransform = GameObject.Find(this.name + "/end").transform;
        }


        private void Update()
        {
            if (object.ReferenceEquals(_beginFollowing, null))
                return;

            _beginTransform.position = _beginFollowing.transform.position;
        }

        #endregion

        #region 长度控制

        public float RopeLength => this._rope.restLength;

        /// <summary>
        /// 直接讲绳子的长度重置成指定长度
        /// </summary>
        /// <param name="length">   需要被直接重置的长度</param>
        public void SetRopeLengthTo(float length)
        {
            _cursor.ChangeLength(length);
        }

        /// <summary>
        ///  通过传入绳子改变量来实现绳子的微小改变
        ///  设置的长度不会低于最小值
        ///  
        /// 如果出现收缩绳子之后end不能束缚绳子的情况。则证明cursors将作为束缚的粒子删除了。
        /// 要解决需要调低source的位置，使其更加靠近begin端。
        /// </summary>
        /// <param name="lengthDelta"> 绳子改变量 </param>
        public void SetRopeLength(float lengthDelta)
        {
            float length = _rope.restLength + lengthDelta;

            SetRopeLengthTo(length);
        }

        #endregion

        #region 绳子free or lock

        /// <summary>
        /// 绳子end端的放松或者被固定
        /// </summary>
        /// <param name="state">true为loack，false为free</param>
        public void SetEndLoackState(bool state)
        {
            this.RopeEndAttachment.enabled = state;
        }

        #endregion

        #region rope点的位置移动

        
        Transform _beginFollowing = null;

        public Transform _beginTransform { get; private set; }

        public Transform _endTransform { get; private set; }

        public float RopePointDistance => Vector3.Distance(this._endTransform.position, this._beginTransform.position);

        public void SetEndPosition(Vector3 position)
        {
            _endTransform.position = position;
        }

        public void SetBeginPosition(Vector3 position)
        {
            _beginTransform.position = position;
        }

        /// <summary>
        /// 设置begin的跟随移动物体
        /// </summary>
        /// <param name="targetTransform"></param>
        public void SetBeginBoundingObject(Transform targetTransform)
        {
            _beginFollowing = targetTransform;
        }


        #endregion

        #region 绳子的显示

        /// <summary>
        /// 设置绳子的隐藏
        /// </summary>
        /// <param name="state">true隐藏；false解除隐藏</param>
        public void SetRopeDisActive()
        {
            this.gameObject.SetActive(false);
        }

        public void SetRopeActive()
        {
            //this.GetMeshRender().enabled = true;
            this.gameObject.SetActive(true);
        }

        #endregion

        #region 得到绳子的Mesh

        private GameObject _tempModel;

        [SerializeField] private MeshFilter _meshFilter;

        public Mesh GetMesh()
        {
            return _rope.gameObject.GetComponent<Mesh>();
        }

        public MeshRenderer GetMeshRender()
        {
            return _rope.gameObject.GetComponent<MeshRenderer>();
        }

        /// <summary>
        /// 创建绳子的模型
        /// 不会包含任何组件
        /// </summary>
        public GameObject GetRopeModelObj()
        {
            _tempModel = Instantiate<GameObject>(RopeManager.Instance._empty, 
                RopeManager.Instance._objModelRoot.transform);

            _tempModel.name = this.name;


            //---复制网格----
            MeshFilter filter= _tempModel.AddComponent<MeshFilter>();
            _tempModel.AddComponent<MeshRenderer>();

            filter.sharedMesh = Mesh.Instantiate<Mesh>(_rope.GetComponent<MeshFilter>().sharedMesh);


            //---复制材质---
            Renderer renderer = _tempModel.GetOrAddComponent<Renderer>();
            renderer.enabled = true;
            renderer.sharedMaterials = _meshFilter.GetComponent<Renderer>().sharedMaterials;

            //---复制位置---
            _tempModel.transform.position = transform.position;

            return _tempModel;
        }

        #endregion
    }
}