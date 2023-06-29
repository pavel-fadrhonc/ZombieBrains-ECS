using Unity.Entities;
using Unity.VisualScripting;
using UnityEngine;

namespace ComponentsAndTags
{
    public class BrainMono : MonoBehaviour
    {
        public float startingHealth;
    }
    
    public class BrainBaker : Baker<BrainMono>
    {
        public override void Bake(BrainMono authoring)
        {
            var brainEntity = GetEntity(TransformUsageFlags.Dynamic);
            
            AddComponent<BrainTag>(brainEntity);
            AddComponent(brainEntity, new BrainHealth()
            {
                Value = authoring.startingHealth,
                Max = authoring.startingHealth,
                
            });
            AddBuffer<BrainDamageBufferElement>(brainEntity);
        }
    }
}