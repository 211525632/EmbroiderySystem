using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TestCode
{
    public class TestFileOp:MonoBehaviour
    {
        string str = "这是一个file information";

        StringBuilder stringBuilder = new StringBuilder();


        private void Awake()
        {
            Debug.Log(Application.dataPath);
            Debug.Log(Application.persistentDataPath);
            Debug.Log(Application.streamingAssetsPath);
            Debug.Log(Application.temporaryCachePath);

            string dirPath = Application.dataPath + "/---------TextTestFile";
            string filepath = "/TestText.text";

            if(!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }


            float cnt = 0;
            float delta = 0.01f;
            for(int i = 0; i < 100; ++i)
            {
                stringBuilder.AppendLine($"{cnt},0,0/{cnt},0,1");
                stringBuilder.AppendLine($"{cnt},0,1/{cnt+delta},0,0");
                cnt += delta;
            }

            stringBuilder.AppendLine("END");
            
            File.WriteAllText(dirPath+filepath, stringBuilder.ToString());
        }




    }
}
