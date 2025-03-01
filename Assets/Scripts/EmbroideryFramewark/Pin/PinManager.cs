using EmbroideryFramewark;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// ��pin�Ĺ���
/// ֻ����һ��pin
/// </summary>

public class PinManager : MonoSingleton<PinManager>
{
    [Header("pin�ı�׼ģ�ͣ�ʹ��AB����ʵ��")]
    [SerializeField] private GameObject _pinModel;

    /// <summary>
    /// ��control�л�ȡ
    /// ��������ھ��Լ�����һ��
    /// </summary>
    [SerializeField] private GameObject _pinOperator;


    /// <summary>
    /// ʵ������pin
    /// </summary>
    public SinglePinHelper CurrentPinHelper { get; private set; }

    public PinOperator CurrentPinOperator { get; private set; }

    public PinState CurrentPinState => CurrentPinHelper.CurrentPinState;



    private float _pinSide = 1;
    /// <summary>
    /// 1Ϊ����
    /// -1Ϊ����
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


    #region ��ʼ��pin��Transform��Ϣ

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


    #region �����¼�

    private void SetEvent()
    {
        EventCenter<Action>.Instance.AddEvent(EventConst.OnPinEndEnter, () => this._pinSide = -_pinSide);
    }

    #endregion

}
