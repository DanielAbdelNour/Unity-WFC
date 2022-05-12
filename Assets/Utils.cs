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
        List<Vector3> tileBVerts = new List<Vector3>(StaticUtils.TransformMesh(tileBMesh, Vector3.zero,
            Quaternion.identity, new Vector3Int(-1, 1, 1)));
        tileBMesh.vertices = tileBVerts.ToArray();
    }

    [Button("Mirror Y")]
    public static void MirrorZ(GameObject go)
    {
        MeshFilter tileBMeshFilter = go.GetComponent<MeshFilter>();
        Mesh tileBMesh = tileBMeshFilter.sharedMesh;
        List<Vector3> tileBVerts = new List<Vector3>(StaticUtils.TransformMesh(tileBMesh, Vector3.zero,
            Quaternion.identity, new Vector3Int(1, -1, 1)));
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

    public static List<Vector3> GetBoundaryPointsForDirection(FaceDirection direction)
    {
        List<Vector3> facePoints = new List<Vector3>();

        switch (direction)
        {
            case FaceDirection.leftFace:
                facePoints.AddRange(new List<Vector3>()
                {
                    new Vector3(-1, -1, -1),
                    new Vector3(-1, -1, 1),
                    new Vector3(-1, 1, 1),
                    new Vector3(-1, 1, -1)
                });
                break;
            case FaceDirection.rightFace:
                facePoints.AddRange(new List<Vector3>()
                {
                    new Vector3(1, -1, -1),
                    new Vector3(1, -1, 1),
                    new Vector3(1, 1, 1),
                    new Vector3(1, 1, -1)
                });
                break;
            case FaceDirection.topFace:
                facePoints.AddRange(new List<Vector3>()
                {
                    new Vector3(-1, 1, -1),
                    new Vector3(-1, 1, 1),
                    new Vector3(1, 1, 1),
                    new Vector3(1, 1, -1)
                });
                break;
            case FaceDirection.bottomFace:
                facePoints.AddRange(new List<Vector3>()
                {
                    new Vector3(-1, -1, -1),
                    new Vector3(-1, -1, 1),
                    new Vector3(1, -1, 1),
                    new Vector3(1, -1, -1)
                });
                break;
            case FaceDirection.frontFace:
                facePoints.AddRange(new List<Vector3>()
                {
                    new Vector3(-1, -1, 1),
                    new Vector3(-1, 1, 1),
                    new Vector3(1, 1, 1),
                    new Vector3(1, -1, 1)
                });
                break;
            case FaceDirection.backFace:
                facePoints.AddRange(new List<Vector3>()
                {
                    new Vector3(-1, -1, -1),
                    new Vector3(-1, 1, -1),
                    new Vector3(1, 1, -1),
                    new Vector3(1, -1, -1)
                });
                break;
        }

        return facePoints;
    }

    public static Dictionary<FaceDirection, List<Vector3>> GetBoundaryPointsFromVerts(List<Vector3> verts, bool normalise = true)
    {
        List<Vector3> frontFaceVerts = new List<Vector3>();

        foreach (var faceVert in GetBoundaryPointsForDirection(FaceDirection.frontFace))
        {
            foreach (var meshVert in verts)
            {
                if (Mathf.Abs(meshVert.x - faceVert.x) < 0.001 && !frontFaceVerts.Contains(meshVert))
                {
                    frontFaceVerts.Add(meshVert);
                }
            }
        }

        List<Vector3> backFaceVerts = new List<Vector3>();
        foreach (var faceVert in GetBoundaryPointsForDirection(FaceDirection.backFace))
        {
            foreach (var meshVert in verts)
            {
                if (Mathf.Abs(meshVert.x - faceVert.x) < 0.001 && !backFaceVerts.Contains(meshVert))
                {
                    backFaceVerts.Add(meshVert);
                }
            }
        }

        List<Vector3> leftFaceVerts = new List<Vector3>();
        foreach (var faceVert in GetBoundaryPointsForDirection(FaceDirection.leftFace))
        {
            foreach (var meshVert in verts)
            {
                if (Mathf.Abs(meshVert.x - faceVert.x) < 0.001 && !leftFaceVerts.Contains(meshVert))
                {
                    leftFaceVerts.Add(meshVert);
                }
            }
        }

        List<Vector3> rightFaceVerts = new List<Vector3>();
        foreach (var faceVert in GetBoundaryPointsForDirection(FaceDirection.rightFace))
        {
            foreach (var meshVert in verts)
            {
                if (Mathf.Abs(meshVert.x - faceVert.x) < 0.001 && !rightFaceVerts.Contains(meshVert))
                {
                    rightFaceVerts.Add(meshVert);
                }
            }
        }

        List<Vector3> topFaceVerts = new List<Vector3>();
        foreach (var faceVert in GetBoundaryPointsForDirection(FaceDirection.topFace))
        {
            foreach (var meshVert in verts)
            {
                if (Mathf.Abs(meshVert.y - faceVert.y) < 0.001 && !topFaceVerts.Contains(meshVert))
                {
                    topFaceVerts.Add(meshVert);
                }
            }
        }

        List<Vector3> bottomFaceVerts = new List<Vector3>();
        foreach (var faceVert in GetBoundaryPointsForDirection(FaceDirection.bottomFace))
        {
            foreach (var meshVert in verts)
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


        return new Dictionary<FaceDirection, List<Vector3>>()
        {
            { FaceDirection.frontFace, normalise ? frontFaceNormalised : frontFaceVerts },
            { FaceDirection.backFace, normalise ? backFaceNormalised : backFaceVerts },
            { FaceDirection.leftFace, normalise ? leftFaceNormalised : leftFaceVerts },
            { FaceDirection.rightFace, normalise ? rightFaceNormalised : rightFaceVerts },
            { FaceDirection.topFace, normalise ? topFaceNormalised : topFaceVerts },
            { FaceDirection.bottomFace, normalise ? bottomFaceNormalised : bottomFaceVerts },
        };
    }

    public static Dictionary<FaceDirection, int> GetHashedBoundaryPointsFromVerts(List<Vector3> verts, bool normalise = true)
    {
        const float eps = 0.001f;
        
        List<Vector3> frontFaceVerts = new List<Vector3>();
        foreach (var meshVert in verts)
        {
            if (Mathf.Abs(meshVert.z - 1f) < eps  && !frontFaceVerts.Contains(meshVert))
            {
                frontFaceVerts.Add(meshVert);
            }
        }

        List<Vector3> backFaceVerts = new List<Vector3>();
        foreach (var faceVert in GetBoundaryPointsForDirection(FaceDirection.backFace))
        {
            foreach (var meshVert in verts)
            {
                if (Mathf.Abs(meshVert.x - faceVert.x) < eps && !backFaceVerts.Contains(meshVert))
                {
                    backFaceVerts.Add(meshVert);
                }
            }
        }

        List<Vector3> leftFaceVerts = new List<Vector3>();
        foreach (var faceVert in GetBoundaryPointsForDirection(FaceDirection.leftFace))
        {
            foreach (var meshVert in verts)
            {
                if (Mathf.Abs(meshVert.x - faceVert.x) < eps && !leftFaceVerts.Contains(meshVert))
                {
                    leftFaceVerts.Add(meshVert);
                }
            }
        }

        List<Vector3> rightFaceVerts = new List<Vector3>();
        foreach (var faceVert in GetBoundaryPointsForDirection(FaceDirection.rightFace))
        {
            foreach (var meshVert in verts)
            {
                if (Mathf.Abs(meshVert.x - faceVert.x) < eps && !rightFaceVerts.Contains(meshVert))
                {
                    rightFaceVerts.Add(meshVert);
                }
            }
        }

        List<Vector3> topFaceVerts = new List<Vector3>();
        foreach (var faceVert in GetBoundaryPointsForDirection(FaceDirection.topFace))
        {
            foreach (var meshVert in verts)
            {
                if (Mathf.Abs(meshVert.y - faceVert.y) < eps && !topFaceVerts.Contains(meshVert))
                {
                    topFaceVerts.Add(meshVert);
                }
            }
        }

        List<Vector3> bottomFaceVerts = new List<Vector3>();
        foreach (var faceVert in GetBoundaryPointsForDirection(FaceDirection.bottomFace))
        {
            foreach (var meshVert in verts)
            {
                if (Mathf.Abs(meshVert.y - faceVert.y) < eps && !bottomFaceVerts.Contains(meshVert))
                {
                    bottomFaceVerts.Add(meshVert);
                }
            }
        }
        
        var frontFaceNormalised = frontFaceVerts.ConvertAll(v => new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z)));
        var backFaceNormalised = backFaceVerts.ConvertAll(v => new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z)));
        var leftFaceNormalised = leftFaceVerts.ConvertAll(v => new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z)));
        var rightFaceNormalised = rightFaceVerts.ConvertAll(v => new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z)));
        var topFaceNormalised = topFaceVerts.ConvertAll(v => new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z)));
        var bottomFaceNormalised = bottomFaceVerts.ConvertAll(v => new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z)));


        return new Dictionary<FaceDirection, int>()
        {
            { FaceDirection.frontFace, GetVertsHash(normalise ? frontFaceNormalised : frontFaceVerts) },
            { FaceDirection.backFace, GetVertsHash(normalise ? backFaceNormalised : backFaceVerts) },
            { FaceDirection.leftFace, GetVertsHash(normalise ? leftFaceNormalised : leftFaceVerts) },
            { FaceDirection.rightFace, GetVertsHash(normalise ? rightFaceNormalised : rightFaceVerts) },
            { FaceDirection.topFace, GetVertsHash(normalise ? topFaceNormalised : topFaceVerts) },
            { FaceDirection.bottomFace, GetVertsHash(normalise ? bottomFaceNormalised : bottomFaceVerts) },
        };
    }


    [Button("Get Boundary Points")]
    public static Dictionary<FaceDirection, List<Vector3>> GetBoundaryPoints(GameObject go)
    {
        var mesh = go.GetComponent<MeshFilter>().sharedMesh;

        List<Vector3> frontFaceVerts = new List<Vector3>();
        foreach (var faceVert in GetBoundaryPointsForDirection(FaceDirection.frontFace))
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
        foreach (var faceVert in GetBoundaryPointsForDirection(FaceDirection.backFace))
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
        foreach (var faceVert in GetBoundaryPointsForDirection(FaceDirection.leftFace))
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
        foreach (var faceVert in GetBoundaryPointsForDirection(FaceDirection.rightFace))
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
        foreach (var faceVert in GetBoundaryPointsForDirection(FaceDirection.topFace))
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
        foreach (var faceVert in GetBoundaryPointsForDirection(FaceDirection.bottomFace))
        {
            foreach (var meshVert in mesh.vertices)
            {
                if (Mathf.Abs(meshVert.y - faceVert.y) < 0.001 && !bottomFaceVerts.Contains(meshVert))
                {
                    bottomFaceVerts.Add(meshVert);
                }
            }
        }

        var backFaceNormalised =
            backFaceVerts.ConvertAll(v => new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z)));
        var frontFaceNormalised =
            frontFaceVerts.ConvertAll(v => new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z)));
        var leftFaceNormalised =
            leftFaceVerts.ConvertAll(v => new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z)));
        var rightFaceNormalised =
            rightFaceVerts.ConvertAll(v => new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z)));
        var topFaceNormalised =
            topFaceVerts.ConvertAll(v => new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z)));
        var bottomFaceNormalised =
            bottomFaceVerts.ConvertAll(v => new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z)));

        return new Dictionary<FaceDirection, List<Vector3>>()
        {
            { FaceDirection.frontFace, frontFaceNormalised },
            { FaceDirection.backFace, backFaceNormalised },
            { FaceDirection.leftFace, leftFaceNormalised },
            { FaceDirection.rightFace, rightFaceNormalised },
            { FaceDirection.topFace, topFaceNormalised },
            { FaceDirection.bottomFace, bottomFaceNormalised },
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
                var vertHash = new Vector3(Mathf.Abs(vert.x), Mathf.Abs(vert.y), Mathf.Abs(vert.z)).ToString()
                    .GetHashCode();
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

    public static List<Vector3> GetRotatedMeshVerts(Mesh mesh, int rot)
    {
        List<Vector3> rotatedVerts = new List<Vector3>(StaticUtils.TransformMesh(mesh, Vector3.zero,
            Quaternion.Euler(0, rot, 0), new Vector3Int(1, 1, 1)));
        return rotatedVerts;
    }

    public static int GetMeshHash(GameObject go)
    {
        var mesh = go.GetComponent<MeshFilter>().sharedMesh;
        int meshHash = 0;
        foreach (var rot in new int[] { 0, 90, 180, 270 })
        {
            List<Vector3> rotatedVerts = new List<Vector3>(StaticUtils.TransformMesh(mesh, Vector3.zero,
                Quaternion.Euler(0, rot, 0), new Vector3Int(1, 1, 1)));

            foreach (var vert in rotatedVerts)
            {
                var vertHash = new Vector3(Mathf.Abs(vert.x), Mathf.Abs(vert.y), Mathf.Abs(vert.z)).ToString()
                    .GetHashCode();
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

        List<Vector3> rotatedVerts = new List<Vector3>(StaticUtils.TransformMesh(mesh, Vector3.zero,
            Quaternion.Euler(0, rot, 0), new Vector3Int(1, 1, 1)));

        foreach (var vert in rotatedVerts)
        {
            var vertHash = new Vector3(Mathf.Abs(vert.x), Mathf.Abs(vert.y), Mathf.Abs(vert.z)).ToString()
                .GetHashCode();
            meshHash += vertHash;
        }

        Debug.Log(meshHash);
        return meshHash;
    }


    /* final dictionary will look like:
    modules: [
        "moduleA": {
            objectName: "objectX",
            rotation: 0
            sockets: [
                posX: "21",
                negX: "24s".
                posY: "24s",
                negY: "61",
                posZ: "-1",
                negZ: "v0_0"
            ]
        },
        "moduleB": {
            objectName: "objectX",
            rotation: 90
            sockets: [
                posX: "24s",
                negX: "25".
                posY: "29",
                negY: "61",
                posZ: "-1",
                negZ: "v0_1"
            ]
        },
        ...
    ]
    */
    public static void GenerateWFCModuleAssets(GameObject go)
    {
        // hash the verts for each faceDirection
        // the resulting hash is the socket name for that faceDirection

        // loop through all the child gameObjects (each child should be a tile)
        for (var i = 0; i < go.transform.childCount; i++)
        {
            var child = go.transform.GetChild(i);

            // repeat the process for every rotation of the tile
            foreach (var rot in new int[] { 0, 90, 180, 270 })
            {
                // first get the rotated verts for the mesh
                var mesh = child.GetComponent<MeshFilter>().sharedMesh;
                var rotatedVerts = GetRotatedMeshVerts(mesh, rot);

                // get the hashed boundary verts for this rotation (each has is the socket name)
                var hashedBoundaryPoints = GetHashedBoundaryPointsFromVerts(rotatedVerts);

                Debug.Log("Test");
            }
        }
    }

    /// <summary>
    /// Returns the hash of the provided array of verts
    /// </summary>
    public static int GetVertsHash(List<Vector3> verts, bool normalise = true)
    {
        var faceDirectionHash = 0;
        foreach (var vert in verts)
        {
            var vertHash = new Vector3(
                    normalise ? Mathf.Abs(vert.x) : vert.x,
                    normalise ? Mathf.Abs(vert.y) : vert.y,
                    normalise ? Mathf.Abs(vert.z) : vert.z)
                .ToString().GetHashCode();
            faceDirectionHash += vertHash;
        }

        return faceDirectionHash;
    }
}