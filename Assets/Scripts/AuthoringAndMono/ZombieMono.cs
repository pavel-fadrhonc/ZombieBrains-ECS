using ComponentsAndTags;
using Unity.Entities;
using UnityEngine;

namespace AuthoringAndMono
{
    public class ZombieMono : MonoBehaviour
    {
        public float RiseRate;
        public float WalkSpeed;
        public float WalkFrequency;
        public float WalkAmplitude;
        
        public float EatDamagePerSecond;
        public float EatAmplitude;
        public float EatFrequency;
    }

    public class ZombieBaker : Baker<ZombieMono>
    {
        public override void Bake(ZombieMono authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            
            AddComponent(entity, new ZombieRiseRate()
            {
                Value = authoring.RiseRate
            });
            
            AddComponent(entity, new ZombieWalkProperties()
            {
                WalkAmplitude = authoring.WalkAmplitude,
                WalkFrequency = authoring.WalkFrequency,
                WalkSpeed = authoring.WalkSpeed
            });
            
            AddComponent<ZombieTimer>(entity);
            AddComponent<ZombieHeading>(entity);
            AddComponent<NewZombieTag>(entity);
            AddComponent(entity, new ZombieEatProperties()
            {
                EatAmplitude = authoring.EatAmplitude,
                EatFrequency = authoring.EatFrequency,
                EatDamagePerSecond = authoring.EatDamagePerSecond
            });
        }
    }
}