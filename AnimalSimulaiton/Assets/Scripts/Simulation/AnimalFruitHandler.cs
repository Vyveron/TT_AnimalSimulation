using System;
using Cysharp.Threading.Tasks;
using Simulation.Pathfinding;
using UnityEngine;

namespace Simulation
{
    internal sealed class AnimalFruitHandler : IDisposable
    {
        internal readonly Animal Animal;
        internal readonly Fruit Fruit;

        private readonly SimulationGrid _simulationGrid;
        private readonly int _animalSpeed;
        
        private readonly ObstaclePattern _obstaclePattern;

        internal AnimalFruitHandler(Animal animal, Fruit fruit, SimulationGrid simulationGrid, int animalSpeed)
        {
            Animal = animal;
            Fruit = fruit;
            _simulationGrid = simulationGrid;
            _animalSpeed = animalSpeed;
            _obstaclePattern = new PathObstaclePattern(_simulationGrid);
            
            MoveFruitRandomly();

            animal.Movement.OnFinished += HandleFinish;
        }

        internal async UniTask MoveNext()
        {
            await Animal.Next();
        }

        internal async UniTask<bool> ConstructPath()
        {
            if(ShouldMoveFruit())
                MoveFruitRandomly();

            if (ShouldMoveFruit())
            {
                _simulationGrid.Occupy(Animal.Position);

                return false;
            }
            
            var result = await ConstructPath(Fruit.Position);

            return result;
        }

        private void HandleFinish()
        {
            Fruit.PlayParticles();
            MoveFruitRandomly();
        }

        private void MoveFruitRandomly()
        {
            Fruit.TryMoveRandomly(Animal.Position, _animalSpeed * 5);
        }

        private async UniTask<bool> ConstructPath(Vector2Int destination)
        {
            var pathfindingSolution = new AstarSolution<SimulationGridCell>(_simulationGrid, Animal.Position, destination, _obstaclePattern);
            var searchResult = await pathfindingSolution.TryFindPathAsync();

            if (searchResult.HasFoundPath == false)
            {
                _simulationGrid.Occupy(Animal.Position);
                
                return false;
            }

            var path = searchResult.Path;
            
            Animal.Movement.SetPath(path);
            _simulationGrid.SetPathObstacle(path);
            
            return true;
        }

        private bool ShouldMoveFruit()
        {
            return Animal.Position == Fruit.Position;
        }
        
        public void Dispose()
        {
            Animal.Movement.OnFinished -= Fruit.PlayParticles;
            
            if(Animal != null)
                Animal.Dispose();
        }
    }
}