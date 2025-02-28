using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using XLua;

namespace UiFramewark
{

    /// <summary>
    /// 好像转为完全使用Lua进行编写了
    /// 放弃了使用Lua继承C#类的这种方式
    /// 
    /// 原因是，lua生成的table不能动态转化成C#类的userdata！
    /// </summary>
    [CSharpCallLua]
    [LuaCallCSharp]
    public class BasicLuaUiAction : BasicUiAction
    {

        public BasicLuaUiAction():base() 
        {
            
        }

        public void TestLua()
        {
            Debug.Log("TestLua");
        }

        public BasicLuaUiAction New()
        {
            Debug.Log("LuaBasicAction：：New");

            return new BasicLuaUiAction();
        }

        public override void OnCreate()
        {
            Debug.Log("LuaBasicAction：：Create");
        }

        public override void OnDestroy()
        {
            Debug.Log("LuaBasicAction：：Destroy");
        }

        public override void OnDisable()
        {
            Debug.Log("LuaBasicAction：：ReturnObj");
        }

        public override void OnEnable()
        {
            Debug.Log("LuaBasicAction：：BorrowObj");
        }
    }
}
