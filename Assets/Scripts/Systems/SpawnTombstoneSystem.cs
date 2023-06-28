using ComponentsAndTags;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems
{
    [BurstCompile]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial struct SpawnTombstoneSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<GraveyardProperties>();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
            
        }
        
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.Enabled = false;

            var graveyardEntity = SystemAPI.GetSingletonEntity<GraveyardProperties>();
            var graveyard = SystemAPI.GetAspectRW<GraveyardAspect>(graveyardEntity);

            var ecb = new EntityCommandBuffer(Allocator.Temp);
            var spawnPoints = new NativeList<float3>(Allocator.Temp);
            var tombStoneOffset = new float3(0f, -3f, 1f);
            
            for (int i = 0; i < graveyard.NumberTombstonesToSpawn; i++)
            {
                var newTombstone = ecb.Instantiate(graveyard.TombstonePrefab);

                var newTombStoneTransform = graveyard.GetRandomTombstoneTransform();
                
                ecb.SetComponent(newTombstone, new LocalToWorldTransform() {Value = newTombStoneTransform});

                var newZombieSpawnPoint = newTombStoneTransform.Position + tombStoneOffset;
                spawnPoints.Add(newZombieSpawnPoint);
            }

            graveyard.ZombieSpawnPoints = spawnPoints.ToArray(Allocator.Persistent);
            
            ecb.Playback(state.EntityManager);
        }
    }
}