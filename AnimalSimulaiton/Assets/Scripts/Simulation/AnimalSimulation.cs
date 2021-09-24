using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Simulation.Extensions;
using UnityEngine;

namespace Simulation
{
    internal sealed partial class AnimalSimulation : MonoBehaviour, IUniTaskAsyncDisposable
    {
        [SerializeField] private Animal _animalPrefab = default;
        [SerializeField] private Fruit _fruitPrefab = default;
        [SerializeField] private ParticleSystem _particlesPrefab = default;
        
        private readonly List<AnimalFruitHandler> _animalFruitHandlers = new List<AnimalFruitHandler>();

        private int _gridSize;
        private int _animalsAmount;
        private int _animalSpeed;
        
        private SimulationTime _time;
        private SimulationGrid _simulationGrid;
        private SimulationGridObjectFactory _simulationGridObjectFactory;
        private FruitParticlePool _particles;

        private CancellationTokenSource _cancellationTokenSource;
        private bool _isRunning;

        internal SimulationTime Time => _time;
        internal bool IsRunning => _isRunning;

        internal void InitializeSimulation(int gridSize, int animalsAmount, int animalSpeed)
        {
            InitializeSimulationAsync(gridSize, animalsAmount, animalSpeed).Forget();
        }
        
        private async UniTask InitializeSimulationAsync(int gridSize, int animalsAmount, int animalSpeed)
        {
            if (_isRunning)
                await StopSimulation();
            
            _isRunning = true;
            _cancellationTokenSource = new CancellationTokenSource();
            
            _gridSize = Mathf.Clamp(gridSize, 2, 1000);
            _animalsAmount = Mathf.Clamp(animalsAmount, 1, gridSize * gridSize / 2);
            _animalSpeed = Mathf.Clamp(animalSpeed, 1, 100);

            SetState(ESimulationState.Computing);

            InitializeTime();
            InitializeGrid(_gridSize);
            InitializeParticles();
            InitializeFactory(_simulationGrid);
            await SpawnAnimals(_animalsAmount);
            
            RunSimulation().Forget();
        }

        private async UniTask RunSimulation()
        {
            while (_cancellationTokenSource.IsCancellationRequested == false)
            {
                _time.Increment();
                
                SetState(ESimulationState.Computing);

                await ConstructMovementForIdle();

                if (_cancellationTokenSource.IsCancellationRequested)
                    break;

                SetState(ESimulationState.Running);

                await MoveAnimals();
            }
        }
        
        private async UniTask ConstructMovementForIdle()
        {
            foreach (var animalFruitHandler in _animalFruitHandlers)
            {
                if (animalFruitHandler.Animal.IsIdle)
                    await animalFruitHandler.ConstructPath();
            }
        }

        private UniTask MoveAnimals()
        {
            var simulation = new List<UniTask>();
                
            foreach (var animalFruitPair in _animalFruitHandlers)
                simulation.Add(animalFruitPair.MoveNext());

            return UniTask.WhenAll(simulation).AttachExternalCancellation(_cancellationTokenSource.Token);
        }

        private void InitializeParticles()
        {
            _particles = new FruitParticlePool(_particlesPrefab, _simulationGrid.GetLocalScaleForObject() * _simulationGrid.Root.localScale.x, transform);
        }

        private void InitializeGrid(int size)
        {
            _simulationGrid = new SimulationGrid(transform, size);
            _time.OnTick += _simulationGrid.DecreaseObstacleValues;
        }

        private void InitializeTime()
        {
            _time = new SimulationTime();
        }

        private void InitializeFactory(SimulationGrid simulationGrid)
        {
            _simulationGridObjectFactory = new SimulationGridObjectFactory(simulationGrid);
        }

        private async UniTask SpawnAnimals(int amount)
        {
            SetState(ESimulationState.Spawning);
            
            var spawnSpace = _simulationGrid.GetFreeSpace();

            for (var i = 0; i < amount; i++)
            {
                var handler = await SpawnAnimalFruitRandom(_animalPrefab, _fruitPrefab, spawnSpace);

                _animalFruitHandlers.Add(handler);

                if (_cancellationTokenSource.IsCancellationRequested)
                    break;
                
#if UNITY_EDITOR
                if (Application.isPlaying == false)
                    break;
#endif
            }
            
            SetState(ESimulationState.Running);
        }

        private async UniTask<AnimalFruitHandler> SpawnAnimalFruitRandom(Animal animalPrefab, Fruit fruitPrefab, List<Vector2Int> freeSpace)
        {
            var animalSpawnPoint = freeSpace.PickRandom();
            var animal = SpawnAnimalRandom(animalPrefab, animalSpawnPoint);
            var fruit = SpawnFruitRandom(fruitPrefab, animal.Position);
            var animalFruitHandler = new AnimalFruitHandler(animal, fruit, _simulationGrid, _animalSpeed);

            var firstRemoval = UniTask.Run(() =>
            {
                freeSpace.Remove(animal.Position);
            });
            var secondRemoval = UniTask.Run(() =>
            {
                freeSpace.Remove(fruit.Position);
            });

            await UniTask.WhenAll(firstRemoval, secondRemoval);

            return animalFruitHandler;
        }

        private Animal SpawnAnimalRandom(Animal prefab, Vector2Int spawnPoint)
        {
            var animal = _simulationGridObjectFactory.Spawn(prefab, spawnPoint);

            animal.Initialize(_animalSpeed, _time);
            
            return animal;
        }

        private Fruit SpawnFruitRandom(Fruit prefab, Vector2Int spawnCenter)
        {
            var fruit = _simulationGridObjectFactory.Spawn(prefab, spawnCenter);

            fruit.Initialize(_particles);
            fruit.TryMoveRandomly(spawnCenter, _animalSpeed * 5);

            return fruit;
        }
        
        private async UniTask StopSimulation()
        {
            if (_isRunning == false) 
                return;

            _isRunning = false;
            _time.OnTick -= _simulationGrid.DecreaseObstacleValues;

            _cancellationTokenSource.Cancel();

            if (State == ESimulationState.Spawning)
                await UniTask.WaitUntil(() => State != ESimulationState.Spawning);
            
            DestroyAnimals();
        }

        private void DestroyAnimals()
        {
            foreach (var animalFruitPair in _animalFruitHandlers)
            {
                var fruit = animalFruitPair.Fruit;
                var animal = animalFruitPair.Animal;

                animalFruitPair.Dispose();

                if (fruit != null)
                    Destroy(fruit.gameObject);

                if (animal != null)
                    Destroy(animal.gameObject);
            }

            _animalFruitHandlers.Clear();
        }
        
        public async UniTask DisposeAsync()
        {
            await StopSimulation();
            await _particles.DisposeAsync();
            
            _cancellationTokenSource?.Dispose();
        }
    }
}