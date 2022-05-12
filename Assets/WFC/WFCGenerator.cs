using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WFC
{
    public class WFCGenerator: ScriptableObject
    {
        private readonly Stack<WFCCell> _stack = new();
        public List<WFCCell> cells;
        public WFCModuleSet moduleSet;
        private readonly List<Vector3Int> _dirs = new()
        {
            Vector3Int.forward , 
            Vector3Int.back , 
            Vector3Int.left , 
            Vector3Int.right , 
            Vector3Int.up ,
            Vector3Int.down 
        };

        
        
        private static void Collapse(WFCCell cell)
        {
            var selectedCandidate = cell.candidates[Random.Range(0, cell.candidates.Count)];
            cell.collapsedModule = selectedCandidate;
            cell.candidates = new List<WFCModule>() { selectedCandidate };
            
            if (GameObject.Find("GeneratedLevel") == null)
            {
                var _ =new GameObject("GeneratedLevel");
            }

            //Add the tile to the scene
            var go = cell.collapsedModule.module
                ? Instantiate(cell.collapsedModule.module, cell.transform.position, Quaternion.Euler(0, cell.collapsedModule.rotation, 0),
                    GameObject.Find("GeneratedLevel").transform)
                : new GameObject("Empty")
                {
                    transform =
                    {
                        position = cell.transform.position,
                        rotation = Quaternion.Euler(0, cell.collapsedModule.rotation, 0),
                        parent = GameObject.Find("GeneratedLevel").transform
                    }
                };
            //go.transform.localScale = new Vector3(1, 0.5f, 0.5f);

            Debug.Log("Added " + cell.collapsedModule.moduleName);
        }

        public void Generate()
        {
            var iter = 0;
            while (cells.TrueForAll(x => x.IsCollapsed()) == false && iter < 1000)
            {
                Iterate();
                iter++;
            }
        }

        public void Iterate()
        {
            // find the cell with the lowest number of candidates that isn't collapsed (has more than one candidate)
            var allUnCollapsed = cells.FindAll(c => c.IsCollapsed() == false);
            var candidateCounts = allUnCollapsed.ConvertAll(c => c.candidates.Count);
            var minCandidateCount = candidateCounts.Min();
            var lowestCandidatesCell = allUnCollapsed.Find(c => c.candidates.Count == minCandidateCount);

            // collapse this cell to a single WFCModule
            Collapse(lowestCandidatesCell);
            
            // propagate
            _stack.Push(lowestCandidatesCell);

            while (_stack.Count > 0)
            {
                var currentCell = _stack.Pop();

                foreach (var dir in _dirs)
                {
                    var validNeighbours = currentCell.candidates
                        .ConvertAll(c => WFCUtils.GetValidNeighboursForDirection(moduleSet.modules, c, WFCUtils.VecToDir[dir]))
                        .SelectMany(x => x.ToList())
                        .ToList();
                    
                    // foreach (var c in currentCell.candidates)
                    // {
                    //     var d = WFCUtils.VecToDir[dir];
                    //     var n = WFCUtils.GetValidNeighboursForDirection(currentCell.candidates, c, d);
                    //     
                    // }

                    // get the cell at the direction
                    var cellAtDir = cells.Find(c => c.transform.position == currentCell.transform.position + dir*2);

                    // continue if we can't find the cell at the direction
                    if (cellAtDir == null)
                    {
                        continue;
                    }
                    
                    for (var i = cellAtDir.candidates.Count - 1; i >= 0; i--)
                    {
                        var candidate = cellAtDir.candidates[i];
                        // remove non valid neighbours from the candidates at direction
                        if (validNeighbours.Contains(candidate)) continue;

                        // remove the non-valid neighbour from candidates
                        cellAtDir.candidates.Remove(candidate);

                        // add the cell to the stack if it's not already there
                        if (_stack.Contains(cellAtDir) == false)
                        {
                            _stack.Push(cellAtDir);
                        }
                    }
                }
            }
        }
    }
}
