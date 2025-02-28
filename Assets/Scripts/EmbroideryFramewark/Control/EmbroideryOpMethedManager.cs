using EmbroideryFramewark.Interface;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.Burst.Intrinsics.Arm;

namespace EmbroideryFramewark
{
    /// <summary>
    /// 刺绣操作管理
    /// </summary>
    public class EmbroideryOpMethedManager : MonoSingleton<EmbroideryOpMethedManager>
    {
        //不允许在运行时修改
        //默认为鼠标操作
        [SerializeField]
        private EEmbroideryOperation _operationState = EEmbroideryOperation.MOUSE;


        [Header("刺绣动画相关：（后面使用AB包实现）")]
        [SerializeField]private List<AnimationClip> _embroideryAnimationList;
        
        /// <summary>
        /// _root被当作刺绣点来创建整个动画
        /// </summary>
        [SerializeField]public GameObject _root;

        /// <summary>
        /// pinOperator的跟随对象
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

        #region 添加与设置

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