using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;


public static class Utils
{
    [Button("Mirror X")]
    public static void MirrorX(GameObject go)
    {
        MeshFilter tileBMeshFilter = go.GetComponent<MeshFilter>();
        Mesh tileBMesh = tileBMeshFilter.sharedMesh;
        List<Vector3> tileBVerts = new List<Vector3>(StaticUtils.TransformMesh(tileBMesh, Vector3.zero, Quaternion.identity, new Vector3Int(-1, 1, 1)));
        tileBMesh.vertices = tileBVerts.ToArray();
    }

    [Button("Mirror Y")]
    public static void MirrorZ(GameObject go)
    {
        MeshFilter tileBMeshFilter = go.GetComponent<MeshFilter>();
        Mesh tileBMesh = tileBMeshFilter.sharedMesh;
        List<Vector3> tileBVerts = new List<Vector3>(StaticUtils.TransformMesh(tileBMesh, Vector3.zero, Quaternion.identity, new Vector3Int(1, -1, 1)));
        tileBMesh.vertices = tileBVerts.ToArray();
    }

    [Button("Flip Triangles")]
    public static void FilpTriangles(GameObject go)
    {
        MeshFilter tileBMeshFilter = go.GetComponent<MeshFilter>();
        Mesh tileBMesh = tileBMeshFilter.sharedMesh;
        List<int> newTriangles = new List<int>(tileBMesh.triangles);
        newTriangles.Reverse();
        tileBMesh.SetTriangles(newTriangles, 0);
    }

    public enum FaceDirection
    {
        leftFace,
        rightFace,
        topFace,
        bottomFace,
        frontFace,
        backFace
    }

    public static List<Vector3> GetBoundaryPointsForDirection(GameObject go, FaceDirection direction)
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
    public static Dictionary<FaceDirection, List<Vector3>> GetBoundaryPoints(GameObject go)
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

    public static Dictionary<int, GameObject> GetUniqueMeshes(GameObject go)
    {
        var meshFilters = go.GetComponentsInChildren<MeshFilter>();
        var meshDict = new Dictionary<int, GameObject>();

        foreach (var meshFilter in meshFilters)
        {
            var mesh = meshFilter.sharedMesh;
            int meshHash = 0;
            foreach (var vert in mesh.vertices)
            {
                var vertHash = new Vector3(Mathf.Abs(vert.x), Mathf.Abs(vert.y), Mathf.Abs(vert.z)).ToString().GetHashCode();
                meshHash += vertHash;
            }
            if (!meshDict.ContainsKey(meshHash))
            {
                meshDict.Add(meshHash, meshFilter.gameObject);
            }
        }
        return meshDict;
    }

    [Button("Check Neighbour Match")]
    public static void CheckNeighbourMatch(GameObject tile, GameObject candidateNeighbour)
    {
        var tileBoundaryPoints = GetBoundaryPoints(tile);
        var candidateNeighbourBoundaryPoints = GetBoundaryPoints(candidateNeighbour);

        // is the candidate a valid neighbour to the left of this tile?
        // each face looks like {'left': [(x,y,z), (x,y,z), ...], 'right': [(x,y,z), (x,y,z), ...]}
    }

    public static int GetMeshHash(GameObject go)
    {
        var mesh = go.GetComponent<MeshFilter>().sharedMesh;
        int meshHash = 0;
        foreach (var rot in new int[] { 0, 90, 180, 270 })
        {
            List<Vector3> rotatedVerts = new List<Vector3>(StaticUtils.TransformMesh(mesh, Vector3.zero, Quaternion.Euler(0, rot, 0), new Vector3Int(1, 1, 1)));

            foreach (var vert in rotatedVerts)
            {
                var vertHash = new Vector3(Mathf.Abs(vert.x), Mathf.Abs(vert.y), Mathf.Abs(vert.z)).ToString().GetHashCode();
                meshHash += vertHash;
            }
        }
        Debug.Log(meshHash);
        return meshHash;
    }

    public static int GetMeshHashForRotation(GameObject go, int rot)
    {
        var mesh = go.GetComponent<MeshFilter>().sharedMesh;
        int meshHash = 0;

        List<Vector3> rotatedVerts = new List<Vector3>(StaticUtils.TransformMesh(mesh, Vector3.zero, Quaternion.Euler(0, rot, 0), new Vector3Int(1, 1, 1)));

        foreach (var vert in rotatedVerts)
        {
            var vertHash = new Vector3(Mathf.Abs(vert.x), Mathf.Abs(vert.y), Mathf.Abs(vert.z)).ToString().GetHashCode();
            meshHash += vertHash;
        }

        Debug.Log(meshHash);
        return meshHash;
    }


    public static void GenerateWFCModuleAssets(GameObject go)
    {
        var meshHashes = new Dictionary<int, GameObject>();

        for (var i = 0; i < go.transform.childCount; i++)
        {
            var child = go.transform.GetChild(i);
            foreach (var rot in new int[] { 0, 90, 180, 270 })
            {
                var meshHash = GetMeshHashForRotation(child.gameObject, 0);
                // add child if meshHash not in meshHashes
                if (!meshHashes.ContainsKey(meshHash))
                {
                    meshHashes.Add(meshHash, child.gameObject);
                }
            }
        }
    }

}