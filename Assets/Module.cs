using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WFC;

public class Module : MonoBehaviour
{
    public int moduleHash;
    public bool highlightForward;
    public bool highlightBack;
    public bool highlightUp;
    public bool highlightDown;
    public bool highlightLeft;
    public bool highlightRight;

    public int rightHash;
    public int leftHash;
    public int forwardHash;
    public int backHash;
    public int upHash;
    public int downHash;
    


    private void OnValidate()
    {
        var verts = GetComponent<MeshFilter>().sharedMesh.vertices;
        moduleHash = WFCUtils.GenerateVertsHash(verts.ToList(), absolute:false);
        
        rightHash = WFCUtils.BoundaryHashForDirection(verts.ToList(), WFCUtils.Direction.Right, false);
        leftHash = WFCUtils.BoundaryHashForDirection(verts.ToList(), WFCUtils.Direction.Left, false);
        upHash = WFCUtils.BoundaryHashForDirection(verts.ToList(), WFCUtils.Direction.Up, false);
        downHash = WFCUtils.BoundaryHashForDirection(verts.ToList(), WFCUtils.Direction.Down, false);
        forwardHash = WFCUtils.BoundaryHashForDirection(verts.ToList(), WFCUtils.Direction.Forward, false);
        backHash = WFCUtils.BoundaryHashForDirection(verts.ToList(), WFCUtils.Direction.Back, false);
    }

    private void OnDrawGizmosSelected()
    {
        var verts = GetComponent<MeshFilter>().sharedMesh.vertices;

        foreach (var vert in verts)
        {
            if (Mathf.Abs(vert.z - 1) < 0.001f)
            {
                if(highlightForward) Gizmos.DrawSphere(transform.TransformPoint(vert), 0.1f);
            }
            
            if (Mathf.Abs(vert.z - -1) < 0.001f)
            {
                if(highlightBack) Gizmos.DrawSphere(transform.TransformPoint(vert), 0.1f);
            }
            
            if (Mathf.Abs(vert.x - -1) < 0.001f)
            {
                if(highlightLeft) Gizmos.DrawSphere(transform.TransformPoint(vert), 0.1f);
            }
            
            if (Mathf.Abs(vert.x - 1) < 0.001f)
            {
                if(highlightRight) Gizmos.DrawSphere(transform.TransformPoint(vert), 0.1f);
            }
            
            if (Mathf.Abs(vert.y - 1) < 0.001f)
            {
                if(highlightUp) Gizmos.DrawSphere(transform.TransformPoint(vert), 0.1f);
            }
            
            if (Mathf.Abs(vert.y - -1) < 0.001f)
            {
                if(highlightDown) Gizmos.DrawSphere(transform.TransformPoint(vert), 0.1f);
            }
        }
    }
}
