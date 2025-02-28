using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pinwheel.MeshToFile;
using System;

namespace EmbroideryFramewark
{
    public struct SingleRopeData
    {
        public Vector3 begin;
        public Vector3 end;

        /// <summary>
        /// ����й���ȵ������ٽ�����Ӽ���
        /// </summary>
        public MaterialData materalData;


        public SingleRopeData(Vector3 begin,Vector3 end,Material material)
        {
            this.begin = begin;
            this.end = end;

            this.materalData = new MaterialData(material);
        }

        public SingleRopeData(SingleRopeHelper ropeHelper)
        {
            this = new SingleRopeData(ropeHelper._beginTransform.position, ropeHelper._endTransform.position,
                ropeHelper.GetMeshRender().GetComponent<Renderer>().sharedMaterial);
        }
    }

    public struct MaterialData
    {
        //��ɫ
        public Vector4 color;

        ///����

        public MaterialData(Material material)
        {
            this.color = material.color;
        }
    }


    public class EmbroideryOpSaver
    {

        /// <summary>
        /// ���캯��
        /// </summary>
        public EmbroideryOpSaver()
        {
            RopeDatas = new();
            RopeModels = new();

            _opDateCachine = new();
            _opModelCachine = new();
        }

        /// <summary>
        /// ���ÿһ�β�����������Ϣ
        /// </summary>
        public readonly Stack<SingleRopeData> RopeDatas;


        /// <summary>
        /// ������б�ʵ������ģ��ʵ��
        /// ͨ�����ٻ���������������Ԫ����ʵ�ֳ�������
        /// </summary>
        public readonly Stack<GameObject> RopeModels;

        /// <summary>
        /// ����һ�δ������
        /// </summary>
        /// <param name="currentRopeHelper">    Ҫ�����Rope��RopeHelper</param>
        public void SaveOp(SingleRopeHelper currentRopeHelper,GameObject ropeModel)
        {
            this.ClearCachine();

            RopeDatas.Push(new SingleRopeData(currentRopeHelper));
            RopeModels.Push(ropeModel);
        }


        /// <summary>
        /// �����ϴβ���
        /// </summary>
        public void RecollectOp()
        {
            //����Ƿ���ڿ��Ի���Ĳ���
            if (_opDateCachine.Count <= 0f)
                return;

            ///�ͷŻ��棬�Ż�֮ǰ������
            RopeDatas.Push(_opDateCachine.Pop());
            RopeModels.Push(_opModelCachine.Pop());

            ///��ʾ��������ģ��
            RopeModels.Peek().SetActive(true);

            Vector3 reversePosition = RopeDatas.Peek().begin;
            reversePosition.y = -reversePosition.y;

            ///��ʣ�µ����ӵ�β�� �� �����ص�β��������
            RopeManager.Instance.CurrentRopeHelper.SetEndPosition(reversePosition);

        }


        /// <summary>
        /// --------------------------��������ʱ�Ļ���-----------------------------------
        /// </summary>
        private Stack<SingleRopeData> _opDateCachine;

        private Stack<GameObject> _opModelCachine;

        /// <summary>
        /// ������һ�εĲ���
        /// </summary>
        public void RevokeOp()
        {
            //����Ƿ���ڿ��Գ����Ĳ���
            if (RopeModels.Count <= 0f)
                return;

            ///���棬ֱ����һ���²���
            _opDateCachine.Push(RopeDatas.Pop());
            _opModelCachine.Push(RopeModels.Pop());

            ///���ر�������ģ�ͣ�����ʹ�ó�������
            _opModelCachine.Peek().SetActive(false);

            RopeManager.Instance.HideOrActivePreRope(false);
            RopeManager.Instance.HideOrActiveAfterRope(false);

            ///��ʣ�µ����ӵ��ײ� �� ���ڵ�β��������
            RopeManager.Instance.CurrentRopeHelper.SetEndPosition(_opDateCachine.Peek().end);

        }


        private void ClearCachine()
        {
            _opDateCachine.Clear();
            
            int cachineLength = _opModelCachine.Count;

            for(int i=0;i< cachineLength; ++i)
            {
                MonoHelper.Destroy(_opModelCachine.Pop());
            }
        }



        #region �浵���

        private SingleRopeData _singleRopeData;

        /// <summary>
        /// TODO:��־û� + �ָ�Stack������
        /// 
        /// ��ʵ�ִ浵�Ĺ��ܣ�
        /// 
        /// ���ݵ�ǰջ��Ԫ������ʼ���µ�Rope
        /// ���ڻָ�֮ǰ��û������Ĵ���
        /// </summary>
        public void ReCreateRope()
        {
            if (RopeDatas.TryPop(out _singleRopeData))
            {
                RopeManager.Instance.CreateRope(_singleRopeData.begin, _singleRopeData.end, 0.1f);
            }
        }

        #endregion

    }

}