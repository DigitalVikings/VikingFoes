using HarmonyLib;
using VikingFoes.Configuration;
using VikingFoes.Registries;
using VikingLib.Combat;
using VikingLib.Combat.Defense;

namespace VikingFoes.Patches
{
    [HarmonyPatch(typeof(Character))]
    public static class CharacterHealthPatches
    {
        [HarmonyPatch(nameof(Character.GetMaxHealthBase))]
        [HarmonyPostfix]
        public static void GetMaxHealthBase_Postfix(Character __instance, ref float __result)
        {
            if (__instance == null || __instance.IsPlayer()) return;
            if (ModConfig.ModEnabled != null && (!ModConfig.ModEnabled.Value || !ModConfig.EnableHealthOverrides.Value)) return;

            string prefab = FoeRegistry.GetCleanPrefabName(__instance);
            if (FoeRegistry.TryGet(prefab, out var def) && def.OverrideBaseHealth.HasValue)
            {
                __result = def.OverrideBaseHealth.Value;
            }
        }

        [HarmonyPatch(nameof(Character.GetDamageModifiers), typeof(WeakSpot))]
        [HarmonyPostfix]
        public static void GetDamageModifiers_Postfix(Character __instance, WeakSpot weakspot, ref HitData.DamageModifiers __result)
        {
            if (__instance == null || __instance.IsPlayer()) return;
            if (ModConfig.ModEnabled != null && (!ModConfig.ModEnabled.Value || !ModConfig.EnableResistanceOverrides.Value)) return;

            string prefab = FoeRegistry.GetCleanPrefabName(__instance);
            if (!FoeRegistry.TryGet(prefab, out var def) || def.Resistances.Count == 0) return;

            foreach (var kvp in def.Resistances)
            {
                HitData.DamageModifier mod = EntityDefenseRegistry.ToVanillaModifier(kvp.Value);
                switch (kvp.Key)
                {
                    case DamageType.Blunt: __result.m_blunt = mod; break;
                    case DamageType.Slash: __result.m_slash = mod; break;
                    case DamageType.Pierce: __result.m_pierce = mod; break;
                    case DamageType.Chop: __result.m_chop = mod; break;
                    case DamageType.Pickaxe: __result.m_pickaxe = mod; break;
                    case DamageType.Fire: __result.m_fire = mod; break;
                    case DamageType.Frost: __result.m_frost = mod; break;
                    case DamageType.Lightning: __result.m_lightning = mod; break;
                    case DamageType.Poison: __result.m_poison = mod; break;
                    case DamageType.Spirit: __result.m_spirit = mod; break;
                }
            }
        }
    }
}
