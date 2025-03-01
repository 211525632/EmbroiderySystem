using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLua;


public static class XluaExtension
{
    /// <summary>
    /// 链式获取 LuaTable 中的嵌套字段
    /// </summary>
    /// <param name="rootTable">根表（如 _luaEnv.Global）</param>
    /// <param name="path">路径（如 "LuaTest.t.f"）</param>
    /// <returns>目标对象（LuaTable/LuaFunction 或其他类型）</returns>
    public static object GetChainValue(this LuaTable rootTable, string path)
    {
        if (rootTable == null || string.IsNullOrEmpty(path))
            return null;

        string[] keys = path.Split('.');
        object current = rootTable;

        foreach (string key in keys)
        {
            if (current is LuaTable table)
            {
                current = table.Get<object>(key);
            }
            else
            {
                // 路径中断，返回 null 或抛出异常
                return null;
            }
        }

        return current;
    }
}
