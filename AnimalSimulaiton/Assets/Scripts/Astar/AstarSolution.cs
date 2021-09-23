using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Simulation.Pathfinding
{
    internal sealed class AstarSolution<T>
    {
        internal readonly struct SearchResult
        {
            internal readonly bool HasFoundPath;
            internal readonly List<PathCell> Path;

            internal SearchResult(bool hasFoundPath, List<PathCell> path)
            {
                HasFoundPath = hasFoundPath;
                Path = path;
            }
        }
        
        private readonly Vector2Int _start;
        private readonly Vector2Int _finish;
        private readonly ObstaclePattern _obstaclePattern;

        private Grid<T> _grid;

        internal AstarSolution(Grid<T> grid, Vector2Int start, Vector2Int finish, ObstaclePattern obstaclePattern)
        {
            _start = start;
            _finish = finish;
            _grid = grid;
            _obstaclePattern = obstaclePattern;
        }

        internal SearchResult TryFindPath()
        {
            var astarGrid = new AstarGrid<T>(_start, _finish, _obstaclePattern, _grid);

            while (astarGrid.HasReachedFinish == false && astarGrid.OpenSet.Count > 0)
                ProcessNextCell(astarGrid);

            var hasSucceeded = astarGrid.TryBuildPath(out var path);
            var result = new SearchResult(hasSucceeded, path);

            return result;
        }

        internal async UniTask<SearchResult> TryFindPathAsync()
        {
            SearchResult result = default;

            await UniTask.Run(() => { result = TryFindPath(); });

            return result;
        }

        private void ProcessNextCell(AstarGrid<T> grid)
        {
            var processingCell = grid.OpenSet.First.Value;

            grid.TryClose(processingCell);
            grid.TryOpenNeighbours(processingCell);
        }
    }
}