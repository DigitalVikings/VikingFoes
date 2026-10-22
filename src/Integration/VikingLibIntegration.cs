using System;
using UnityEngine;
using VikingFoes.Configuration;
using VikingFoes.Registries;
using VikingLib.Combat;
using VikingLib.Combat.Defense;
using VikingLib.Combat.Stagger;

namespace VikingFoes.Integration
{
    /// <summary>
    /// Bridges VikingFoes creature definitions with VikingLib's DamagePipeline, EntityDefenseRegistry, and StaggerManager.
    /// </summary>
    public static class VikingLibIntegration
    {
        private static bool _initialized;

        public static void Init()
        {
            if (_initialized) return;
            _initialized = true;

            // 1. Register creature defenses (resistances & armor) into VikingLib's EntityDefenseRegistry
            RegisterDefenses();

            // 2. Register inanimate non-flesh objects into VikingLib's bloodless registry
            RegisterBloodlessObjects();

            // 3. Register custom poise thresholds into VikingLib's CreatureStaggerTable
            RegisterPoiseProfiles();

            // 4. Register Phase 2 AttackerModifier to convert physical damage types via ctx.ConvertDamage
            DamagePipeline.RegisterAttackerModifier(100, HandleMonsterAttackerModifier);

            // 5. Register Phase 6 PostHitProcs to apply custom on-hit status afflictions via DamagePipeline.ApplyOnHitStatus
            DamagePipeline.RegisterHandler(DamagePhase.PostHitProcs, priority: 50, HandleMonsterPostHitProcs);

            VikingFoesPlugin.Log?.LogInfo("[VikingFoes] Successfully hooked VikingLib DamagePipeline, EntityDefenseRegistry, and CreatureStaggerTable.");
        }

        private static void RegisterDefenses()
        {
            foreach (var def in FoeRegistry.GetAll())
            {
                if (def.Resistances.Count > 0 || def.PhysicalArmor > 0f || def.MagicArmor > 0f)
                {
                    EntityDefenseRegistry.Register(def.PrefabName, def.ToEntityDefenseData());
                    VikingFoesPlugin.Log?.LogDebug($"[VikingFoes] Registered defense profile for '{def.PrefabName}' with {def.Resistances.Count} resistances.");
                }
            }
        }

        private static void RegisterBloodlessObjects()
        {
            string[] bloodlessObjects = new[]
            {
                "Greydwarf_spawner",
                "Spawner_GreydwarfNest",
                "BonePileSpawner",
                "Spawner_BonePile"
            };

            foreach (var obj in bloodlessObjects)
            {
                EntityDefenseRegistry.RegisterBloodless(obj);
            }
            VikingFoesPlugin.Log?.LogDebug($"[VikingFoes] Registered {bloodlessObjects.Length} inanimate spawner prefabs as bloodless entities.");
        }

        private static void RegisterPoiseProfiles()
        {
            foreach (var def in FoeRegistry.GetAll())
            {
                if (def.PoiseThreshold.HasValue)
                {
                    CreatureStaggerTable.Register(
                        def.PrefabName,
                        def.PoiseThreshold.Value,
                        def.StaggerDurationMultiplier,
                        def.HasNativeAnimation
                    );
                    VikingFoesPlugin.Log?.LogDebug($"[VikingFoes] Registered poise profile for '{def.PrefabName}': Poise={def.PoiseThreshold.Value}, DurationMult={def.StaggerDurationMultiplier}x, NativeAnim={def.HasNativeAnimation}");
                }
            }
        }

        private static void HandleMonsterAttackerModifier(DamageContext ctx)
        {
            if (ModConfig.ModEnabled != null && (!ModConfig.ModEnabled.Value || !ModConfig.EnableThematicDamageTypes.Value)) return;
            if (ctx.Attacker == null || ctx.Attacker.IsPlayer()) return;

            string prefab = FoeRegistry.GetCleanPrefabName(ctx.Attacker);
            if (!FoeRegistry.TryGet(prefab, out var def) || !def.PhysicalDamageOverride.HasValue) return;

            DamageType targetType = def.PhysicalDamageOverride.Value;
            DamageType[] physTypes = new[] { DamageType.Blunt, DamageType.Slash, DamageType.Pierce };

            foreach (var pt in physTypes)
            {
                if (pt != targetType && ctx.GetDamage(pt) > 0.001f)
                {
                    ctx.ConvertDamage(pt, targetType, 1.0f, $"{prefab} {targetType} Strike");
                }
            }
        }

        private static void HandleMonsterPostHitProcs(DamageContext ctx)
        {
            if (ModConfig.ModEnabled != null && !ModConfig.ModEnabled.Value) return;
            if (ctx.IsCancelled || ctx.IsPreview) return;
            if (ctx.Attacker == null || ctx.Attacker.IsPlayer() || ctx.Target == null) return;

            string prefab = FoeRegistry.GetCleanPrefabName(ctx.Attacker);
            if (!FoeRegistry.TryGet(prefab, out var def) || def.OnHitAfflictions.Count == 0) return;

            foreach (var affliction in def.OnHitAfflictions)
            {
                if (affliction.Chance < 1.0f && UnityEngine.Random.value > affliction.Chance) continue;

                bool applied = DamagePipeline.ApplyOnHitStatus(
                    ctx.Target,
                    affliction.EffectNameOrId,
                    affliction.Value,
                    affliction.Duration,
                    ctx.Attacker
                );

                if (applied)
                {
                    ctx.Trace?.RecordStep(ctx.CurrentPhase, "VikingFoes", $"{prefab} inflicted {affliction.EffectNameOrId} ({affliction.Duration:F1}s)", affliction.Value);
                }
            }
        }
    }
}
