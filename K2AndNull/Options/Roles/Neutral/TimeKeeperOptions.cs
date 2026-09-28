using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using MiraAPI.Utilities;
using K2AmongUs.Roles.Neutral;

namespace K2AmongUs.Options.Roles.Neutral;

public sealed class TimeKeeperOptions : AbstractOptionGroup<TimeKeeperRole>
{
    public override string GroupName => "Time Keeper Options";
    
    [ModdedNumberOption("Kill Cooldown", 5f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float TimeKeeperCooldown { get; set; } = 20f;

    [ModdedNumberOption("Cd Decrease", 0f, 10f, 0.5f, MiraNumberSuffixes.Seconds)]
    public float CooldownDecrease { get; set; } = 3f;

    [ModdedToggleOption("Cooldown Decreases Each Tie")]
    public bool AlwaysDecreaseCd { get; set; } = false;

    // [ModdedToggleOption("Forbearing Can Vent")]
    // public bool ForbearingCanVent { get; set; } = true;
}