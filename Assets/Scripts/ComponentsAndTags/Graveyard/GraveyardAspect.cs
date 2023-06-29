using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace ComponentsAndTags
{
    public readonly partial struct GraveyardAspect : IAspect
    {
        public readonly Entity Entity;

        //private readonly TransformAspect _transformAspect;
        
        private readonly RefRO<LocalTransform> _transform;

        private readonly RefRO<GraveyardProperties> _graveyardProperties;
        private readonly RefRW<GraveyardRandom> _graveyardRandom;
        private readonly RefRW<ZombieSpawnPoints> _zombieSpawnPoints;
        private readonly RefRW<ZombieSpawnTimer> _zombieSpawnTimer;

        public int NumberTombstonesToSpawn => _graveyardProperties.ValueRO.NumberTombstonesToSpawn;
        public Entity TombstonePrefab => _graveyardProperties.ValueRO.TombstonePrefab;
        public Entity ZombiePrefab => _graveyardProperties.ValueRO.ZombiePrefab;
        public float ZombieSpawnRate => _graveyardProperties.ValueRO.ZombieSpawnRate;
        
        public bool ZombieSpawnPointInitialized()
        {
            return _zombieSpawnPoints.ValueRO.Value.IsCreated && ZombieSpawnPointCount > 0;
        }

        private int ZombieSpawnPointCount => _zombieSpawnPoints.ValueRO.Value.Value.Value.Length;
        
        public LocalTransform GetRandomTombstoneTransform()
        {
            return new LocalTransform ()
            {
                Position = GetRandomPosition(),
                Rotation = GetRandomRotation(),
                Scale = GetRandomScale(0.5f)
            };
        }

        private float3 GetRandomPosition()
        {
            float3 randomPosition;

            do
            {
                randomPosition = _graveyardRandom.ValueRW.Value.NextFloat3(MinCorner, MaxCorner);    
            } while (math.distancesq(Position, randomPosition) < BRAIN_SAFETY_RADIUS_SQ);

            return randomPosition;
        }

        private quaternion GetRandomRotation() =>
            quaternion.RotateY(_graveyardRandom.ValueRW.Value.NextFloat(-0.25f, 0.25f));

        private float GetRandomScale(float min) => _graveyardRandom.ValueRW.Value.NextFloat(min, 1f);

        private float3 MinCorner => Position - HalfDimensions;
        private float3 MaxCorner => Position + HalfDimensions;

        private float3 HalfDimensions => new float3(
            _graveyardProperties.ValueRO.FieldDimensions.x * 0.5f,
            0,
            _graveyardProperties.ValueRO.FieldDimensions.y * 0.5f);

        private const float BRAIN_SAFETY_RADIUS_SQ = 100;

        public float ZombieSpawnTimer
        {
            get => _zombieSpawnTimer.ValueRO.Value;
            set => _zombieSpawnTimer.ValueRW.Value = value;
        }

        public bool TimeToSpawnZombie => ZombieSpawnTimer <= 0;

        public float3 Position => _transform.ValueRO.Position;

        public LocalTransform GetZombieSpawnPoint()
        {
            var position = GetRandomZombieSpawnPoint();

            return new LocalTransform 
            {
                Position = position,
                Rotation = quaternion.RotateY(MathHelpers.GetHeading(position, Position)),
                Scale = 1f
            };
        }

        private float3 GetRandomZombieSpawnPoint()
        {
            return GetZombieSpawnPoint(_graveyardRandom.ValueRW.Value.NextInt(ZombieSpawnPointCount));
        }
        
        private float3 GetZombieSpawnPoint(int i) => _zombieSpawnPoints.ValueRO.Value.Value.Value[i];
    }
}