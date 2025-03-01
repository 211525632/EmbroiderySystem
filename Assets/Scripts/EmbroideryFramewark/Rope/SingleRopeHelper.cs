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
    /// ���ڵ���rope�ĸ�������
    /// �����ǻ�ȡ����һ��rope��������б��������ӵ���������
    /// </summary>
    public class SingleRopeHelper : MonoBehaviour
    {
        [Header("�������˵�����")]
        public ObiParticleAttachment RopeBeginAttachment;

        public ObiParticleAttachment RopeEndAttachment;


        //--------------------private------------------

        private ObiRopeCursor _cursor;

        private ObiRope _rope;

        #region �������ں���

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

        #region ���ȿ���

        public float RopeLength => this._rope.restLength;

        /// <summary>
        /// ֱ�ӽ����ӵĳ������ó�ָ������
        /// </summary>
        /// <param name="length">   ��Ҫ��ֱ�����õĳ���</param>
        public void SetRopeLengthTo(float length)
        {
            _cursor.ChangeLength(length);
        }

        /// <summary>
        ///  ͨ���������Ӹı�����ʵ�����ӵ�΢С�ı�
        ///  ���õĳ��Ȳ��������Сֵ
        ///  
        /// ���������������֮��end�����������ӵ��������֤��cursors����Ϊ����������ɾ���ˡ�
        /// Ҫ�����Ҫ����source��λ�ã�ʹ����ӿ���begin�ˡ�
        /// </summary>
        /// <param name="lengthDelta"> ���Ӹı��� </param>
        public void SetRopeLength(float lengthDelta)
        {
            float length = _rope.restLength + lengthDelta;

            SetRopeLengthTo(length);
        }

        #endregion

        #region ����free or lock

        /// <summary>
        /// ����end�˵ķ��ɻ��߱��̶�
        /// </summary>
        /// <param name="state">trueΪloack��falseΪfree</param>
        public void SetEndLoackState(bool state)
        {
            this.RopeEndAttachment.enabled = state;
        }

        #endregion

        #region rope���λ���ƶ�

        
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
        /// ����begin�ĸ����ƶ�����
        /// </summary>
        /// <param name="targetTransform"></param>
        public void SetBeginBoundingObject(Transform targetTransform)
        {
            _beginFollowing = targetTransform;
        }


        #endregion

        #region ���ӵ���ʾ

        /// <summary>
        /// �������ӵ�����
        /// </summary>
        /// <param name="state">true���أ�false�������</param>
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

        #region �õ����ӵ�Mesh

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
        /// �������ӵ�ģ��
        /// ��������κ����
        /// </summary>
        public GameObject GetRopeModelObj()
        {
            _tempModel = Instantiate<GameObject>(RopeManager.Instance._empty, 
                RopeManager.Instance._objModelRoot.transform);

            _tempModel.name = this.name;


            //---��������----
            MeshFilter filter= _tempModel.AddComponent<MeshFilter>();
            _tempModel.AddComponent<MeshRenderer>();

            filter.sharedMesh = Mesh.Instantiate<Mesh>(_rope.GetComponent<MeshFilter>().sharedMesh);


            //---���Ʋ���---
            Renderer renderer = _tempModel.GetOrAddComponent<Renderer>();
            renderer.enabled = true;
            renderer.sharedMaterials = _meshFilter.GetComponent<Renderer>().sharedMaterials;

            //---����λ��---
            _tempModel.transform.position = transform.position;

            return _tempModel;
        }

        #endregion
    }
}