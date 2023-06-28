using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace ComponentsAndTags
{
    public readonly partial struct ZombieRiseAspect : IAspect
    {
        public readonly Entity Entity;

        private readonly TransformAspect _transformAspect;
        
        private readonly RefRO<ZombieRiseRate> _zombieRiseRate;

        public void Rise(float dt)
        {
            _transformAspect.Position += math.up() * _zombieRiseRate.ValueRO.Value * dt;
        }

        public bool IsAboveGround => _transformAspect.Position.y > 0;

        public void SetAtGroundLevel()
        {
            var position = _transformAspect.Position;
            position.y = 0;
            _transformAspect.Position = position;
        }
    }
}