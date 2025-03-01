using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TestRayCast : MonoBehaviour
{
    Ray ray1;
    Ray ray2;

    public Vector3 v1 = Vector3.left;
    public Vector3 v2 = Vector3.right;

    // Start is called before the first frame update
    void Start()
    {

    }

    RaycastHit[] hits = new RaycastHit[10];

    public int num1;
    public int num2;

    void Update()
    {
        ray1 = new Ray(v1, Vector3.right);
        ray2 = new Ray(v2, Vector3.left);

        num1 = Physics.RaycastNonAlloc(ray1, hits,10f);
        num2 = Physics.RaycastNonAlloc(ray2, hits,10f);

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(ray1);
        Gizmos.color = Color.red;
        Gizmos.DrawRay(ray2);
    }
}
