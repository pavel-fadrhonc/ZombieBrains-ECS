using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace ComponentsAndTags
{
    public readonly partial struct BrainAspect : IAspect
    {
        public readonly Entity Entity;
        
        private readonly RefRW<LocalTransform> _transform;
        private readonly RefRW<BrainHealth> _brainHealth;
        private readonly DynamicBuffer<BrainDamageBufferElement> _brainDamageBuffer;

        private float BrainHealth
        {
            get => _brainHealth.ValueRO.Value;
            set => _brainHealth.ValueRW.Value = value;
        }

        private float BrainHealthMax
        {
            get => _brainHealth.ValueRO.Max;
        }

        public void DamageBrain()
        {
            foreach (var bufferElement in _brainDamageBuffer)
            {
                BrainHealth -= bufferElement.Value;
            }
            
            _brainDamageBuffer.Clear();
            
            _transform.ValueRW.Scale = BrainHealth / BrainHealthMax;
        }
    }
}