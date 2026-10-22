# VikingFoes

**VikingFoes** overhauls Valheim's creature combat dynamics through anatomical damage resistances, converted attack types, calibrated poise stagger thresholds, and organic bleed afflictions built on VikingLib.

> ⚠️ **Work in Progress (Pre-Release v0.1.0)**: This mod is in active development and has not been tested extensively. Features, balance, and mechanics may change, and unintended bugs or edge cases may occur. Backup your character and world saves before testing.

> ⚡ **Powered by VikingLib**: This mod requires [VikingLib](https://valheim.thunderstore.io/package/DigitalVikings/VikingLib/). 
> VikingLib replaces Valheim's native combat calculation and stat engines. Not all mods may be compatible / work as expected with this.

---

## How It Works

Creatures no longer rely on arbitrary monolithic health pools and generic attacks; their combat identity reflects their physical anatomy:

- **Anatomical Damage Interactions**: Physical resistances match creature physiology. Thrusting tangled roots with narrow spears is inefficient (-20% pierce damage against Greydwarfs), while sea serpents submerged in saltwater conduct heavy electrical discharge (+25% lightning damage).
- **Physical Attack Type Conversions**: Creature attacks deliver accurate physical damage types. Boars gouge with razor tusks (Slashing), Greydwarf Brutes smash with heavy clubbed fists (Blunt), and Serpents strike with puncture bites (Piercing).
- **Poise Stagger Thresholds**: Creatures have individual poise ratings governing stagger resistance, rewarding heavy weapons and timed parries.
- **Organic Afflictions**: Predator attacks organically inflict bleeding lacerations over time via VikingLib's status effect pipeline.

<details>
<summary><b>Click to expand Overhauled Creature Roster</b></summary>

| Creature | Health | Primary Attack | Key Resistances | Poise | Status Afflictions |
| :--- | :--- | :--- | :--- | :--- | :--- |
| **Boar** | 13 HP | Slashing (Tusk gouge) | 1.25× Pierce (Thin hide) | 18 | 25% chance to inflict minor Bleed (4 dmg over 4s) |
| **Greydwarf** | Vanilla | Vanilla | 0.80× Pierce (Tangled roots) | Vanilla | None |
| **Greydwarf Shaman** | Vanilla | Vanilla | 0.80× Pierce (Tangled roots) | Vanilla | None |
| **Greydwarf Brute** | Vanilla | Blunt (Crushing fists) | 0.80× Pierce (Dense wood) | 85 | None |
| **Serpent** | Vanilla | Piercing (Deep bite) | 1.25× Lightning (Saltwater) | 120 | 75% chance to inflict arterial Bleed (12 dmg over 6s) |
| **Wolf** | Vanilla | Slashing (Tearing bite) | 0.75× Frost (Winter coat) | 38 | 50% chance to inflict tearing Bleed (8 dmg over 5s) |

</details>
