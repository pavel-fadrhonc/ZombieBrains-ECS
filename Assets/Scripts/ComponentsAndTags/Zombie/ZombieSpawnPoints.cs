using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace ComponentsAndTags
{
    public struct ZombieSpawnPoints : IComponentData
    {
        public NativeArray<float3> Value;
    }
}