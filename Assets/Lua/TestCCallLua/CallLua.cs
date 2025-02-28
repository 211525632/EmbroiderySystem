
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

            // ��ȡȫ�ֱ� LuaTest
            LuaTable luaTest = luaEnv.Global.Get<LuaTable>("LuaTest");

            // ��ȡ�ӱ� t
            LuaTable tTable = luaTest.Get<LuaTable>("t");

            // ��ȡ���� f��Lua ��ð���﷨����ʽ���� self���˴����õ��﷨��
            LuaFunction fFunc = tTable.Get<LuaFunction>("f");

            // ���ú��� f���������
            if (fFunc != null)
            {
                // ��һ������Ϊ self���� tTable�����ڶ�������Ϊ num
                fFunc.Call(tTable, 123);
            }
            else
            {
                Debug.LogError("���� f δ�ҵ���");
            }


            Debug.Log(tTable.Get<string>("name"));

            LuaFunction fun = luaEnv.Global.GetInPath<LuaFunction>(call);
            fun.Call(2);
            //fun.Call("1");
        }
    }
}