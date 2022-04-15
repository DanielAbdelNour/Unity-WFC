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

    public struct Edge
    {
        Vector3 v1;
        Vector3 v2;
        int triangleIndex;

        int hash;

        public Edge(Vector3 v1, Vector3 v2, int triangleIndex)
        {
            this.v1 = v1;
            this.v2 = v2;
            this.triangleIndex = triangleIndex;
            this.hash = v1.GetHashCode() ^ v2.GetHashCode();
        }

        public int GetEdgeHash()
        {
            return v1.GetHashCode() + v2.GetHashCode();
        }
    }

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

    void ClearGizmos()
    {
        redGizmoPoints.Clear();
        greenGizmoPoints.Clear();
        blueGizmoPoints.Clear();
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


    [Button("Get Boundary Points")]
    void GetBoundaryPoints(GameObject go, out Dictionary<string, List<Vector3>> boundaryPoints, out Dictionary<string, List<string>> boundaryHashes, out Dictionary<string, List<string>> faceVertHashes)
    {
        ClearGizmos();

        var mesh = go.GetComponent<MeshFilter>().sharedMesh;

        var minBounds = mesh.bounds.min;
        var maxBounds = mesh.bounds.max;

        //front face
        var frontFacePoint1 = new Vector3(minBounds.x, minBounds.y, minBounds.z);
        var frontFacePoint2 = new Vector3(maxBounds.x, minBounds.y, minBounds.z);
        var frontFacePoint3 = new Vector3(maxBounds.x, maxBounds.y, minBounds.z);
        var frontFacePoint4 = new Vector3(minBounds.x, maxBounds.y, minBounds.z);

        //back face
        var backFacePoint1 = new Vector3(minBounds.x, minBounds.y, maxBounds.z);
        var backFacePoint2 = new Vector3(maxBounds.x, minBounds.y, maxBounds.z);
        var backFacePoint3 = new Vector3(maxBounds.x, maxBounds.y, maxBounds.z);
        var backFacePoint4 = new Vector3(minBounds.x, maxBounds.y, maxBounds.z);

        //left face
        var leftFacePoint1 = new Vector3(minBounds.x, minBounds.y, minBounds.z);
        var leftFacePoint2 = new Vector3(minBounds.x, minBounds.y, maxBounds.z);
        var leftFacePoint3 = new Vector3(minBounds.x, maxBounds.y, maxBounds.z);
        var leftFacePoint4 = new Vector3(minBounds.x, maxBounds.y, minBounds.z);

        //right face
        var rightFacePoint1 = new Vector3(maxBounds.x, minBounds.y, minBounds.z);
        var rightFacePoint2 = new Vector3(maxBounds.x, minBounds.y, maxBounds.z);
        var rightFacePoint3 = new Vector3(maxBounds.x, maxBounds.y, maxBounds.z);
        var rightFacePoint4 = new Vector3(maxBounds.x, maxBounds.y, minBounds.z);

        //top face
        var topFacePoint1 = new Vector3(minBounds.x, maxBounds.y, minBounds.z);
        var topFacePoint2 = new Vector3(maxBounds.x, maxBounds.y, minBounds.z);
        var topFacePoint3 = new Vector3(maxBounds.x, maxBounds.y, maxBounds.z);
        var topFacePoint4 = new Vector3(minBounds.x, maxBounds.y, maxBounds.z);

        //bottom face
        var bottomFacePoint1 = new Vector3(minBounds.x, minBounds.y, minBounds.z);
        var bottomFacePoint2 = new Vector3(maxBounds.x, minBounds.y, minBounds.z);
        var bottomFacePoint3 = new Vector3(maxBounds.x, minBounds.y, maxBounds.z);
        var bottomFacePoint4 = new Vector3(minBounds.x, minBounds.y, maxBounds.z);

        var backFaceHashes = new List<string>() {
            backFacePoint1.ToString(),
            backFacePoint2.ToString(),
            backFacePoint3.ToString(),
            backFacePoint4.ToString()
        };
        var frontFaceHashes = new List<string>() {
            frontFacePoint1.ToString(),
            frontFacePoint2.ToString(),
            frontFacePoint3.ToString(),
            frontFacePoint4.ToString()
        };
        var leftFaceHashes = new List<string>() {
            leftFacePoint1.ToString(),
            leftFacePoint2.ToString(),
            leftFacePoint3.ToString(),
            leftFacePoint4.ToString()
        };
        var rightFaceHashes = new List<string>() {
            rightFacePoint1.ToString(),
            rightFacePoint2.ToString(),
            rightFacePoint3.ToString(),
            rightFacePoint4.ToString()
        };
        var topFaceHashes = new List<string>() {
            topFacePoint1.ToString(),
            topFacePoint2.ToString(),
            topFacePoint3.ToString(),
            topFacePoint4.ToString()
        };
        var bottomFaceHashes = new List<string>() {
            bottomFacePoint1.ToString(),
            bottomFacePoint2.ToString(),
            bottomFacePoint3.ToString(),
            bottomFacePoint4.ToString()
        };

        boundaryPoints = new Dictionary<string, List<Vector3>>() {
            { "backFace", new List<Vector3>() { backFacePoint1, backFacePoint2, backFacePoint3, backFacePoint4 } },
            { "frontFace", new List<Vector3>() { frontFacePoint1, frontFacePoint2, frontFacePoint3, frontFacePoint4 } },
            { "leftFace", new List<Vector3>() { leftFacePoint1, leftFacePoint2, leftFacePoint3, leftFacePoint4 } },
            { "rightFace", new List<Vector3>() { rightFacePoint1, rightFacePoint2, rightFacePoint3, rightFacePoint4 } },
            { "topFace", new List<Vector3>() { topFacePoint1, topFacePoint2, topFacePoint3, topFacePoint4 } },
            { "bottomFace", new List<Vector3>() { bottomFacePoint1, bottomFacePoint2, bottomFacePoint3, bottomFacePoint4 } },
        };

        boundaryHashes = new Dictionary<string, List<string>>() {
            {"backFace", backFaceHashes},
            {"frontFace", frontFaceHashes},
            {"leftFace", leftFaceHashes},
            {"rightFace", rightFaceHashes},
            {"topFace", topFaceHashes},
            {"bottomFace", bottomFaceHashes},
        };

        var bh = new Dictionary<string, List<string>>(boundaryHashes);

        List<Vector3> frontFaceVerts = new List<Vector3>();
        foreach (var faceVert in boundaryPoints["frontFace"])
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
        foreach (var faceVert in boundaryPoints["backFace"])
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
        foreach (var faceVert in boundaryPoints["leftFace"])
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
        foreach (var faceVert in boundaryPoints["rightFace"])
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
        foreach (var faceVert in boundaryPoints["topFace"])
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
        foreach (var faceVert in boundaryPoints["bottomFace"])
        {
            foreach (var meshVert in mesh.vertices)
            {
                if (Mathf.Abs(meshVert.y - faceVert.y) < 0.001 && !bottomFaceVerts.Contains(meshVert))
                {
                    bottomFaceVerts.Add(meshVert);
                }
            }
        }

        var backFaceNormalised = backFaceVerts.ConvertAll(x => x - mesh.bounds.center);
        var frontFaceNormalised = frontFaceVerts.ConvertAll(x => x + mesh.bounds.center);
        var leftFaceNormalised = leftFaceVerts.ConvertAll(x => x - mesh.bounds.center);
        var rightFaceNormalised = rightFaceVerts.ConvertAll(x => x + mesh.bounds.center);
        var bottomFaceNormalised = bottomFaceVerts.ConvertAll(x => x - mesh.bounds.center);
        var topFaceNormalised = topFaceVerts.ConvertAll(x => x + mesh.bounds.center);

        faceVertHashes = new Dictionary<string, List<string>>() {
            {"backFace", backFaceNormalised.ConvertAll(x => x.ToString())},
            {"frontFace", frontFaceNormalised.ConvertAll(x => x.ToString())},
            {"leftFace", leftFaceNormalised.ConvertAll(x => x.ToString())},
            {"rightFace", rightFaceNormalised.ConvertAll(x => x.ToString())},
            {"topFace", topFaceNormalised.ConvertAll(x => x.ToString())},
            {"bottomFace", bottomFaceNormalised.ConvertAll(x => x.ToString())},
        };


        redGizmoPoints.AddRange(leftFaceVerts.ConvertAll(x => go.transform.TransformPoint(x)));
        greenGizmoPoints.AddRange(rightFaceVerts.ConvertAll(x => go.transform.TransformPoint(x)));
    }

    [Button("Check Vert Match")]
    void CheckVertMatch(GameObject tileA, GameObject tileB)
    {
        var tileAMesh = tileA.GetComponent<MeshFilter>().sharedMesh;
        var tileBMesh = tileB.GetComponent<MeshFilter>().sharedMesh;

        Dictionary<string, List<Vector3>> boundaryPointsA = new Dictionary<string, List<Vector3>>();
        Dictionary<string, List<string>> boundaryHashesA = new Dictionary<string, List<string>>();

        Dictionary<string, List<Vector3>> boundaryPointsB = new Dictionary<string, List<Vector3>>();
        Dictionary<string, List<string>> boundaryHashesB = new Dictionary<string, List<string>>();

        Dictionary<string, List<string>> faceVertHashesA = new Dictionary<string, List<string>>();
        Dictionary<string, List<string>> faceVertHashesB = new Dictionary<string, List<string>>();

        GetBoundaryPoints(tileA, out boundaryPointsA, out boundaryHashesA, out faceVertHashesA);
        GetBoundaryPoints(tileB, out boundaryPointsB, out boundaryHashesB, out faceVertHashesB);
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
    }

    [Button("Check Edge Match")]
    void CheckEdgeMatch(GameObject tileA, GameObject tileB)
    {
        MeshFilter tileAMeshFilter = tileA.GetComponent<MeshFilter>();
        MeshFilter tileBMeshFilter = tileB.GetComponent<MeshFilter>();

        Mesh tileAMesh = tileAMeshFilter.sharedMesh;
        Mesh tileBMesh = tileBMeshFilter.sharedMesh;

        List<Vector3> tileAVerts = new List<Vector3>(tileAMesh.vertices);
        List<Vector3> tileBVerts = new List<Vector3>(tileBMesh.vertices);

        List<int> tileATriangles = new List<int>(tileAMesh.triangles);
        List<int> tileBTriangles = new List<int>(tileBMesh.triangles);

        List<Edge> tileAEdges = new List<Edge>();
        List<Edge> tileBEdges = new List<Edge>();

        for (int i = 0; i < tileATriangles.Count; i += 3)
        {
            Vector3 a = tileAVerts[tileATriangles[i]];
            Vector3 b = tileAVerts[tileATriangles[i + 1]];
            Vector3 c = tileAVerts[tileATriangles[i + 2]];
            tileAEdges.Add(new Edge(a, b, i));
            tileAEdges.Add(new Edge(b, c, i));
            tileAEdges.Add(new Edge(c, a, i));
        }

        for (int i = 0; i < tileBTriangles.Count; i += 3)
        {
            Vector3 a = tileBVerts[tileBTriangles[i]];
            Vector3 b = tileBVerts[tileBTriangles[i + 1]];
            Vector3 c = tileBVerts[tileBTriangles[i + 2]];
            tileBEdges.Add(new Edge(a, b, i));
            tileBEdges.Add(new Edge(b, c, i));
            tileBEdges.Add(new Edge(c, a, i));
        }

        // check for matching hashes
        foreach (Edge e in tileAEdges)
        {
            foreach (Edge f in tileBEdges)
            {
                if (e.GetEdgeHash() == f.GetEdgeHash())
                {
                    Debug.Log("Match found!");
                }
            }
        }

        List<int> aHash = tileAVerts.ConvertAll(x => x.normalized.GetHashCode());
        List<int> bHash = tileBVerts.ConvertAll(x => x.normalized.GetHashCode());

        foreach (int i in aHash)
        {
            if (bHash.Contains(i))
            {
                Debug.Log("Match found!");
            }
        }
    }

    void Test()
    {

    }
}
