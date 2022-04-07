using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WFCTileset", menuName = "New WFC Tileset")]
public class WFCTileset : ScriptableObject
{
    public List<WFCTile> tiles = new List<WFCTile>();
}
