
using Obi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace EmbroideryFramewark
{
    [RequireComponent(typeof(EbdShortMemeryCtrl))]

    /// <summary>
    /// 工具类
    /// 任务是file与object之间的转换
    /// 还需要输出必要的RopeData
    /// </summary>
    public static class EbdGraphIO
    {
        private static object mylock = new();

        private static List<SingleRopeData> singleRopeDatas = new List<SingleRopeData>();

        private static List<GameObject> models = new List<GameObject>();

        private static EbdIOMode _default = new EbdDefaultIOModel();


        /// <summary>
        /// 从file -> datas和models
        /// </summary>
        /// <param name="path"></param>
        /// <param name="ropedatas"></param>
        /// <param name="models"></param>
        /// <param name="ioModel"></param>
        public static void GetRopeGraphFromFile(
            string path,
            Action<List<SingleRopeData>, List<GameObject>> callback,
            EbdIOMode ioModel = null
            )
        {
            lock (mylock)
            {
                //使用默认初始化
                if (object.ReferenceEquals(null, ioModel))
                    ioModel = _default;

                ioModel.AsyncFilePathToRopeDatas(path,callback);
            }
        }


        /// <summary>
        /// 从datas/model -> file
        /// </summary>
        /// <param name="path"></param>
        /// <param name="ioMode"></param>
        public static void SaveRopeGraphToFile(
            string path,
            List<SingleRopeData> ropedatas,
            EbdIOMode ioMode = null)
        {
            lock(mylock)
            {
                if (object.ReferenceEquals(null, ioMode))
                    ioMode = _default;

                ///进行save操作
                ioMode.ToFileInf(path,ropedatas);
            }
        }
    }


    public class EbdDefaultIOModel : EbdIOMode
    {
        private List<SingleRopeData> ropedatas = new();

        private List<GameObject> models = new();

        /// <summary>
        /// 未完成
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="fileinf"></param>
        public void GetInfFromFile(string filePath, out string fileinf)
        {
            if(File.Exists(filePath))
            {
                fileinf = File.ReadAllText(filePath);
                return;
            }
            fileinf = null;
            return;
        }




        /// <summary>
        /// 解析传递进来的fileInf
        /// 并根据这些返回对应modelList和RopeData List
        /// 
        /// line1: {（xyz）、（xyz）、（（rgba））}
        /// END
        /// </summary>
        /// <param name="fileInf">      文本信息</param>
        /// <param name="onFinish">     回调函数,当完成整个操作的时候调用</param>
        /// <returns></returns>
        public IEnumerator AsyncToRopeDatas(string fileInf, 
            Action<List<SingleRopeData>,List<GameObject>> onFinish)
        {
            if(object.ReferenceEquals(fileInf,null))
            {
                Debug.LogError($"fileInf: is null!");
                yield break;
            }

            ///初始化
            this.ropedatas.Clear();
            this.models.Clear();

            SingleRopeData ropeData = new SingleRopeData();

            string[] lineInf = fileInf.Split('\n');
            int lineNum = lineInf.Length;

            for(int i = 0; i < lineNum; ++i)
            {
                if (lineInf[i].StartsWith("END"))
                    break;

                string[] points = lineInf[i].Split('/');

                ropeData = new SingleRopeData();
                
                for(int j = 0; j < points.Length; ++j)
                {
                    string[] nums = points[j].Split(",");

                    Vector3 vector = new Vector3(
                        float.Parse(nums[0]), float.Parse(nums[1]), float.Parse(nums[2]));
                    
                    if (j == 0)
                        ropeData.begin = vector;
                    else 
                        ropeData.end = vector;
                }


                this.ropedatas.Add(ropeData);
            }


            
            ///ropeData已经完成了
            for (int i = 0; i < this.ropedatas.Count; ++i)
            {
                //RopeManager.Instance.CreateRope(ropedatas[i].begin, ropedatas[i].end, 0.01f);

                RopeManager.Instance.PreferRopeHelper.SetBeginPosition(ropedatas[i].begin);
                RopeManager.Instance.PreferRopeHelper.SetEndPosition(ropedatas[i].end);
                RopeManager.Instance.PreferRopeHelper.SetRopeLengthTo(0.01f);


                ///等两帧，等待绳子变化完全
                yield return null;
                yield return null;

                //将currentRope模型化
                this.models.Add(RopeManager.Instance.RopeChangeToModel(1));
            }

            //TODO:触发callback
            onFinish.Invoke(ropedatas, models);

            /////重新分配Rope的值
            RopeManager.Instance.ResetAllRope();
        }


        /// <summary>
        /// 解析传递进来的fileInf
        /// 并根据这些返回对应modelList和RopeData List
        /// 
        /// line1: {（xyz）、（xyz）、（（rgba））}
        /// END
        /// </summary>
        /// <param name="filePath">     文件地址</param>
        /// <param name="onFinish">     同上</param>
        public void AsyncFilePathToRopeDatas(string filePath,
            Action<List<SingleRopeData>, List<GameObject>> onFinish)
        {
            string inf = null;
            this.GetInfFromFile(filePath,out inf);
            MonoHelper.Instance.StartCoroutine(AsyncToRopeDatas(inf, onFinish));
        }




        private StringBuilder stringBuilder = new StringBuilder();

        /// <summary>
        /// 将RopeDatas转换成对应的文件信息
        /// </summary>
        /// <param name="ropedatas">    需要序列化的RopeDatas</param>
        /// <param name="fileInf">      转化成的文件信息</param>
        public void ToFileInf(string targetPath, List<SingleRopeData> ropedatas)
        {
            ///TODO:添加一个事件，来显示“正在导出……”字样。

            int ropeLength = ropedatas.Count;

            stringBuilder.Clear();

            for (int i = 0; i < ropeLength; ++i)
            {
                Vector3 begin = ropedatas[i].begin;
                Vector3 end = ropedatas[i].end;

                stringBuilder.AppendLine($"{begin.x},{begin.y},{begin.z}/{end.x},{end.y},{end.z}");
            }
            stringBuilder.AppendLine("END");

            CreateRopeFile(targetPath, stringBuilder.ToString());
        }



        public void CreateRopeFile(string fileName, string dirpath, string inf)
        {
            if (!Directory.Exists(dirpath))
            {
                Directory.CreateDirectory(dirpath);
            }

            File.WriteAllText(dirpath + "/" + fileName, inf);
        }
        private void CreateRopeFile(string filePath,string inf)
        {
            if (!File.Exists(filePath))
            {
                File.Create(filePath);
            }

            File.WriteAllText(filePath, inf);
        }

    }



    public interface EbdIOMode
    {
        #region file->data
        void GetInfFromFile(string filePath, out string fileinf);

        IEnumerator AsyncToRopeDatas(string fileInf, Action<List<SingleRopeData>, List<GameObject>> callback);

        public void AsyncFilePathToRopeDatas(string filePath, Action<List<SingleRopeData>, List<GameObject>> callback);

        #endregion


        #region data -> file

        void ToFileInf(string targetPath,List<SingleRopeData> ropedatas);

        #endregion

    }
}
