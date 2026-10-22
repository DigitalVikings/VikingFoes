using Jotunn.Entities;
using VikingFoes.Registries;
using VikingLib.Combat;

namespace VikingFoes.Commands
{
    public class FoeInfoConsoleCommand : ConsoleCommand
    {
        public override string Name => "vf_info";
        public override string Help => "Inspects a creature's VikingFoes profile. Usage: vf_info <prefabName>";

        public override void Run(string[] args)
        {
            if (args.Length == 0)
            {
                global::Console.instance?.Print("[VikingFoes] Usage: vf_info <prefabName> (e.g. vf_info Boar, vf_info Greydwarf_Elite, vf_info Serpent)");
                return;
            }

            string name = args[0].Trim();
            if (!FoeRegistry.TryGet(name, out var def))
            {
                global::Console.instance?.Print($"[VikingFoes] No creature definition found for '{name}'.");
                return;
            }

            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"[VikingFoes] Profile: {def.PrefabName}");
            sb.AppendLine($"  - Base Health Override: {(def.OverrideBaseHealth.HasValue ? def.OverrideBaseHealth.Value.ToString("F0") : "Vanilla Default")}");
            sb.AppendLine($"  - Physical Damage Type: {(def.PhysicalDamageOverride.HasValue ? def.PhysicalDamageOverride.Value.ToString() : "Vanilla Default")}");

            if (def.Resistances.Count > 0)
            {
                sb.AppendLine("  - Continuous Resistances:");
                foreach (var kvp in def.Resistances)
                {
                    string modifier = kvp.Value > 1.0f ? $"Weak ({kvp.Value:F2}x)" : kvp.Value < 1.0f ? $"Resistant ({kvp.Value:F2}x)" : "Normal (1.0x)";
                    sb.AppendLine($"      {kvp.Key}: {modifier}");
                }
            }
            else
            {
                sb.AppendLine("  - Continuous Resistances: None (Vanilla default)");
            }

            global::Console.instance?.Print(sb.ToString().TrimEnd());
        }
    }
}
