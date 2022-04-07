using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;

public class WFCCell
{
    public Vector3Int position;
    public List<WFCTile> candidates = new List<WFCTile>();
    public WFCTile selectedCandidate;
    public bool collapsed = false;

    public WFCCell(Vector3Int position, List<WFCTile> candidates)
    {
        this.position = position;
        this.candidates = candidates;
    }
}

public class LevelGenerator : MonoBehaviour
{
    public int MAX_X = 10;
    public int MAX_Y = 0;
    public int MAX_Z = 10;
    public WFCTileset tileset;
    private List<WFCCell> cells;
    private Stack<WFCCell> stack;
    List<Vector3Int> dirs = new List<Vector3Int>() { Vector3Int.forward, Vector3Int.back, Vector3Int.left, Vector3Int.right, Vector3Int.up, Vector3Int.down };

    bool IsCollapsed()
    {
        return cells.TrueForAll(c => c.collapsed == true); ;
    }

    void Collapse(WFCCell cell)
    {
        WFCTile selectedCandidate = cell.candidates[Random.Range(0, cell.candidates.Count)];
        cell.selectedCandidate = selectedCandidate;
        cell.candidates = new List<WFCTile>();
        cell.collapsed = true;
    }

    void Iterate()
    {
        // find the cell with the lowest number of candidates that isn't collapsed (has more than one candidate)
        List<WFCCell> allUnCollapsed = cells.FindAll(c => c.collapsed == false);

        int minCandidateCount = allUnCollapsed.ConvertAll(c => c.candidates.Count).Min();
        WFCCell lowestCandidatesCell = allUnCollapsed.Find(c => c.candidates.Count == minCandidateCount);

        // collapse this cell to a single wfctile
        Collapse(lowestCandidatesCell);

        // propagate
        stack.Push(lowestCandidatesCell);

        while (stack.Count > 0)
        {
            WFCCell currentCell = stack.Pop();

            foreach (Vector3Int dir in dirs)
            {

                // get the valid neighbours for this direction for the candidate (item 0 of the candidates list because it's already collapsed)
                List<WFCTile> validNeighbours;
                if (currentCell.selectedCandidate != null)
                {
                    validNeighbours = currentCell.selectedCandidate.GetValidNeighboursForDirection(dir);
                }
                else
                {
                    validNeighbours = new HashSet<WFCTile>(currentCell.candidates.ConvertAll(c =>
                    {
                        return c.GetValidNeighboursForDirection(dir);
                    }).SelectMany(x => x).ToList()).ToList();
                }

                // get the cell at the direction
                WFCCell cellAtDir = cells.Find(c => c.position == currentCell.position + dir);

                // continue if we can't find the cell at the direction
                if (cellAtDir == null)
                {
                    continue;
                }

                for (int i = cellAtDir.candidates.Count-1; i >= 0; i--)
                {
                    WFCTile candidate = cellAtDir.candidates[i];
                    // remove non valid neighbours from the candidates at direction
                    if (validNeighbours.Contains(candidate) == false)
                    {
                        // remove the nonvalid neighbour from candidates
                        cellAtDir.candidates.Remove(candidate);

                        // add the cell to the stack if it's not already there
                        if (stack.Contains(cellAtDir) == false)
                        {
                            stack.Push(cellAtDir);
                        }
                    }
                }
            }
        }
    }



    [Button("Generate Level")]
    void GenerateLevel()
    {
        stack = new Stack<WFCCell>();
        cells = new List<WFCCell>();

        //List<WFCTile> tiles = new List<WFCTile>(tileset.tiles);

        // create a 3d list of cells and give each cell the complete list of candidates
        for (int x = 0; x < MAX_X; x++)
        {
            for (int y = 0; y < MAX_Y; y++)
            {
                for (int z = 0; z < MAX_Z; z++)
                {
                    WFCCell cell = new WFCCell(new Vector3Int(x, y, z), new List<WFCTile>(tileset.tiles));
                    cells.Add(cell);
                }
            }
        }

        // while there are still candidates iterate the WFC function
        int iter = 0;
        while (IsCollapsed() == false && iter < 40000)
        {
            Iterate();
            iter++;
        }

        foreach (WFCCell cell in cells)
        {
            GameObject cellGameObject = cell.selectedCandidate.tileGameObject;
            if (cellGameObject != null)
            {
                GameObject go = Instantiate(cellGameObject, cell.position, Quaternion.Euler(-90, 0, 0), transform);
                go.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            }
        }

    }
}
