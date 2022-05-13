using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;

namespace WFC
{
    public class WFCModule : SerializedScriptableObject
    {
        public GameObject module;
        public string moduleName;
        public int moduleHash;
        public Mesh moduleMesh;
        public int rotation;
        public bool mirrorX;
        public bool mirrorZ;
        public bool isEmpty;

        public int leftFaceHash;
        public int rightFaceHash;
        public int forwardFaceHash;
        public int backFaceHash;
        public int upFaceHash;
        public int downFaceHash;

        public List<WFCModule> leftValidNeighbours = new();
        public List<WFCModule> rightValidNeighbours = new();
        public List<WFCModule> forwardValidNeighbours= new();
        public List<WFCModule> backValidNeighbours= new();
        public List<WFCModule> upValidNeighbours= new();
        public List<WFCModule> downValidNeighbours = new();

        public readonly Dictionary<WFCUtils.Direction, List<WFCModule>> ValidNeighbours = new()
        {
            { WFCUtils.Direction.Right , new List<WFCModule>()},
            { WFCUtils.Direction.Left , new List<WFCModule>()},
            { WFCUtils.Direction.Up , new List<WFCModule>()},
            { WFCUtils.Direction.Down , new List<WFCModule>()},
            { WFCUtils.Direction.Forward , new List<WFCModule>()},
            { WFCUtils.Direction.Back , new List<WFCModule>()}
        };


        public bool IsValidNeighbour(WFCModule otherModule, WFCUtils.Direction direction)
        {
            if (otherModule.isEmpty)
            {
                if (direction == WFCUtils.Direction.Right && rightFaceHash == 0) return true;
                if (direction == WFCUtils.Direction.Left && leftFaceHash == 0) return true;
                if (direction == WFCUtils.Direction.Up && upFaceHash == 0) return true;
                if (direction == WFCUtils.Direction.Down && downFaceHash == 0) return true;
                if (direction == WFCUtils.Direction.Forward && forwardFaceHash == 0) return true;
                if (direction == WFCUtils.Direction.Back && backFaceHash == 0) return true;
            }
            
            if (isEmpty)
            {
                if (direction == WFCUtils.Direction.Left && otherModule.rightFaceHash == 0) return true;
                if (direction == WFCUtils.Direction.Right && otherModule.leftFaceHash == 0) return true;
                if (direction == WFCUtils.Direction.Down && otherModule.upFaceHash == 0) return true;
                if (direction == WFCUtils.Direction.Up && otherModule.downFaceHash == 0) return true;
                if (direction == WFCUtils.Direction.Back && otherModule.forwardFaceHash == 0) return true;
                if (direction == WFCUtils.Direction.Forward && otherModule.backFaceHash == 0) return true;
            }
            
            if (direction == WFCUtils.Direction.Right && rightFaceHash == 0) return false;
            if (direction == WFCUtils.Direction.Left && leftFaceHash == 0) return false;
            if (direction == WFCUtils.Direction.Up && upFaceHash == 0) return false;
            if (direction == WFCUtils.Direction.Down && downFaceHash == 0) return false;
            if (direction == WFCUtils.Direction.Forward && forwardFaceHash == 0) return false;
            if (direction == WFCUtils.Direction.Back && backFaceHash == 0) return false;
            
            var mirrorXMesh = WFCUtils.TransformMesh(moduleMesh, Vector3.zero, Quaternion.Euler(0, 0, 0), new Vector3(-1, 1, 1));
            var mirrorXLeftHash = WFCUtils.BoundaryHashForDirection(mirrorXMesh.vertices.ToList(), WFCUtils.Direction.Left, false);
            var mirrorXRightHash = WFCUtils.BoundaryHashForDirection(mirrorXMesh.vertices.ToList(), WFCUtils.Direction.Right, false);
            
            var mirrorZMesh = WFCUtils.TransformMesh(moduleMesh, Vector3.zero, Quaternion.Euler(0, 0, 0), new Vector3(1, 1, -1));
            var mirrorZForwardHash = WFCUtils.BoundaryHashForDirection(mirrorZMesh.vertices.ToList(), WFCUtils.Direction.Forward, false);
            var mirrorZBackHash = WFCUtils.BoundaryHashForDirection(mirrorZMesh.vertices.ToList(), WFCUtils.Direction.Back, false);

            return direction switch
            {
                WFCUtils.Direction.Right => mirrorXLeftHash == otherModule.leftFaceHash,
                WFCUtils.Direction.Left => mirrorXRightHash == otherModule.rightFaceHash,
                WFCUtils.Direction.Forward => mirrorZBackHash == otherModule.backFaceHash,
                WFCUtils.Direction.Back => mirrorZForwardHash == otherModule.forwardFaceHash,
                WFCUtils.Direction.Up => upFaceHash == otherModule.downFaceHash,
                WFCUtils.Direction.Down => downFaceHash == otherModule.upFaceHash,
                _ => false
            };
        }
    }
}