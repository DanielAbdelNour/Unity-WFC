using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace WFC
{
    public static class WFCUtils
    {
        public enum Direction
        {
            Left,
            Right,
            Up,
            Down,
            Forward,
            Back
        }

        public static Dictionary<Vector3Int, Direction> VecToDir = new()
        {
            { Vector3Int.right, Direction.Right },
            { Vector3Int.left, Direction.Left },
            { Vector3Int.forward, Direction.Forward },
            { Vector3Int.back, Direction.Back },
            { Vector3Int.up, Direction.Up },
            { Vector3Int.down, Direction.Down },
        };

        public static int GenerateVertsHash(List<Vector3> verts, bool absolute = true)
        {
            var meshHash = 0;
            foreach (var meshVert in verts)
            {
                var hash = absolute
                    ? new Vector3(Mathf.Abs(meshVert.x), Mathf.Abs(meshVert.y), Mathf.Abs(meshVert.z)).ToString().GetHashCode()
                    : new Vector3(meshVert.x, meshVert.y, meshVert.z).ToString().GetHashCode();
                meshHash += hash;
            }

            return meshHash;
        }

        public static int BoundaryHashForDirection(List<Vector3> verts, Direction dir, bool absolute = true)
        {
            return dir switch
            {
                Direction.Forward => GenerateVertsHash(verts.Where(vert => Mathf.Abs(vert.z - 1) < 0.001f).ToList(), absolute),
                Direction.Back => GenerateVertsHash(verts.Where(vert => Mathf.Abs(vert.z - -1) < 0.001f).ToList(), absolute),
                Direction.Right => GenerateVertsHash(verts.Where(vert => Mathf.Abs(vert.x - 1) < 0.001f).ToList(), absolute),
                Direction.Left => GenerateVertsHash(verts.Where(vert => Mathf.Abs(vert.x - -1) < 0.001f).ToList(), absolute),
                Direction.Up => GenerateVertsHash(verts.Where(vert => Mathf.Abs(vert.y - 1) < 0.001f).ToList(), absolute),
                Direction.Down => GenerateVertsHash(verts.Where(vert => Mathf.Abs(vert.y - -1) < 0.001f).ToList(), absolute),
                _ => -1
            };
        }
        
        /// <summary>
        /// Apply a transformation matrix to a mesh
        /// </summary>
        /// <param name="mesh">The mesh to transform</param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="scale"></param>
        /// <returns>A list of transformed vertices</returns>
        public static Mesh TransformMesh(Mesh mesh, Vector3 position, Quaternion rotation, Vector3 scale)
        {
           
            var vertices = mesh.vertices;
            var matrix = Matrix4x4.TRS(position, rotation, scale);
            for (var i = 0; i < vertices.Length; i++)
            {
                vertices[i] = matrix.MultiplyPoint(vertices[i]);
            }
            var newMesh = new Mesh()
            {
                vertices = vertices, 
                triangles = mesh.triangles, 
                normals = mesh.normals, 
                tangents = mesh.tangents, 
                bounds = mesh.bounds,
                uv = mesh.uv
            };
            // newMesh.RecalculateBounds();
            // newMesh.RecalculateNormals();
            // newMesh.RecalculateTangents();
            return newMesh;
        }
        
        /// <summary>
        /// Will generate a WFCModule asset (scriptable object) for every child GameObject in go
        /// </summary>
        /// <param name="go">Parent GameObject containing desired module meshes</param>
        public static void GenerateWFCModuleAssets(GameObject go)
        {
            // delete any module asset folder if it exists
            if (AssetDatabase.IsValidFolder("Assets/WFC/Modules"))
            {
                AssetDatabase.DeleteAsset("Assets/WFC/Modules");
            }
            AssetDatabase.CreateFolder("Assets/WFC", "Modules");
            
            // loop through all child GameObjects, store any mesh meta-data, and remove duplicate meshes
            var uniqueModules = new Dictionary<int, WFCModule>();
            for(var i = 0; i < go.transform.childCount; i++)
            {
                var child = go.transform.GetChild(i);
                var meshFilter = child.gameObject.GetComponent<MeshFilter>();
                if(meshFilter == null) continue;
                var originalMesh = meshFilter.sharedMesh;

                foreach (var rot in new[] { 0, 90, 180, 270 })
                {
                    var newMesh = TransformMesh(originalMesh, Vector3.zero, Quaternion.Euler(0, rot, 0), Vector3.one);
                    var newMeshHash = GenerateVertsHash(newMesh.vertices.ToList(), absolute:false);
                    if (uniqueModules.ContainsKey(newMeshHash)) continue;
                    var wfcModule = ScriptableObject.CreateInstance<WFCModule>();

                    var gameObject = child.gameObject;
                    wfcModule.module = gameObject;
                    wfcModule.rotation = rot;
                    wfcModule.moduleHash = newMeshHash;
                    wfcModule.moduleMesh = newMesh;
                    
                    wfcModule.name = gameObject.name + "_" + rot;

                    wfcModule.leftFaceHash = BoundaryHashForDirection(newMesh.vertices.ToList(), Direction.Left, false);
                    wfcModule.rightFaceHash = BoundaryHashForDirection(newMesh.vertices.ToList(), Direction.Right, false);
                    wfcModule.upFaceHash = BoundaryHashForDirection(newMesh.vertices.ToList(), Direction.Up, false);
                    wfcModule.downFaceHash = BoundaryHashForDirection(newMesh.vertices.ToList(), Direction.Down, false);
                    wfcModule.forwardFaceHash = BoundaryHashForDirection(newMesh.vertices.ToList(), Direction.Forward, false);
                    wfcModule.backFaceHash = BoundaryHashForDirection(newMesh.vertices.ToList(), Direction.Back, false);
                    
                    uniqueModules.Add(newMeshHash, wfcModule);
                }
            }

            var emptyModule = ScriptableObject.CreateInstance<WFCModule>();
            emptyModule.name = "Empty";
            emptyModule.leftFaceHash = 0;
            emptyModule.rightFaceHash = 0;
            emptyModule.upFaceHash = 0;
            emptyModule.downFaceHash = 0;
            emptyModule.forwardFaceHash = 0;
            emptyModule.backFaceHash = 0;
            emptyModule.isEmpty = true;
            
            uniqueModules.Add(-1, emptyModule);
            
            foreach(var module in uniqueModules.Values)
            {
                foreach (var otherModule in uniqueModules.Values)
                {
                    if (module.IsValidNeighbour(otherModule, Direction.Right))
                    {
                        if(!module.rightValidNeighbours.Contains(otherModule)) module.rightValidNeighbours.Add(otherModule);
                    }
                    if (module.IsValidNeighbour(otherModule, Direction.Left))
                    {
                        if(!module.leftValidNeighbours.Contains(otherModule)) module.leftValidNeighbours.Add(otherModule);
                    }
                    if (module.IsValidNeighbour(otherModule, Direction.Forward))
                    {
                        if(!module.forwardValidNeighbours.Contains(otherModule)) module.forwardValidNeighbours.Add(otherModule);
                    }
                    if (module.IsValidNeighbour(otherModule, Direction.Back))
                    {
                        if(!module.backValidNeighbours.Contains(otherModule)) module.backValidNeighbours.Add(otherModule);
                    }
                    if (module.IsValidNeighbour(otherModule, Direction.Up))
                    {
                        if(!module.upValidNeighbours.Contains(otherModule)) module.upValidNeighbours.Add(otherModule);
                    }
                    if (module.IsValidNeighbour(otherModule, Direction.Down))
                    {
                        if(!module.downValidNeighbours.Contains(otherModule)) module.downValidNeighbours.Add(otherModule);
                    }
                }
                AssetDatabase.CreateAsset(module, "Assets/WFC/Modules/" + module.name  + ".asset");
            }

            var moduleSet = ScriptableObject.CreateInstance<WFCModuleSet>();
            moduleSet.modules = uniqueModules.Values.ToList();
            
            AssetDatabase.CreateAsset(moduleSet, "Assets/WFC/Modules/ModuleSet.asset");

        }

        public static List<WFCModule> GetValidNeighboursForDirection(List<WFCModule> candidates, WFCModule module, Direction dir)
        {
            // foreach (var otherModule in moduleSet.modules)
            // {
            //     if (module.IsValidNeighbour(otherModule, dir))
            //     {
            //         validNeighbours.Add(otherModule);
            //     };
            // }
            var validNeighbours = candidates.FindAll(m => module.IsValidNeighbour(m, dir));
            return validNeighbours;
        }
        
        public static void GenerateCells(WFCModuleSet moduleSet, int gridSize = 4, int cellSize = 2)
        {
            var cells = new GameObject("WFCCells");
                
            for (var z = 0; z < gridSize; z++)
            {
                for (var x = 0; x < gridSize; x++)
                {
                    var cell = new GameObject("cell", new[]{ typeof(WFCCell)})
                    {
                        transform =
                        {
                            position = new Vector3(x*cellSize, 0, z*cellSize),
                            parent = cells.transform
                        }
                    };
                    cell.GetComponent<WFCCell>().candidates = new List<WFCModule>(moduleSet.modules);
                }
            }
        }
    }
}
