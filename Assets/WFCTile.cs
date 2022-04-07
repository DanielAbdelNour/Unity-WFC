using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WFCTile : ScriptableObject
{
    public GameObject tileGameObject;
    public string tileId;
    public bool isEmpty;
    public int nNeightbours = 0;

    public List<WFCTile> forwardNeighbors = new List<WFCTile>();
    public List<WFCTile> backNeighbors = new List<WFCTile>();
    public List<WFCTile> leftNeighbors = new List<WFCTile>();
    public List<WFCTile> rightNeighbors = new List<WFCTile>();
    public List<WFCTile> upNeighbors = new List<WFCTile>();
    public List<WFCTile> downNeighbors = new List<WFCTile>();


    public void SetNNeighbours()
    {
        // add this tile to the tileset
        nNeightbours = forwardNeighbors.Count + backNeighbors.Count + leftNeighbors.Count + rightNeighbors.Count + upNeighbors.Count + downNeighbors.Count;
    }

    public List<WFCTile> GetValidNeighboursForDirection(Vector3Int dir)
    {
        if (dir == Vector3Int.forward)
        {
            return forwardNeighbors;
        }
        else if (dir == Vector3Int.back)
        {
            return backNeighbors;
        }
        else if (dir == Vector3Int.left)
        {
            return leftNeighbors;
        }
        else if (dir == Vector3Int.right)
        {
            return rightNeighbors;
        }
        else if (dir == Vector3Int.up)
        {
            return upNeighbors;
        }
        else if (dir == Vector3Int.down)
        {
            return downNeighbors;
        }
        else
        {
            return new List<WFCTile>();
        }
    }
}
