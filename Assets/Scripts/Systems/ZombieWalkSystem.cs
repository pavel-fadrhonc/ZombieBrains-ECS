using ComponentsAndTags;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems
{
    [BurstCompile]
    [UpdateAfter(typeof(ZombieRiseSystem))]
    public partial struct ZombieWalkSystem : ISystem
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
            var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            var brainEntity = SystemAPI.GetSingletonEntity<BrainTag>();
            var brainScale = SystemAPI.GetComponent<LocalToWorldTransform>(brainEntity).Value.Scale;
            var brainRadius = brainScale * 5 + 0.5f;

            new ZombieWalkJob()
            {
                DeltaTime = dt,
                BrainCloseDistanceSq = brainRadius * brainRadius,
                ECB = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter()
            }.ScheduleParallel();
        }
    }

    [BurstCompile]
    public partial struct ZombieWalkJob : IJobEntity
    {
        public float DeltaTime;
        public float BrainCloseDistanceSq;
        public EntityCommandBuffer.ParallelWriter ECB;

        [BurstCompile]
        private void Execute(ZombieWalkAspect zombieWalk,[EntityInQueryIndex] int sortKey)
        {
            zombieWalk.Walk(DeltaTime);

            if (zombieWalk.IsInStoppingRange(float3.zero, BrainCloseDistanceSq))
            {
                ECB.SetComponentEnabled<ZombieWalkProperties>(sortKey, zombieWalk.Entity, false);
                ECB.SetComponentEnabled<ZombieEatProperties>(sortKey, zombieWalk.Entity, true);
            }
        }
    }
}