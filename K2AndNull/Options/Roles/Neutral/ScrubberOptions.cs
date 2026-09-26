using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using MiraAPI.Utilities;
using K2AmongUs.Roles.Crewmate;
using K2AmongUs.Roles.Neutral;
using TownOfUs.Extensions;
using TownOfUs.Modules.Localization;

namespace K2AmongUs.Options.Roles.Neutral;

public sealed class ScrubberOptions : AbstractOptionGroup<ScrubberRole>
{
        public override string GroupName => "Cleanser Options";

        [ModdedNumberOption("Scrub Cooldown", 5f, 60f, 5f, MiraNumberSuffixes.Seconds)]
    public float ScrubCooldown { get; set; } = 30f;

    [ModdedNumberOption("Scrub Delay", 0f, 10f, 0.5f, MiraNumberSuffixes.Seconds)]
    public float ScrubDelay { get; set; } = 5f;
}