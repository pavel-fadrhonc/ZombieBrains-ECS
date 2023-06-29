using ComponentsAndTags;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Systems
{
    [BurstCompile]
    public partial struct ZombieSpawnSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state) { }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var deltaTime = SystemAPI.Time.DeltaTime;
            var ecbSingleton = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>();
            
            new SpawnZombieJob()
            {
                DeltaTime = deltaTime,
                ECB = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged)
            }.Run();
        }
    }

    [BurstCompile]
    public partial struct SpawnZombieJob : IJobEntity
    {
        public float DeltaTime;
        public EntityCommandBuffer ECB; 
        
        private void Execute(GraveyardAspect graveyard)
        {
            graveyard.ZombieSpawnTimer -= DeltaTime;

            if (!graveyard.TimeToSpawnZombie)
                return;

            if(!graveyard.ZombieSpawnPointInitialized()) return;
            
            graveyard.ZombieSpawnTimer = graveyard.ZombieSpawnRate;

            var newZombie = ECB.Instantiate(graveyard.ZombiePrefab);

            var zombieSpawnPoint = graveyard.GetZombieSpawnPoint();
            
            ECB.SetComponent(newZombie, zombieSpawnPoint);

            var zombieHeading = MathHelpers.GetHeading(zombieSpawnPoint.Position, graveyard.Position);
            ECB.AddComponent(newZombie, new ZombieHeading() {Value = zombieHeading});
        }
    }
}