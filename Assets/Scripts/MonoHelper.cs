using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������MonoBehaviour�ű�ʹ��Э��
/// </summary>
public class MonoHelper : MonoSingleton<MonoHelper>
{
    protected override void Awake()
    {
        base.Awake(); 
    }
}
