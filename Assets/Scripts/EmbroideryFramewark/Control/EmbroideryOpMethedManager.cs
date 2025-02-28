using EmbroideryFramewark.Interface;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.Burst.Intrinsics.Arm;

namespace EmbroideryFramewark
{
    /// <summary>
    /// �����������
    /// </summary>
    public class EmbroideryOpMethedManager : MonoSingleton<EmbroideryOpMethedManager>
    {
        //������������ʱ�޸�
        //Ĭ��Ϊ������
        [SerializeField]
        private EEmbroideryOperation _operationState = EEmbroideryOperation.MOUSE;


        [Header("���嶯����أ�������ʹ��AB��ʵ�֣�")]
        [SerializeField]private List<AnimationClip> _embroideryAnimationList;
        
        /// <summary>
        /// _root�������������������������
        /// </summary>
        [SerializeField]public GameObject _root;

        /// <summary>
        /// pinOperator�ĸ������
        /// </summary>
        [SerializeField]public GameObject _follow;

        private Animation anim = null;

        public Animation pinMoveAnim { get => anim; }

        public EEmbroideryOperation CurrentOpState
        {
            get => _operationState;
        }

        private readonly Dictionary<EEmbroideryOperation,IEmbroideryOp> _opDict = new ();


        protected override void Awake()
        {
            base.Awake();
        }

        private void Start()
        {
            if(this.CurrentOpState == EEmbroideryOperation.MOUSE)
            {
                this.AddOperation(EEmbroideryOperation.MOUSE, new MousePinControl());
                SetMouseMove();
            }
        }


        private void Update()
        {
            if (object.ReferenceEquals(null, _opDict))
                return;

            _opDict[CurrentOpState].Run();
        }

        public Dictionary<string, float> animTimeDict = new();

        private void SetMouseMove()
        {
            _root = Instantiate<GameObject>(_root);
            _root.name = "mouse_animationMove_root";
            _root.transform.position = Vector3.zero;

            _follow = Instantiate<GameObject>(_follow,_root.transform);
            _follow.name = "follow";
            _follow.transform.position = Vector3.zero;

            anim = _root.AddComponent<Animation>();

            for(int i = 0; i < _embroideryAnimationList.Count; i++)
            {
                _embroideryAnimationList[i].legacy = true;
                anim?.AddClip(_embroideryAnimationList[i], _embroideryAnimationList[i].name);
                animTimeDict.Add(_embroideryAnimationList[i].name, _embroideryAnimationList[i].length);
            }
        }

        #region ���������

        public void AddOperation(EEmbroideryOperation addEnum,IEmbroideryOp newOp)
        {
            _opDict.Add(addEnum, newOp);
        }

        public void RemoveOperation(EEmbroideryOperation addEnum)
        {
            _opDict.Remove(addEnum);
        }

        #endregion
    }

}