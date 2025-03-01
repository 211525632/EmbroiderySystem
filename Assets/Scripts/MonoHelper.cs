using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 帮助非MonoBehaviour脚本使用协程
/// </summary>
public class MonoHelper : MonoSingleton<MonoHelper>
{
    protected override void Awake()
    {
        base.Awake(); 
    }
}
