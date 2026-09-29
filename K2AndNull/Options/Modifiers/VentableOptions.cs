using JackOfAllMods.Modifiers.Game.Universal;
using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using MiraAPI.Utilities;
using JackOfAllMods.Roles.Crewmate;
using JackOfAllMods.Roles.Neutral;
using TownOfUs.Extensions;
using TownOfUs.Modules.Localization;

namespace JackOfAllMods.Options.Modifiers.Game.Universal;

public sealed class VentableOptions : AbstractOptionGroup<VentableModifier>
{
        public override string GroupName => "Ventable Options";
    
        [ModdedNumberOption("Ventable Count", 0f, 5f, 1f)]
    public float VentableCount { get; set; } = 1f;

        [ModdedNumberOption("Ventable Chance", 0f, 100f, 10f, MiraNumberSuffixes.Percent)]
    public float VentableChance { get; set; } = 50f;

        [ModdedNumberOption("Max Vents", 0f, 100f, 1f, MiraNumberSuffixes.None, null, true)]
    public float MaxVents { get; set; } = 0f;
}
