using System.Collections.Generic;
using System.Linq;
using Simulation.Extensions;
using UnityEngine;

namespace Simulation.Pathfinding
{
    internal sealed class AstarGrid<T>
    {
        private readonly Vector2Int _start;
        private readonly Vector2Int _finish;

        private readonly Grid<T> _grid;
        private readonly ObstaclePattern _obstaclePattern;
        
        private AstarCell _finishCell;
        
        internal bool HasReachedFinish { get; private set; }
        
        internal SortedLinkedList<AstarCell> OpenSet { get; }
        private HashSet<Vector2Int> OpenSetCoords { get; }
        private HashSet<Vector2Int> ClosedSetCoords { get; }
        
        private static bool SetSort(AstarCell first, AstarCell second)
        {
            return first.G > second.G;
        }

        internal AstarGrid(Vector2Int start, Vector2Int finish, ObstaclePattern obstaclePattern, Grid<T> grid)
        {
            _start = start;
            _finish = finish;
            _grid = grid;
            _obstaclePattern = obstaclePattern;
            
            OpenSet = new SortedLinkedList<AstarCell>(SetSort);
            OpenSetCoords = new HashSet<Vector2Int>();
            ClosedSetCoords = new HashSet<Vector2Int>();
            
            InitializeStartCell();
        }

        private void InitializeStartCell()
        {
            var h = GetHValue(_start);
            var startCell = new AstarCell(_start, 0f, h, null);

            OpenSet.Add(startCell);
        }

        internal bool TryBuildPath(out List<PathCell> path)
        {
            if (HasReachedFinish == false)
            {
                path = default;
                return false;
            }

            var linkedPath = new LinkedList<PathCell>();
            var currentCell = _finishCell;

            while (currentCell != null)
            {
                var origin = currentCell.Origin;
                var pathPosition = currentCell.Position;
                
                linkedPath.AddFirst(new PathCell(pathPosition, currentCell.ArrivingTime));

                currentCell = origin;
            }

            path = linkedPath.ToList();

            return true;
        }

        internal void TryOpenNeighbours(AstarCell cell)
        {
            var coordinates = cell.Position;
            
            TryOpen(coordinates + Vector2Int.left, cell);
            TryOpen(coordinates + Vector2Int.right, cell);
            TryOpen(coordinates + Vector2Int.up, cell);
            TryOpen(coordinates + Vector2Int.down, cell);
        }

        internal void TryOpen(Vector2Int coordinates, AstarCell origin)
        {
            if (coordinates.x < 0 || coordinates.x >= _grid.Size ||
                coordinates.y < 0 || coordinates.y >= _grid.Size)
                return;

            if (IsOpen(coordinates) || 
                IsClosed(coordinates))
                return;

            var f = origin.F + 1f;
            var h = GetHValue(coordinates);
            var cell = new AstarCell(coordinates, f, h, origin);
            var isObstacle = _obstaclePattern.IsObstacle(cell);

            if (isObstacle)
                return;

            OpenSet.Add(cell);
            OpenSetCoords.Add(cell.Position);
        }

        internal void TryClose(AstarCell cell)
        {
            if (cell.Position == _finish)
            {
                HasReachedFinish = true;
                _finishCell = cell;
                return;
            }

            if (OpenSet.Contains(cell) == false)
                return;

            OpenSet.Remove(cell);
            OpenSetCoords.Remove(cell.Position); 
            ClosedSetCoords.Add(cell.Position);
        }

        private bool IsClosed(Vector2Int position)
        {
            return ClosedSetCoords.Contains(position);
        }

        private bool IsOpen(Vector2Int position)
        {
            return OpenSetCoords.Contains(position);
        }

        private float GetHValue(Vector2Int position)
        {
            var direction = _finish - position;
            var h = Mathf.Sqrt(direction.x.Square() + direction.y.Square());

            return h;
        }
    }
}