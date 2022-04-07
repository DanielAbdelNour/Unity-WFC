using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;

public class WFCTileGenerator : MonoBehaviour
{
    public GameObject tileSet;

    int GenerateTileHash(Transform tile)
    {
        MeshFilter tileMeshFilter = tile.GetComponent<MeshFilter>();
        if (tileMeshFilter == null) return -1;
        List<Vector3> tileVerts = new List<Vector3>(tile.GetComponent<MeshFilter>().sharedMesh.vertices);
        string tileHash = "";
        foreach (Vector3 v in tileVerts)
        {
            tileHash += v.ToString();
        }
        return tileHash.GetHashCode();
    }

    [Button("Generate Tile Assets")]
    void GenerateTileMetadataAsset()
    {
        WFCTileset tileset = WFCTileset.CreateInstance<WFCTileset>();

        // detete the existing Tiles folder
        if (AssetDatabase.IsValidFolder("Assets/Tiles"))
        {
            AssetDatabase.DeleteAsset("Assets/Tiles");
        }
        // create a new folder for the tiles
        AssetDatabase.CreateFolder("Assets", "Tiles");

        // store each unique tile as a hash of it's vertices and the corrosponding gameobject
        Dictionary<int, GameObject> uniqueTiles = new Dictionary<int, GameObject>();
        List<int> tileHashes = new List<int>();
        // loop trough all tiles, hash them, then add them to a dictionary if they are unique
        // add boxcollider for raycasting
        for (var i = 0; i < tileSet.transform.childCount; i++)
        {
            Transform tile = tileSet.transform.GetChild(i);
            if (tile.gameObject.GetComponent<BoxCollider>() == null)
            {
                BoxCollider bc = tile.gameObject.AddComponent<BoxCollider>();
                bc.center = new Vector3(0, 0, 0);
                bc.size = new Vector3(2, 2, 2);
            }

            int tileHash = GenerateTileHash(tile);
            //if (tileHash == 0) continue;
            if (!tileHashes.Contains(tileHash))
            {
                tileHashes.Add(tileHash);
                uniqueTiles.Add(tileHash, tile.gameObject);
            }
        }

        // create a new asset for every unique tile
        foreach (GameObject tile in uniqueTiles.Values)
        {
            WFCTile wfcTile = WFCTile.CreateInstance<WFCTile>();
            wfcTile.tileId = tile.name;
            wfcTile.tileGameObject = tile.gameObject;
            if(tile.GetComponent<MeshFilter>() == null){
                wfcTile.isEmpty = true;
            }
            AssetDatabase.CreateAsset(wfcTile, "Assets/Tiles/" + wfcTile.tileId + ".asset");
        }

        // go through every mesh in the tileSet and create and 
        Transform[] tiles = tileSet.GetComponentsInChildren<Transform>();
        foreach (Transform tile in tiles)
        {
            // grab the collider and skip if it's null
            BoxCollider tileCollider = tile.GetComponent<BoxCollider>();

            // ensure the tile has a collider for raycasting to wrok
            if (tileCollider == null)
            {
                Debug.Log("Skipping tile " + tile.name + " because it has no collider");
                continue;
            }

            // hash the tile
            int tileHash = GenerateTileHash(tile);
            GameObject uniqueTile = uniqueTiles[tileHash];

            // get the unique tile from assets
            WFCTile wfcTile = AssetDatabase.LoadAssetAtPath<WFCTile>("Assets/Tiles/" + uniqueTile.name + ".asset");

            // get first transform in positive x direction
            RaycastHit hit;
            // forward
            if (Physics.Raycast(tile.transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity))
            {
                GameObject hashedNeighbourTile = uniqueTiles[GenerateTileHash(hit.transform)];
                WFCTile neighbourTile = AssetDatabase.LoadAssetAtPath<WFCTile>("Assets/Tiles/" + hashedNeighbourTile.name + ".asset");
                if (!wfcTile.forwardNeighbors.Contains(neighbourTile))
                {
                    wfcTile.forwardNeighbors.Add(neighbourTile);
                }
            }
            // back
            if (Physics.Raycast(tile.transform.position, transform.TransformDirection(Vector3.back), out hit, Mathf.Infinity))
            {
                GameObject hashedNeighbourTile = uniqueTiles[GenerateTileHash(hit.transform)];
                WFCTile neighbourTile = AssetDatabase.LoadAssetAtPath<WFCTile>("Assets/Tiles/" + hashedNeighbourTile.name + ".asset");
                if (!wfcTile.backNeighbors.Contains(neighbourTile))
                {
                    wfcTile.backNeighbors.Add(neighbourTile);
                }
            }
            // left
            if (Physics.Raycast(tile.transform.position, transform.TransformDirection(Vector3.left), out hit, Mathf.Infinity))
            {
                GameObject hashedNeighbourTile = uniqueTiles[GenerateTileHash(hit.transform)];
                WFCTile neighbourTile = AssetDatabase.LoadAssetAtPath<WFCTile>("Assets/Tiles/" + hashedNeighbourTile.name + ".asset");
                if (!wfcTile.leftNeighbors.Contains(neighbourTile))
                {
                    wfcTile.leftNeighbors.Add(neighbourTile);
                }
            }
            // right
            if (Physics.Raycast(tile.transform.position, transform.TransformDirection(Vector3.right), out hit, Mathf.Infinity))
            {
                GameObject hashedNeighbourTile = uniqueTiles[GenerateTileHash(hit.transform)];
                WFCTile neighbourTile = AssetDatabase.LoadAssetAtPath<WFCTile>("Assets/Tiles/" + hashedNeighbourTile.name + ".asset");
                if (!wfcTile.rightNeighbors.Contains(neighbourTile))
                {
                    wfcTile.rightNeighbors.Add(neighbourTile);
                }
            }

            // update the tile's neighbour list and add to tilest
            // wfcTile.SetNNeighbours();
            if (!tileset.tiles.Contains(wfcTile))
            {
                tileset.tiles.Add(wfcTile);
            }
        }

        AssetDatabase.CreateAsset(tileset, "Assets/Tiles/" + "WFCTileset" + ".asset");
    }

    void OnDrawGizmos()
    {

    }
}


// List<GameObject> uniqueTiles = new List<GameObject>();

// foreach (Transform tile in tileSet.GetComponentsInChildren<Transform>())
// {
//     MeshFilter tileMeshFilter = tile.GetComponent<MeshFilter>();
//     if (tileMeshFilter == null) continue;
//     List<Vector3> tileVerts = new List<Vector3>(tile.GetComponent<MeshFilter>().sharedMesh.vertices);

//     if (uniqueTiles.Count == 0)
//     {
//         uniqueTiles.Add(tile.gameObject);
//     }

//     bool allUnique = true;
//     foreach (GameObject uniqueTile in uniqueTiles)
//     {
//         MeshFilter uniqueTileMeshFilter = uniqueTile.GetComponent<MeshFilter>();
//         List<Vector3> uniqueTileVerts = new List<Vector3>(uniqueTileMeshFilter.sharedMesh.vertices);

//         bool[] equalVerts = new bool[uniqueTileVerts.Count];

//         if (uniqueTileVerts.Count == tileVerts.Count)
//         {
//             for (var i = 0; i < uniqueTileVerts.Count; i++)
//             {
//                 if (uniqueTileVerts[i].Equals(tileVerts[i]))
//                 {
//                     equalVerts[i] = true;
//                 }
//                 else
//                 {
//                     equalVerts[i] = false;
//                 }
//             }
//         }
//         List<bool> equalVertsList = new List<bool>(equalVerts);

//         if (!equalVertsList.Contains(false))
//         {
//             allUnique = false;
//             break;
//         }
//     }
//     if (allUnique) uniqueTiles.Add(tile.gameObject);
// }