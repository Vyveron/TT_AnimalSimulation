using UnityEngine;

namespace Simulation
{
    internal abstract class SimulationGridObject : MonoBehaviour, IGridObject
    {
        [SerializeField] private Transform _root = default;

        internal Transform Root => _root;
        
        protected SimulationGrid Grid { get; private set; }
        
        public Vector2Int Position { get; private set; }
        
        public void SetPosition(Vector2Int position)
        {
            SetPositionWithoutRefresh(position);
            RefreshPosition();
        }

        internal void InitializeGrid(SimulationGrid grid, Vector2Int position)
        {
            Grid = grid;
            Position = position;
            
            RefreshPosition();
        }

        protected void SetPositionWithoutRefresh(Vector2Int position)
        {
            Position = position;
        }

        protected void RefreshPosition()
        {
            _root.position = Grid.GridToWorldPosition(Position);
        }
    }
}