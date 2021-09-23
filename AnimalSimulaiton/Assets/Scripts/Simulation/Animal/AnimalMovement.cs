using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Simulation.Pathfinding;
using UnityEngine;

namespace Simulation
{
    internal sealed class AnimalMovement : IDisposable
    {
        private readonly Transform _movementRoot;
        private readonly SimulationGrid _grid;
        
        private readonly float _speed;
        private readonly SimulationTransformLinearTweener _tweener;

        private readonly CancellationTokenSource _cancellationToken;

        private List<PathCell> _currentPath;
        private int _pathIndex;

        internal bool IsIdle => _currentPath == null || _pathIndex >= _currentPath.Count;

        internal Action OnFinished;
        internal Action<PathCell> OnDestinationChanged;

        internal AnimalMovement(float speed, Transform movementRoot, SimulationGrid grid, SimulationTime simulationTime)
        {
            _speed = speed;
            _movementRoot = movementRoot;
            _grid = grid;
            _tweener = new SimulationTransformLinearTweener(movementRoot, simulationTime);
            _cancellationToken = new CancellationTokenSource();
        }

        internal void SetPath(List<PathCell> path)
        {
            _currentPath = path;
            _pathIndex = 1;
        }

        internal async UniTask MoveToNext()
        {
            if (IsIdle)
                return;

            var pathCell = _currentPath[_pathIndex++];

            OnDestinationChanged?.Invoke(pathCell);

            await _tweener.MoveTo(_grid.GridToWorldPosition(pathCell.Position), 1f / _speed);

            if (_cancellationToken.IsCancellationRequested)
                return;

            if(_pathIndex == _currentPath.Count)
                OnFinished?.Invoke();
        }
        
        public void Dispose()
        {
            _cancellationToken.Cancel();
            _tweener?.Dispose();
        }
    }
}