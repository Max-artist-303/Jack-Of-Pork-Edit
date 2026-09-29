using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using MiraAPI.Utilities;
using JackOfAllMods.Roles.Crewmate;
using JackOfAllMods.Roles.Neutral;
using TownOfUs.Extensions;
using TownOfUs.Modules.Localization;

namespace JackOfAllMods.Options.Roles.Crewmate;

public sealed class StealthyOptions : AbstractOptionGroup<SnoopRole>
{
    public override string GroupName => "Snoop Options";
    
    [ModdedNumberOption("Sneak Cooldown", 0f, 45f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float SneakCooldown { get; set; } = 25f;

    [ModdedNumberOption("Sneak Duration", 10f, 120f, 5f, MiraNumberSuffixes.Seconds)]
    public float SneakDuration { get; set; } = 30f;
}
