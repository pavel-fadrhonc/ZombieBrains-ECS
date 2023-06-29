using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace ComponentsAndTags
{
    public readonly partial struct ZombieWalkAspect : IAspect
    {
        public readonly Entity Entity;

        private readonly RefRW<LocalTransform> _transform;

        private readonly RefRO<ZombieWalkProperties> _walkProperties;
        private readonly RefRW<ZombieTimer> _walkTimer;
        private readonly RefRO<ZombieHeading> _heading;

        private float WalkSpeed => _walkProperties.ValueRO.WalkSpeed;
        private float WalkFrequency => _walkProperties.ValueRO.WalkFrequency;
        private float WalkAmplitude => _walkProperties.ValueRO.WalkAmplitude;

        private float WalkTimer
        {
            get => _walkTimer.ValueRO.Value;
            set => _walkTimer.ValueRW.Value = value;
        }

        private float Heading => _heading.ValueRO.Value;

        public bool IsInStoppingRange(float3 brainPosition, float brainDistanceSq)
        {
            return math.distancesq(brainPosition, _transform.ValueRO.Position) <= brainDistanceSq;
        }
        
        public void Walk(float dt)
        {
            WalkTimer += dt;
            _transform.ValueRW.Position += _transform.ValueRO.Forward() * WalkSpeed * dt;

            var swayAngle = WalkAmplitude * math.sin(WalkFrequency * WalkTimer);
            _transform.ValueRW.Rotation = quaternion.Euler(0, Heading, swayAngle);
        }
    }
}