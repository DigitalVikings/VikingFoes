using HarmonyLib;
using VikingFoes.Configuration;
using VikingFoes.Registries;
using VikingLib.Combat;

namespace VikingFoes.Patches
{
    [HarmonyPatch(typeof(Attack))]
    public static class AttackDamagePatches
    {
        [HarmonyPatch("ModifyDamage")]
        [HarmonyPostfix]
        public static void ModifyDamage_Postfix(Humanoid ___m_character, HitData hitData)
        {
            if (___m_character == null || hitData == null || ___m_character.IsPlayer()) return;
            if (!ModConfig.ModEnabled.Value || !ModConfig.EnableThematicDamageTypes.Value) return;

            string prefab = FoeRegistry.GetCleanPrefabName(___m_character);
            if (!FoeRegistry.TryGet(prefab, out var def) || !def.PhysicalDamageOverride.HasValue) return;

            float blunt = hitData.m_damage.m_blunt;
            float slash = hitData.m_damage.m_slash;
            float pierce = hitData.m_damage.m_pierce;
            float totalPhys = blunt + slash + pierce;

            if (totalPhys <= 0.001f) return;

            // Clear physical damage and reassign to the override type
            hitData.m_damage.m_blunt = 0f;
            hitData.m_damage.m_slash = 0f;
            hitData.m_damage.m_pierce = 0f;

            switch (def.PhysicalDamageOverride.Value)
            {
                case DamageType.Blunt:
                    hitData.m_damage.m_blunt = totalPhys;
                    break;
                case DamageType.Slash:
                    hitData.m_damage.m_slash = totalPhys;
                    break;
                case DamageType.Pierce:
                    hitData.m_damage.m_pierce = totalPhys;
                    break;
            }
        }
    }
}
