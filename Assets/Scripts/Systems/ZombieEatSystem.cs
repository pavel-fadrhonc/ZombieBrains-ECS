﻿using ComponentsAndTags;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems
{
    [BurstCompile]
    [UpdateAfter(typeof(ZombieWalkSystem))]
    public partial struct ZombieEatSystem : ISystem 
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var dt = SystemAPI.Time.DeltaTime;
            var brainEntity = SystemAPI.GetSingletonEntity<BrainTag>();
            var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            var brainScale = SystemAPI.GetComponent<LocalTransform>(brainEntity).Scale;
            var brainRadius = brainScale * 5 + 1f;

            new ZombieEatJob()
            {
                DeltaTime = dt,
                ECB = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter(),
                BrainEntity = brainEntity,
                BrainRadiusSq = brainRadius * brainRadius
            }.ScheduleParallel();
        }
    }

    [BurstCompile]
    public partial struct ZombieEatJob : IJobEntity
    {
        public float DeltaTime;
        public EntityCommandBuffer.ParallelWriter ECB;
        public Entity BrainEntity;
        public float BrainRadiusSq;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BrainTag>();
        }
        
        [BurstCompile]
        private void Execute(ZombieEatAspect zombie, [ChunkIndexInQuery] int sortKey)
        {
            if (zombie.IsInEatingRange(float3.zero, BrainRadiusSq))
            {
                zombie.Eat(DeltaTime, ECB, sortKey, BrainEntity);    
            }
            else
            {
                ECB.SetComponentEnabled<ZombieWalkProperties>(sortKey,  zombie.Entity, true);
                ECB.SetComponentEnabled<ZombieEatProperties>(sortKey, zombie.Entity, false);
            }
        }
    }
}