using ThunderRoad;

namespace StandaloneReactionAnimationMod.Core
{
    internal sealed partial class SRAMManager
    {
        private static string SafeName(Creature creature)
        {
            if (creature == null)
            {
                return "null";
            }

            if (!string.IsNullOrWhiteSpace(creature.creatureId))
            {
                return creature.creatureId;
            }

            if (creature.data != null && !string.IsNullOrWhiteSpace(creature.data.id))
            {
                return creature.data.id;
            }

            return "instance_" + creature.GetInstanceID();
        }
    }
}
