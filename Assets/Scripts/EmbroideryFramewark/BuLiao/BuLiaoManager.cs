using EmbroideryFramewark;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;


/// <summary>
/// 布料控制器的职责：
/// //对布料的操作
/// 1.使用其生成、修改、销毁一个布料 （但是与此同时，将会把刺绣的图形一起删除）
/// 2.可以自定义生成布料的 模型、材质、transform等属性
/// 3.支持对布料的一些扩展与操作
/// 4.支持自身添加部分拓展功能
/// 
/// ///设计结构体-》获取布料的相关信息
/// 5.同时，可以获取当前BuLiao的基本信息
/// 6.
/// 
/// 将布料想象为平面，而不是曲面
/// </summary>
public class BuLiaoManager : MonoSingleton<BuLiaoManager>, IGetBuLiaoDate,ISetOrGetBuLiaoTrans, ISetOrGetBuLiaoType,IGetAndSetBuLiaoWidth
{

    [Header("AB:")]
    [SerializeField] private GameObject BuLiaoModel;

    [Header("设置BuLiao的layer")]
    public string LayerName = "BuLiao";


    private object myLock = new object();

    protected override void Awake()
    {
        base.Awake();
        InitBuLiao();
    }


    #region 全局唯一的布料

    private BuLiao _buLiao;

    private void InitBuLiao()
    {
        if(object.ReferenceEquals(null,BuLiaoModel))
        {
            Debug.Log("BuLiaoManager::布料model为null，初始化或者切换失败");
            return;
        }


        ///生成新的BuLiao
        GameObject  obj     = Instantiate<GameObject>(BuLiaoModel);

        BuLiao      buLiao  = obj.GetComponent<BuLiao>();

        if (!object.ReferenceEquals(buLiao, null))
        {
            this._buLiao    = buLiao;
        }
        else
        {
            this._buLiao    = obj.AddComponent<DefaultBuLiao>();
            Debug.LogWarning("指定的模型没有挂载BuLiao类，及其子类，现已经自动挂载……");
        }

        obj.transform.position = Vector3.up * 0.3f;
        obj.transform.rotation = Quaternion.identity;

    }

    #endregion

    #region Get、Set相关


    ///会出现多个BuLiao的情况
    ///那么什么时候会？
    ///对于针线他们的操作都是全局唯一的

    public BuLiaoData GetBuLiaoData()
    {
        lock (myLock)
        {
            if (object.ReferenceEquals(null, _buLiao))
            {
                return BuLiaoData.None;
            }

            return _buLiao.GetBuLiaoData();
        }
    }

    public void SetBuLiaoTrans(Transform targetTrans)
    {
        ///TODO:同时计算法线normal
        _buLiao.gameObject.transform.position = targetTrans.position;
        _buLiao.gameObject.transform.rotation = targetTrans.rotation;
        _buLiao.gameObject.transform.localScale = targetTrans.localScale;
    }

    public void SetBuLiaoTrans(Vector3 position, Quaternion rotation, Vector3 scale)
    {
        ///TODO:同时计算法线normal
        _buLiao.gameObject.transform.position = position;
        _buLiao.gameObject.transform.rotation = rotation;
        _buLiao.gameObject.transform.localScale = scale;
    }

    public void SetBuLiaoType(GameObject prefabs)
    {
        if (!object.ReferenceEquals(null, prefabs))
            this.BuLiaoModel = prefabs;
    }

    public GameObject GetBuLiaoType()
    {
        return _buLiao.gameObject;
    }


    #endregion

    #region 布料宽度，与绳子在两侧的位置设置有关


    private float _buLiaoWidth = 0.004f;

    public float GetBuLiaoWidth()
    {
        return _buLiaoWidth;
    }

    public void SetBuLiaoWidth(float buLiaoWidth)
    {
        this._buLiaoWidth = buLiaoWidth;
    }

    #endregion
}
