using BepInEx.Configuration;

namespace VikingFoes.Configuration
{
    public static class ModConfig
    {
        public static ConfigEntry<bool> ModEnabled { get; private set; } = null!;
        public static ConfigEntry<bool> EnableThematicDamageTypes { get; private set; } = null!;
        public static ConfigEntry<bool> EnableResistanceOverrides { get; private set; } = null!;
        public static ConfigEntry<bool> EnableHealthOverrides { get; private set; } = null!;

        public static void Init(ConfigFile config)
        {
            ModEnabled = Bind(
                config,
                "1 - General",
                "Enabled",
                true,
                "Master switch to enable or disable all features of VikingFoes.",
                order: 100,
                isAdminOnly: true
            );

            EnableThematicDamageTypes = Bind(
                config,
                "2 - Features",
                "EnableThematicDamageTypes",
                true,
                "Enables thematic physical damage type overrides (e.g. Boar deals Slashing, Greydwarf Brute deals Bludgeoning, Serpent deals Piercing).",
                order: 90,
                isAdminOnly: true
            );

            EnableResistanceOverrides = Bind(
                config,
                "2 - Features",
                "EnableResistanceOverrides",
                true,
                "Enables continuous resistance overrides registered into VikingLib (e.g. Boar is 1.25x weak to Pierce).",
                order: 80,
                isAdminOnly: true
            );

            EnableHealthOverrides = Bind(
                config,
                "2 - Features",
                "EnableHealthOverrides",
                true,
                "Enables creature base health overrides (e.g. Boar base HP increased from 10 to 13).",
                order: 70,
                isAdminOnly: true
            );
        }

        private static ConfigEntry<T> Bind<T>(
            ConfigFile config,
            string section,
            string key,
            T defaultValue,
            string description,
            int order = 0,
            AcceptableValueBase? acceptableValues = null,
            bool isAdminOnly = false)
        {
            var tags = new ConfigurationManagerAttributes
            {
                Order = order,
                Category = section,
                IsAdminOnly = isAdminOnly
            };
            var configDesc = new ConfigDescription(description, acceptableValues, tags);
            return config.Bind(section, key, defaultValue, configDesc);
        }
    }
}
