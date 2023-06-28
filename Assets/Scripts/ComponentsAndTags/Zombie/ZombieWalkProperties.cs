using Unity.Entities;

namespace ComponentsAndTags
{
    public struct ZombieWalkProperties : IComponentData, IEnableableComponent
    {
        public float WalkSpeed;
        public float WalkFrequency;
        public float WalkAmplitude;
    }

    public struct ZombieTimer : IComponentData
    {
        public float Value;
    }

    public struct ZombieHeading : IComponentData
    {
        public float Value;
    }

    public struct NewZombieTag : IComponentData { }
}