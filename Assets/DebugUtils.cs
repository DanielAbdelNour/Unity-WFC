using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEditor;
using System.Linq;

public class DebugUtils : MonoBehaviour
{
    List<Vector3> redGizmoPoints = new List<Vector3>();
    List<Vector3> greenGizmoPoints = new List<Vector3>();
    List<Vector3> blueGizmoPoints = new List<Vector3>();
    List<Vector3> gizmoShapes = new List<Vector3>();

    [Button("Mirror X")]
    void MirrorX(GameObject go)
    {
        MeshFilter tileBMeshFilter = go.GetComponent<MeshFilter>();
        Mesh tileBMesh = tileBMeshFilter.sharedMesh;
        List<Vector3> tileBVerts = new List<Vector3>(StaticUtils.TransformMesh(tileBMesh, Vector3.zero, Quaternion.identity, new Vector3Int(-1, 1, 1)));
        tileBMesh.vertices = tileBVerts.ToArray();
    }

    [Button("Mirror Y")]
    void MirrorZ(GameObject go)
    {
        MeshFilter tileBMeshFilter = go.GetComponent<MeshFilter>();
        Mesh tileBMesh = tileBMeshFilter.sharedMesh;
        List<Vector3> tileBVerts = new List<Vector3>(StaticUtils.TransformMesh(tileBMesh, Vector3.zero, Quaternion.identity, new Vector3Int(1, -1, 1)));
        tileBMesh.vertices = tileBVerts.ToArray();
    }

    [Button("Flip Triangles")]
    void FilpTriangles(GameObject go)
    {
        MeshFilter tileBMeshFilter = go.GetComponent<MeshFilter>();
        Mesh tileBMesh = tileBMeshFilter.sharedMesh;
        List<int> newTriangles = new List<int>(tileBMesh.triangles);
        newTriangles.Reverse();
        tileBMesh.SetTriangles(newTriangles, 0);
    }

    [Button("Clear Gizmos")]
    void ClearGizmos()
    {
        redGizmoPoints.Clear();
        greenGizmoPoints.Clear();
        blueGizmoPoints.Clear();
        gizmoShapes.Clear();
    }

    enum FaceDirection
    {
        leftFace,
        rightFace,
        topFace,
        bottomFace,
        frontFace,
        backFace
    }

    List<Vector3> GetBoundaryPointsForDirection(GameObject go, FaceDirection direction)
    {
        List<Vector3> vertices = new List<Vector3>(go.GetComponent<MeshFilter>().sharedMesh.vertices);
        List<Vector3> facePoints = new List<Vector3>();

        switch (direction)
        {
            case FaceDirection.leftFace:
                facePoints.AddRange(new List<Vector3>() {
                    new Vector3(-1, -1, -1),
                    new Vector3(-1, -1, 1),
                    new Vector3(-1, 1, 1),
                    new Vector3(-1, 1, -1)
                });
                break;
            case FaceDirection.rightFace:
                facePoints.AddRange(new List<Vector3>() {
                    new Vector3(1, -1, -1),
                    new Vector3(1, -1, 1),
                    new Vector3(1, 1, 1),
                    new Vector3(1, 1, -1)
                });
                break;
            case FaceDirection.topFace:
                facePoints.AddRange(new List<Vector3>() {
                    new Vector3(-1, 1, -1),
                    new Vector3(-1, 1, 1),
                    new Vector3(1, 1, 1),
                    new Vector3(1, 1, -1)
                });
                break;
            case FaceDirection.bottomFace:
                facePoints.AddRange(new List<Vector3>() {
                    new Vector3(-1, -1, -1),
                    new Vector3(-1, -1, 1),
                    new Vector3(1, -1, 1),
                    new Vector3(1, -1, -1)
                });
                break;
            case FaceDirection.frontFace:
                facePoints.AddRange(new List<Vector3>() {
                    new Vector3(-1, -1, 1),
                    new Vector3(-1, 1, 1),
                    new Vector3(1, 1, 1),
                    new Vector3(1, -1, 1)
                });
                break;
            case FaceDirection.backFace:
                facePoints.AddRange(new List<Vector3>() {
                    new Vector3(-1, -1, -1),
                    new Vector3(-1, 1, -1),
                    new Vector3(1, 1, -1),
                    new Vector3(1, -1, -1)
                });
                break;
        }

        return facePoints;
    }


    [Button("Get Boundary Points")]
    Dictionary<FaceDirection, List<Vector3>> GetBoundaryPoints(GameObject go)
    {
        var mesh = go.GetComponent<MeshFilter>().sharedMesh;

        List<Vector3> frontFaceVerts = new List<Vector3>();
        foreach (var faceVert in GetBoundaryPointsForDirection(go, FaceDirection.frontFace))
        {
            foreach (var meshVert in mesh.vertices)
            {
                if (Mathf.Abs(meshVert.x - faceVert.x) < 0.001 && !frontFaceVerts.Contains(meshVert))
                {
                    frontFaceVerts.Add(meshVert);
                }
            }
        }

        List<Vector3> backFaceVerts = new List<Vector3>();
        foreach (var faceVert in GetBoundaryPointsForDirection(go, FaceDirection.backFace))
        {
            foreach (var meshVert in mesh.vertices)
            {
                if (Mathf.Abs(meshVert.x - faceVert.x) < 0.001 && !backFaceVerts.Contains(meshVert))
                {
                    backFaceVerts.Add(meshVert);
                }
            }
        }

        List<Vector3> leftFaceVerts = new List<Vector3>();
        foreach (var faceVert in GetBoundaryPointsForDirection(go, FaceDirection.leftFace))
        {
            foreach (var meshVert in mesh.vertices)
            {
                if (Mathf.Abs(meshVert.x - faceVert.x) < 0.001 && !leftFaceVerts.Contains(meshVert))
                {
                    leftFaceVerts.Add(meshVert);
                }
            }
        }

        List<Vector3> rightFaceVerts = new List<Vector3>();
        foreach (var faceVert in GetBoundaryPointsForDirection(go, FaceDirection.rightFace))
        {
            foreach (var meshVert in mesh.vertices)
            {
                if (Mathf.Abs(meshVert.x - faceVert.x) < 0.001 && !rightFaceVerts.Contains(meshVert))
                {
                    rightFaceVerts.Add(meshVert);
                }
            }
        }

        List<Vector3> topFaceVerts = new List<Vector3>();
        foreach (var faceVert in GetBoundaryPointsForDirection(go, FaceDirection.topFace))
        {
            foreach (var meshVert in mesh.vertices)
            {
                if (Mathf.Abs(meshVert.y - faceVert.y) < 0.001 && !topFaceVerts.Contains(meshVert))
                {
                    topFaceVerts.Add(meshVert);
                }
            }
        }

        List<Vector3> bottomFaceVerts = new List<Vector3>();
        foreach (var faceVert in GetBoundaryPointsForDirection(go, FaceDirection.bottomFace))
        {
            foreach (var meshVert in mesh.vertices)
            {
                if (Mathf.Abs(meshVert.y - faceVert.y) < 0.001 && !bottomFaceVerts.Contains(meshVert))
                {
                    bottomFaceVerts.Add(meshVert);
                }
            }
        }

        var backFaceNormalised = backFaceVerts.ConvertAll(v => new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z)));
        var frontFaceNormalised = frontFaceVerts.ConvertAll(v => new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z)));
        var leftFaceNormalised = leftFaceVerts.ConvertAll(v => new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z)));
        var rightFaceNormalised = rightFaceVerts.ConvertAll(v => new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z)));
        var topFaceNormalised = topFaceVerts.ConvertAll(v => new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z)));
        var bottomFaceNormalised = bottomFaceVerts.ConvertAll(v => new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z)));

        return new Dictionary<FaceDirection, List<Vector3>>(){
            {FaceDirection.frontFace, frontFaceNormalised},
            {FaceDirection.backFace, backFaceNormalised},
            {FaceDirection.leftFace, leftFaceNormalised},
            {FaceDirection.rightFace, rightFaceNormalised},
            {FaceDirection.topFace, topFaceNormalised},
            {FaceDirection.bottomFace, bottomFaceNormalised},
        };
    }

    [Button("Check Vert Match")]
    void CheckVertMatch(GameObject tileA, GameObject tileB)
    {
        var tileABoundaryPoints = GetBoundaryPoints(tileA);
        var tileBBoundaryPoints = GetBoundaryPoints(tileB);

    }

    void OnDrawGizmos()
    {
        foreach (var point in redGizmoPoints)
        {
            // set color to red
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(point, 0.1f);
        }
        foreach (var point in greenGizmoPoints)
        {
            // set color to red
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(point, 0.1f);
        }
        foreach (var point in blueGizmoPoints)
        {
            // set color to red
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(point, 0.1f);
        }

        foreach (var point in gizmoShapes)
        {
            Gizmos.DrawCube(point, new Vector3(2, 2, 2));
        }
    }

    void Test()
    {

    }
}
