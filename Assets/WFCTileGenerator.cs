using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;

public class WFCTileGenerator : MonoBehaviour
{
    public GameObject tilePrefabContainer;

    Dictionary<string, Vector3Int> directions = new Dictionary<string, Vector3Int>(){
                {"forward", Vector3Int.forward},
                {"back", Vector3Int.back},
                {"left", Vector3Int.left},
                {"right", Vector3Int.right},
                {"up", Vector3Int.up},
                {"down", Vector3Int.down},
            };

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

        // int output = 0;
        // foreach(Vector3 vertex in tileVerts){
        //     output = output ^ vertex.GetHashCode();
        // }
        // return output;
    }

    [Button("Generate Tile Assets")]
    void GenerateTileMetadataAsset()
    {
        // remove generatedTilePrefabs gameobject if it exists
        if (GameObject.Find("generatedTilePrefabs") != null)
        {
            DestroyImmediate(GameObject.Find("generatedTilePrefabs"));
        }
        GameObject generatedTilePrefabs = new GameObject("generatedTilePrefabs");

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

        // loop through each tile in the container and:
        // 1. instantiate a gameobject for the tile to the scene
        // 2. add a boxcollider if it doesn't already have one
        // 3. generate a hash of the tile's vertices
        // 4. add the tile to the uniquetiles list if it's hash is unique
        for (var i = 0; i < tilePrefabContainer.transform.childCount; i++)
        {
            Transform tile = Instantiate(tilePrefabContainer.transform.GetChild(i));
            tile.parent = generatedTilePrefabs.transform;

            if (tile.gameObject.GetComponent<BoxCollider>() == null)
            {
                BoxCollider bc = tile.gameObject.AddComponent<BoxCollider>();
                bc.center = new Vector3(0, 0, 0);
                bc.size = new Vector3(2, 2, 2);
            }

            int tileHash = GenerateTileHash(tile);
            if (!tileHashes.Contains(tileHash))
            {
                tileHashes.Add(tileHash);
                uniqueTiles.Add(tileHash, tile.gameObject);
            }
        }

        // generate empty tile prefabs
        //GameObject emptyTiles = new GameObject("EmptyTiles");
        //foreach (Transform tile in generatedTilePrefabs.transform)
        int initialTileCount = generatedTilePrefabs.transform.childCount;
        for(int i = 0; i < initialTileCount; i++)
        {
            Transform tile = generatedTilePrefabs.transform.GetChild(i);
            foreach (string directionString in directions.Keys)
            {
                directions.TryGetValue(directionString, out Vector3Int dir);
                RaycastHit hit;
                if (Physics.Raycast(tile.transform.position, dir, out hit, 2f) == false)
                {
                    GameObject emptyTile = new GameObject("Empty");
                    emptyTile.transform.parent = generatedTilePrefabs.transform;
                    emptyTile.transform.position = tile.transform.position + dir*2;
                    emptyTile.AddComponent<BoxCollider>();
                    emptyTile.GetComponent<BoxCollider>().center = new Vector3(0, 0, 0);
                    emptyTile.GetComponent<BoxCollider>().size = new Vector3(2, 2, 2);

                    int tileHash = GenerateTileHash(emptyTile.transform);
                    if (!tileHashes.Contains(tileHash))
                    {
                        tileHashes.Add(tileHash);
                        uniqueTiles.Add(tileHash, emptyTile);
                    }
                }
            }
        }


        // create a new asset for every unique tile
        foreach (GameObject tile in uniqueTiles.Values)
        {
            WFCTile wfcTile = WFCTile.CreateInstance<WFCTile>();
            wfcTile.tileId = tile.name;
            wfcTile.tileGameObject = tile.gameObject;
            if (tile.GetComponent<MeshFilter>() == null)
            {
                wfcTile.isEmpty = true;
            }
            AssetDatabase.CreateAsset(wfcTile, "Assets/Tiles/" + wfcTile.tileId + ".asset");
        }

        // go through every mesh in the tileSet and create and 
        Transform[] tiles = generatedTilePrefabs.GetComponentsInChildren<Transform>();
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

            foreach (string directionString in directions.Keys)
            {
                directions.TryGetValue(directionString, out Vector3Int dir);
                RaycastHit hit;

                if (Physics.Raycast(tile.transform.position, dir, out hit, 2f))
                {
                    GameObject hashedNeighbourTile = uniqueTiles[GenerateTileHash(hit.transform)];
                    WFCTile neighbourTile = AssetDatabase.LoadAssetAtPath<WFCTile>("Assets/Tiles/" + hashedNeighbourTile.name + ".asset");
                    List<WFCTile> neighbours = (List<WFCTile>)typeof(WFCTile).GetField(directionString + "Neighbors").GetValue(wfcTile);
                    if (!neighbours.Contains(neighbourTile))
                    {
                        neighbours.Add(neighbourTile);
                    }
                }
                // else
                // {
                //     // there is no neighbour in that direction, so add a empty neighbour
                //     GameObject hashedEmptyNeighbourTile = uniqueTiles[-1];
                //     WFCTile emptyNeighbourTile = AssetDatabase.LoadAssetAtPath<WFCTile>("Assets/Tiles/" + hashedEmptyNeighbourTile.name + ".asset");
                //     List<WFCTile> neighbours = (List<WFCTile>)typeof(WFCTile).GetField(directionString + "Neighbors").GetValue(wfcTile);
                //     // add the empty neighbour if the list is empty (it should only ever contain empty)
                //     if (neighbours.Count == 0)
                //     {
                //         neighbours.Add(emptyNeighbourTile);
                //     }
                //     // add this tile to empty's possible neighbours at direction if it doesn't already exist
                //     List<WFCTile> emptyNeighbours = (List<WFCTile>)typeof(WFCTile).GetField(directionString + "Neighbors").GetValue(emptyNeighbourTile);
                //     if (!emptyNeighbours.Contains(wfcTile) && !wfcTile.isEmpty)
                //     {
                //         emptyNeighbours.Add(wfcTile);
                //     }
                // }
            }

            // set the number of neighbours
            wfcTile.SetNNeighbours();

            // add the tile to the tilset asset
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
