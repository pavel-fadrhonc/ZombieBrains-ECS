using ComponentsAndTags;
using Unity.Burst;
using Unity.Entities;

namespace Systems
{
    [BurstCompile]
    [UpdateAfter(typeof(ZombieSpawnSystem))]
    public partial struct ZombieRiseSystem : ISystem
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
            var deltaTime = SystemAPI.Time.DeltaTime;
            var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();

            new ZombieRiseJob()
            {
                DeltaTime = deltaTime,
                Ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter()
            }.ScheduleParallel();
        }
    }

    [BurstCompile]
    public partial struct ZombieRiseJob : IJobEntity
    {
        public float DeltaTime;
        public EntityCommandBuffer.ParallelWriter Ecb;
        
        [BurstCompile]
        private void Execute(ZombieRiseAspect zombie, [EntityInQueryIndex] int sortKey)
        {
            zombie.Rise(DeltaTime);

            if (!zombie.IsAboveGround)
                return;
            
            zombie.SetAtGroundLevel();
            
            Ecb.RemoveComponent<ZombieRiseRate>(sortKey, zombie.Entity);
            Ecb.SetComponentEnabled<ZombieWalkProperties>(sortKey, zombie.Entity, true);
        }
    }
}