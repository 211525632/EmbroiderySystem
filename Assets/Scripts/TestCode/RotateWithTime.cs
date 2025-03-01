using Unity.VisualScripting;
using UnityEngine;

public class RotateWithTime : MonoBehaviour
{
    public Transform Target;

    public Quaternion PreRotation;

    [Range(0f,1f)]public float axis=0f;

    private void Start()
    {
        PreRotation = this.transform.rotation;
    }

    /// <summary>
    /// 中心转向
    /// </summary>
    Quaternion lookAtRotation
    {
        get =>Quaternion.LookRotation(
            Target.transform.position - this.transform.position
            , Vector3.up);
    }

    void Update()
    {
        transform.rotation = Quaternion.Lerp(
            PreRotation, this.lookAtRotation, axis);

        RotateWithCenter(Quaternion.AngleAxis(axis * 360f, Vector3.up));
    }
    #region 按照中心旋转

    [SerializeField] private Vector3 _center = Vector3.zero;
    [SerializeField] private float ridus = 0.5f;

    [SerializeField] private Transform _obj;

    [SerializeField] private Vector3 DistanceVector3;

    /// <summary>
    /// 中心旋转
    /// </summary>
    /// <param name="rotation"></param>
    private void RotateWithCenter(Quaternion rotation)
    {
        DistanceVector3 = _obj.position - _center;

        _obj.transform.position -= DistanceVector3;
        DistanceVector3 = rotation * DistanceVector3;
        _obj.rotation *= rotation;

        _obj.transform.position += DistanceVector3;
    }

    #endregion


    #region Gizmos

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_center, ridus);
    }

    #endregion
}
