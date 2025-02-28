using XLua;
using UnityEngine;
using System.IO;

namespace MyLua
{
    public class LuaManager : MonoSingleton<LuaManager>
    {
        protected override void Awake()
        {
            base.Awake();


            LuaEnv.AddLoader(MyCustomLoader);
            LuaEnv.DoString("print('Testing Lua Environment')");
            //_luaEnv.AddLoader(MyCustomABLoader);
        }


        public static LuaEnv LuaEnv = new();

        private byte[] MyCustomLoader(ref string filePath)
        {
            string path = Application.dataPath + "/Lua/" + filePath + ".lua";
            if (File.Exists(path))
            {
                return File.ReadAllBytes(path);
            }
            return null;
        }

        private byte[] MyCustomABLoader(ref string filePath)
        {
            string abPath = Application.streamingAssetsPath + "/LuaAB";
            AssetBundle ab = AssetBundle.LoadFromFile(abPath);
            if (ab != null)
            {
                TextAsset luaScript = ab.LoadAsset<TextAsset>(filePath + ".lua");
                if (luaScript != null)
                {
                    return luaScript.bytes;
                }
            }
            return null;
        }
    }
}