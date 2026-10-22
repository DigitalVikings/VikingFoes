using System;
using System.Collections.Generic;
using VikingLib.Combat;
using VikingLib.Combat.Defense;

namespace VikingFoes.Core
{
    /// <summary>
    /// Declarative combat and stat override definition for a creature prefab.
    /// </summary>
    public class FoeDefinition
    {
        public string PrefabName { get; }

        /// <summary>
        /// Optional base health override at Level 1 (0 stars).
        /// If null, creature uses vanilla base health.
        /// </summary>
        public float? OverrideBaseHealth { get; set; }

        /// <summary>
        /// Optional physical damage type override (e.g. Slashing for Boar, Bludgeoning for Brute, Piercing for Serpent).
        /// When set, the creature's outgoing physical damage is converted to this type.
        /// </summary>
        public DamageType? PhysicalDamageOverride { get; set; }

        /// <summary>
        /// Continuous float damage resistance multipliers (e.g. Pierce = 1.25f for 25% extra pierce damage taken).
        /// Registered directly into VikingLib's EntityDefenseRegistry.
        /// </summary>
        public Dictionary<DamageType, float> Resistances { get; } = new();

        /// <summary>
        /// Base physical armor (default 0f).
        /// </summary>
        public float PhysicalArmor { get; set; } = 0f;

        /// <summary>
        /// Base elemental/magic armor (default 0f).
        /// </summary>
        public float MagicArmor { get; set; } = 0f;

        /// <summary>
        /// Optional decoupled poise threshold for CreatureStaggerTable.
        /// If null, creature uses VikingLib's calibrated defaults or synthesized curve.
        /// </summary>
        public float? PoiseThreshold { get; set; }

        /// <summary>
        /// Stagger duration multiplier (default 1.0f).
        /// </summary>
        public float StaggerDurationMultiplier { get; set; } = 1.0f;

        /// <summary>
        /// Whether this creature has native Unity stagger animation clips (false triggers artificial stun).
        /// </summary>
        public bool HasNativeAnimation { get; set; } = true;

        /// <summary>
        /// Status afflictions inflicted by this foe upon striking a target.
        /// Applied via DamagePipeline.ApplyOnHitStatus.
        /// </summary>
        public List<FoeStatusAffliction> OnHitAfflictions { get; } = new();

        public FoeDefinition(string prefabName)
        {
            if (string.IsNullOrWhiteSpace(prefabName))
                throw new ArgumentException("PrefabName cannot be null or whitespace", nameof(prefabName));

            PrefabName = prefabName.Trim();
        }

        public FoeDefinition SetBaseHealth(float health)
        {
            OverrideBaseHealth = health;
            return this;
        }

        public FoeDefinition SetPhysicalDamageType(DamageType type)
        {
            PhysicalDamageOverride = type;
            return this;
        }

        public FoeDefinition SetResistance(DamageType type, float multiplier)
        {
            Resistances[type] = multiplier;
            return this;
        }

        public FoeDefinition SetPoiseThreshold(float threshold, float durationMultiplier = 1.0f, bool hasNativeAnimation = true)
        {
            PoiseThreshold = threshold;
            StaggerDurationMultiplier = durationMultiplier;
            HasNativeAnimation = hasNativeAnimation;
            return this;
        }

        public FoeDefinition AddOnHitAffliction(string effectNameOrId, float value, float duration, float chance = 1.0f)
        {
            OnHitAfflictions.Add(new FoeStatusAffliction(effectNameOrId, value, duration, chance));
            return this;
        }

        /// <summary>
        /// Converts this definition into a VikingLib EntityDefenseData instance for EntityDefenseRegistry.
        /// </summary>
        public EntityDefenseData ToEntityDefenseData()
        {
            var data = new EntityDefenseData(PhysicalArmor, MagicArmor);
            foreach (var kvp in Resistances)
            {
                data.Resistances[kvp.Key] = kvp.Value;
            }
            return data;
        }
    }

    /// <summary>
    /// Declarative status affliction triggered when a foe lands a hit.
    /// Evaluated in Phase 6 (PostHitProcs) via DamagePipeline.ApplyOnHitStatus.
    /// </summary>
    public class FoeStatusAffliction
    {
        public string EffectNameOrId { get; }
        public float Value { get; }
        public float Duration { get; }
        public float Chance { get; }

        public FoeStatusAffliction(string effectNameOrId, float value, float duration, float chance = 1.0f)
        {
            if (string.IsNullOrWhiteSpace(effectNameOrId))
                throw new ArgumentException("EffectNameOrId cannot be null or whitespace", nameof(effectNameOrId));

            EffectNameOrId = effectNameOrId.Trim();
            Value = value;
            Duration = duration;
            Chance = Math.Max(0f, Math.Min(1.0f, chance));
        }
    }
}
