using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace ComponentsAndTags
{
    public readonly partial struct ZombieEatAspect : IAspect
    {
        public readonly Entity Entity;

        private readonly RefRW<LocalTransform> _transform;

        private readonly RefRO<ZombieEatProperties> _zombieEatProperties;
        private readonly RefRO<ZombieHeading> _zombieHeading;
        private readonly RefRW<ZombieTimer> _zombieTimer;

        private float EatDamagePerSecond => _zombieEatProperties.ValueRO.EatDamagePerSecond;
        private float EatAmplitude => _zombieEatProperties.ValueRO.EatAmplitude;
        private float EatFrequency => _zombieEatProperties.ValueRO.EatFrequency;
        private float Heading => _zombieHeading.ValueRO.Value;

        private float ZombieTimer
        {
            get => _zombieTimer.ValueRO.Value;
            set => _zombieTimer.ValueRW.Value = value;
        }

        public void Eat(float dt, EntityCommandBuffer.ParallelWriter ecb, int sortKey, Entity brainEntity)
        {
            ZombieTimer += dt;
            var eatAngle = EatAmplitude * math.sin(ZombieTimer * EatFrequency);
            _transform.ValueRW.Rotation = quaternion.Euler(eatAngle, Heading, 0);

            var eatDamage = EatDamagePerSecond * dt;
            var curBrainDamage = new BrainDamageBufferElement() { Value = eatDamage };
            ecb.AppendToBuffer(sortKey, brainEntity, curBrainDamage);
        }

        public bool IsInEatingRange(float3 brainPosition, float brainRadiusSq)
        {
            return math.distancesq(brainPosition, _transform.ValueRO.Position) < (brainRadiusSq - 1);
        }
    }
}