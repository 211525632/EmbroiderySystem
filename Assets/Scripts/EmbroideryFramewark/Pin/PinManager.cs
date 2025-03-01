using EmbroideryFramewark;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 对pin的管理
/// 只存在一个pin
/// </summary>

public class PinManager : MonoSingleton<PinManager>
{
    [Header("pin的标准模型，使用AB包来实现")]
    [SerializeField] private GameObject _pinModel;

    /// <summary>
    /// 在control中获取
    /// 如果不存在就自己创建一个
    /// </summary>
    [SerializeField] private GameObject _pinOperator;


    /// <summary>
    /// 实例化的pin
    /// </summary>
    public SinglePinHelper CurrentPinHelper { get; private set; }

    public PinOperator CurrentPinOperator { get; private set; }

    public PinState CurrentPinState => CurrentPinHelper.CurrentPinState;



    private float _pinSide = 1;
    /// <summary>
    /// 1为正面
    /// -1为反面
    /// </summary>
    public float PinSide { get => _pinSide; }


    protected override void Awake()
    {
        base.Awake();

        if (object.ReferenceEquals(CurrentPinHelper, null))
        {
            CurrentPinHelper = Instantiate(_pinModel).GetComponent<SinglePinHelper>();
            Debug.LogError("SingleHelper Cant Be Find");
        }


        if (object.ReferenceEquals(CurrentPinOperator, null))
        {
            Debug.LogError("PinOperator Cant Be Find");
            CurrentPinOperator = Instantiate(_pinOperator,this.transform).GetComponent<PinOperator>();
            CurrentPinOperator.transform.position = Vector3.up;
        }

    }
    void Start()
    {
        SetEvent();
    }


    #region 初始化pin的Transform信息

    public void SetPinTransform(Vector3 poisition, Quaternion rotation)
    {
        CurrentPinHelper.transform.position = poisition;
        CurrentPinHelper.transform.rotation = rotation;
    }

    public void SetPinTransform(Vector3 poisition)
    {
        SetPinTransform(poisition, Quaternion.identity);
    }

    public void SetPinTransform()
    {
        SetPinTransform(Vector3.zero, Quaternion.identity);
    }

    #endregion


    #region 设置事件

    private void SetEvent()
    {
        EventCenter<Action>.Instance.AddEvent(EventConst.OnPinEndEnter, () => this._pinSide = -_pinSide);
    }

    #endregion

}
