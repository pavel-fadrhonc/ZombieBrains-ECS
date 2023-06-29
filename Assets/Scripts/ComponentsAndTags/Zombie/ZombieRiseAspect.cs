using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace ComponentsAndTags
{
    public readonly partial struct ZombieRiseAspect : IAspect
    {
        public readonly Entity Entity;

        private readonly RefRW<LocalTransform> _transform;
        
        private readonly RefRO<ZombieRiseRate> _zombieRiseRate;

        public void Rise(float dt)
        {
            _transform.ValueRW.Position += math.up() * _zombieRiseRate.ValueRO.Value * dt;
        }

        public bool IsAboveGround => _transform.ValueRO.Position.y > 0;

        public void SetAtGroundLevel()
        {
            var position = _transform.ValueRO.Position;
            position.y = 0;
            _transform.ValueRW.Position = position;
        }
    }
}