using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using VikingFoes.Core;
using VikingLib.Combat;

namespace VikingFoes.Registries
{
    /// <summary>
    /// Central thread-safe registry of creature combat definitions and overrides.
    /// </summary>
    public static class FoeRegistry
    {
        private static readonly ConcurrentDictionary<string, FoeDefinition> _definitions =
            new(StringComparer.OrdinalIgnoreCase);

        private static readonly ConcurrentDictionary<string, string> _aliases =
            new(StringComparer.OrdinalIgnoreCase)
            {
                ["GreydwarfBrute"] = "Greydwarf_Elite",
                ["Greydwarf_Brute"] = "Greydwarf_Elite",
                ["SeaSerpent"] = "Serpent"
            };

        static FoeRegistry()
        {
            InitDefaults();
        }

        public static void Init()
        {
            // Forces static constructor execution if not already triggered
        }

        /// <summary>
        /// Registers or updates a creature definition.
        /// </summary>
        public static void Register(FoeDefinition definition)
        {
            if (definition == null) return;
            string clean = CleanPrefabName(definition.PrefabName);
            _definitions[clean] = definition;
        }

        /// <summary>
        /// Attempts to get a registered FoeDefinition by prefab or alias name.
        /// </summary>
        public static bool TryGet(string prefabName, out FoeDefinition definition)
        {
            if (string.IsNullOrWhiteSpace(prefabName))
            {
                definition = null!;
                return false;
            }

            string clean = CleanPrefabName(prefabName);
            if (_aliases.TryGetValue(clean, out string canonical))
            {
                clean = canonical;
            }

            return _definitions.TryGetValue(clean, out definition);
        }

        /// <summary>
        /// Gets all currently registered creature definitions.
        /// </summary>
        public static IEnumerable<FoeDefinition> GetAll() => _definitions.Values;

        /// <summary>
        /// Extracts the canonical clean prefab name from a Character instance.
        /// </summary>
        public static string GetCleanPrefabName(Character? character)
        {
            if (character == null) return string.Empty;

            string raw = string.Empty;
            try
            {
                if (character.gameObject != null)
                {
                    raw = global::Utils.GetPrefabName(character.gameObject);
                }
            }
            catch { }

            if (string.IsNullOrEmpty(raw))
            {
                raw = character.m_name;
            }

            if (string.IsNullOrEmpty(raw))
            {
                try { raw = character.name; } catch { }
            }

            return CleanPrefabName(raw ?? string.Empty);
        }

        /// <summary>
        /// Strips "(Clone)" suffixes and whitespace.
        /// </summary>
        public static string CleanPrefabName(string rawName)
        {
            if (string.IsNullOrWhiteSpace(rawName)) return string.Empty;

            string clean = rawName;
            int cloneIndex = clean.IndexOf("(Clone)", StringComparison.OrdinalIgnoreCase);
            if (cloneIndex >= 0)
            {
                clean = clean.Substring(0, cloneIndex);
            }

            return clean.Trim();
        }

        private static void InitDefaults()
        {
            // =========================================================================
            // 1. BOAR
            // - Deals slashing damage (tusk gouge)
            // - Increase HP 10 -> 13
            // - Slightly weak (1.25x) against pierce
            // - Poise threshold: 18f
            // - 25% chance to inflict minor bleeding laceration (4 dmg over 4s)
            // =========================================================================
            Register(new FoeDefinition("Boar")
                .SetBaseHealth(13f)
                .SetPhysicalDamageType(DamageType.Slash)
                .SetResistance(DamageType.Pierce, 1.25f)
                .SetPoiseThreshold(18f)
                .AddOnHitAffliction(VikingLib.StatusEffects.CanonicalStatusEffects.BleedId, value: 4f, duration: 4f, chance: 0.25f));

            // =========================================================================
            // 2. GREYDWARFS (Root & Branch Wood Anatomy)
            // - Puncturing dry tangled branches/roots with narrow thrusts is inefficient (0.8x Pierce)
            // - Greydwarf Brute (Greydwarf_Elite) delivers crushing bludgeoning blows (Blunt)
            // - Poise threshold: 85f (sturdy brute frame)
            // =========================================================================
            Register(new FoeDefinition("Greydwarf")
                .SetResistance(DamageType.Pierce, 0.8f));

            Register(new FoeDefinition("Greydwarf_Shaman")
                .SetResistance(DamageType.Pierce, 0.8f));

            Register(new FoeDefinition("Greydwarf_Elite")
                .SetPhysicalDamageType(DamageType.Blunt)
                .SetResistance(DamageType.Pierce, 0.8f)
                .SetPoiseThreshold(85f));

            // =========================================================================
            // 3. SERPENT (Ocean Sea Monster)
            // - Deals piercing bite damage
            // - Submerged in saltwater: conducts electricity (1.25x Lightning weak)
            // - Poise threshold: 120f
            // - 75% chance to inflict deep arterial bleeding (12 dmg over 6s)
            // =========================================================================
            Register(new FoeDefinition("Serpent")
                .SetPhysicalDamageType(DamageType.Pierce)
                .SetResistance(DamageType.Lightning, 1.25f)
                .SetPoiseThreshold(120f)
                .AddOnHitAffliction(VikingLib.StatusEffects.CanonicalStatusEffects.BleedId, value: 12f, duration: 6f, chance: 0.75f));

            // =========================================================================
            // 4. WOLF (Mountain Apex Predator)
            // - Thick arctic winter coat provides natural cold insulation (0.75x Frost)
            // - Poise threshold: 38f
            // - 50% chance to inflict tearing laceration (8 dmg over 5s)
            // =========================================================================
            Register(new FoeDefinition("Wolf")
                .SetResistance(DamageType.Frost, 0.75f)
                .SetPoiseThreshold(38f)
                .AddOnHitAffliction(VikingLib.StatusEffects.CanonicalStatusEffects.BleedId, value: 8f, duration: 5f, chance: 0.50f));
        }
    }
}
