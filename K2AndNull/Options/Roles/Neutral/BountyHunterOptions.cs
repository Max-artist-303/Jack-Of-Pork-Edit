using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using MiraAPI.Utilities;
using K2AmongUs.Roles.Crewmate;
using K2AmongUs.Roles.Neutral;
using TownOfUs.Extensions;
using TownOfUs.Modules.Localization;

namespace K2AmongUs.Options.Roles.Neutral;

public sealed class BountyHunterOptions : AbstractOptionGroup<BountyHunterRole>
{
    public override string GroupName => "Bounty Hunter Options";

    [ModdedNumberOption("Bounties To Win", 1f, 5f, 1f)]
    public float BountiesToWin { get; set; } = 3;

    [ModdedToggleOption("Target Knows They're Being Targeted")]
    public bool TargetKnows { get; set; } = false;

    [ModdedToggleOption("Crewmate Killings See Bounty Target")]
    public bool CrewAreHunters {  get; set; } = true;
    [ModdedToggleOption("Bounty On The Same Player In A Row")]
    public bool BountyInARow { get; set; } = false;

    [ModdedNumberOption("Faction Modifier Weight", 0f, 100f, 5f, MiraNumberSuffixes.Percent)]
    public float RandFactMod { get; set; } = 10f;
    [ModdedNumberOption("Universal Modifier Weight", 0f, 100f, 5f, MiraNumberSuffixes.Percent)]
    public float RandUnivMod { get; set; } = 10f;
    /*
    [ModdedNumberOption("Lower Cooldowns Weight", 0f, 100f, 5f, MiraNumberSuffixes.Percent)]
    public float lowerCooldowns { get; set; } = 10f;
    */
    [ModdedNumberOption("Allow Venting Weight", 0f, 100f, 5f, MiraNumberSuffixes.Percent)]
    public float GiveVentable { get; set; } = 10f;
    [ModdedNumberOption("Extra Vote Weight", 0f, 100f, 5f, MiraNumberSuffixes.Percent)]
    public float GiveExtraVote { get; set; } = 10f;
    [ModdedNumberOption("Reveal Role Weight (CK Only)", 0f, 100f, 5f, MiraNumberSuffixes.Percent)]
    public float RevealCKRole { get; set; } = 10f;
    [ModdedNumberOption("Double Shot Weight", 0f, 100f, 5f, MiraNumberSuffixes.Percent)]
    public float GiveDblShot { get; set; } = 10f;
    [ModdedNumberOption("Temporary Shield Weight", 0f, 100f, 5f, MiraNumberSuffixes.Percent)]
    public float ShieldNextRound { get; set; } = 10f;
}
