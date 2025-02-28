
using MyLua;
using System;
using UnityEngine;
using XLua;


namespace mLua
{
    public class CallLua:MonoBehaviour
    {
        LuaEnv luaEnv;
        public string nameCall;

        public string call;

        private void Awake()
        {
            
        }
        private void Start()
        {
            luaEnv= LuaManager.LuaEnv;
            luaEnv.DoString("require 'TestCCallLua/TestCSharpCallLua'");

            // 获取全局表 LuaTest
            LuaTable luaTest = luaEnv.Global.Get<LuaTable>("LuaTest");

            // 获取子表 t
            LuaTable tTable = luaTest.Get<LuaTable>("t");

            // 获取函数 f（Lua 的冒号语法会隐式传递 self，此处需用点语法）
            LuaFunction fFunc = tTable.Get<LuaFunction>("f");

            // 调用函数 f，传入参数
            if (fFunc != null)
            {
                // 第一个参数为 self（即 tTable），第二个参数为 num
                fFunc.Call(tTable, 123);
            }
            else
            {
                Debug.LogError("函数 f 未找到！");
            }


            Debug.Log(tTable.Get<string>("name"));

            LuaFunction fun = luaEnv.Global.GetInPath<LuaFunction>(call);
            fun.Call(2);
            //fun.Call("1");
        }
    }
}