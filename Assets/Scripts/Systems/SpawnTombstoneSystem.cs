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
            var graveyard = SystemAPI.GetAspect<GraveyardAspect>(graveyardEntity);

            var ecb = new EntityCommandBuffer(Allocator.Temp);
            var builder = new BlobBuilder(Allocator.Temp);
            ref var spawnPoints = ref builder.ConstructRoot<ZombieSpawnPointsBlob>();
            var arrayBuilder = builder.Allocate(ref spawnPoints.Value, graveyard.NumberTombstonesToSpawn);
            var tombStoneOffset = new float3(0f, -3f, 1f);
            
            for (int i = 0; i < graveyard.NumberTombstonesToSpawn; i++)
            {
                var newTombstone = ecb.Instantiate(graveyard.TombstonePrefab);

                var newTombStoneTransform = graveyard.GetRandomTombstoneTransform();
                
                ecb.SetComponent(newTombstone, newTombStoneTransform);

                var newZombieSpawnPoint = newTombStoneTransform.Position + tombStoneOffset;
                arrayBuilder[i] = newZombieSpawnPoint;
            }
            
            var blobAsset = builder.CreateBlobAssetReference<ZombieSpawnPointsBlob>(Allocator.Persistent);
            ecb.SetComponent(graveyardEntity, new ZombieSpawnPoints{Value = blobAsset});
            builder.Dispose();

            ecb.Playback(state.EntityManager);
        }
    }
}