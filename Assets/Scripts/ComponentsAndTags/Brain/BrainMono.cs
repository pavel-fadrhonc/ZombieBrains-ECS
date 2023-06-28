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
            AddComponent<BrainTag>();
            AddComponent(new BrainHealth()
            {
                Value = authoring.startingHealth,
                Max = authoring.startingHealth,
                
            });
            AddBuffer<BrainDamageBufferElement>();
        }
    }
}