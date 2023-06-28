using Unity.Entities;

namespace ComponentsAndTags
{
    public struct BrainHealth : IComponentData
    {
        public float Value;
        public float Max;
    }
}