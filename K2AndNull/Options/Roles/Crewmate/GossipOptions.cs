using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using MiraAPI.Utilities;
using K2AmongUs.Roles.Crewmate;
using K2AmongUs.Roles.Neutral;
using TownOfUs.Extensions;
using TownOfUs.Modules.Localization;

namespace K2AmongUs.Options.Roles.Crewmate;

public sealed class GossipOptions : AbstractOptionGroup<GossipRole>
{
    public override string GroupName => "Gossip";
    
    [ModdedNumberOption("Overhear Cooldown", 0f, 35f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float GossipCooldown { get; set; } = 15f;
    
    [ModdedNumberOption("Overhear Roles Count", 0f, 15f, 1f)]
    public float GossipRoles { get; set; } = 8f;

    [ModdedToggleOption("Shares Info")]
    public bool ShowGossip { get; set; } = true;

    [ModdedNumberOption("Crew Role Weight", 25f, 75f, 5f, MiraNumberSuffixes.Percent)]
    public float CrewWeight { get; set; } = 50;
}