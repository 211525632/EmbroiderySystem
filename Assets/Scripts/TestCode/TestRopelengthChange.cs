using Obi;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRopelengthChange : MonoBehaviour
{
    // Start is called before the first frame update

    private ObiRope _rope;

    private ObiRopeCursor _cursor;

    public float ropeLength = 1f;

    public bool isRopeChange = false;

    void Start()
    {
        _cursor = GetComponent<ObiRopeCursor>();
        _rope = _cursor.GetComponent<ObiRope>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isRopeChange)
        {
            _cursor.ChangeLength(ropeLength);
        }
    }


    public Vector3 _begin;
    public Vector3 _end;

    private void OnEnable()
    {
        _begin = _rope.GetParticlePosition(1);
        _end = _rope.GetParticlePosition(_rope.activeParticleCount);
    }

    private void OnDisable()
    {
        
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(_begin, 0.1f);

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(_end, 0.1f);

    }
}
