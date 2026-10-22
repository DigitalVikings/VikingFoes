using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using VikingFoes.Configuration;

namespace VikingFoes
{
    [BepInPlugin(ModGuid, ModName, ModVersion)]
    [BepInDependency("com.jotunn.jotunn", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.digitalvikings.vikinglib", BepInDependency.DependencyFlags.HardDependency)]
    public class VikingFoesPlugin : BaseUnityPlugin
    {
        public const string ModGuid = "com.digitalvikings.vikingfoes";
        public const string ModName = "VikingFoes";
        public const string ModVersion = "0.1.0";

        internal static ManualLogSource Log = null!;
        private Harmony _harmony = null!;

        private void Awake()
        {
            Log = Logger;
            Log.LogInfo($"{ModName} v{ModVersion} initializing...");

            // Initialize configuration
            ModConfig.Init(Config);

            // Initialize creature definitions
            Registries.FoeRegistry.Init();

            // Hook VikingLib systems
            Integration.VikingLibIntegration.Init();

            // Apply Harmony patches
            _harmony = new Harmony(ModGuid);
            _harmony.PatchAll();

            // Register Jötunn console commands if Jötunn is loaded
            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.jotunn.jotunn"))
            {
                RegisterCommands();
                HookJotunnSync();
            }

            Log.LogInfo($"{ModName} v{ModVersion} loaded successfully.");
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
        private void RegisterCommands()
        {
            Jotunn.Managers.CommandManager.Instance.AddConsoleCommand(new Commands.FoeInfoConsoleCommand());
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
        private void HookJotunnSync()
        {
            Jotunn.Managers.SynchronizationManager.OnConfigurationSynchronized += (sender, args) =>
            {
                Log.LogInfo("Server configuration synchronized successfully.");
            };
        }

        private void OnDestroy()
        {
            _harmony?.UnpatchSelf();
        }
    }
}
