/*using JAM.Options.Roles.Crewmate;
using JAM.Roles.Crewmate;
using JAM.Roles.Neutral;
using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using MiraAPI.Utilities;
using TownOfUs.Extensions;
using TownOfUs.Modules.Localization;

namespace JAM.Options.Roles.Neutral;

public sealed class BountyHunterOptions : AbstractOptionGroup<BountyHunterRole>
{
    public override string GroupName => "Bounty Hunter Options";

    [ModdedNumberOption("Bounties To Win", 1f, 5f, 1f)]
    public float BountiesToWin { get; set; } = 3;

    [ModdedToggleOption("Arrow To Bounty Target")]
    public bool TargetArrow { get; set; } = true;

    [ModdedNumberOption("Bounty Target Speed Modifier", 1f, 3f, 0.25f, MiraNumberSuffixes.Multiplier)]
    public float TargetSpeed { get; set; } = 1.5f;

    [ModdedNumberOption("Hunted Player Grace Period", 0f, 30f, 5f, MiraNumberSuffixes.Seconds)]
    public float HuntedGracePeriod { get; set; } = 15;

    [ModdedToggleOption("Breaks Between Bounties")]
    public bool BountyBreaks { get; set; } = false;

    //Hunting Period
    [ModdedToggleOption("Hunting Time Is Limited")]
    public bool LimitedHuntingTime { get; set; } = false;
    public ModdedNumberOption BountyTimeframe { get; set; } = new("Bounty Hunting Timeframe", 30f, 30f, 120f, 5f, MiraNumberSuffixes.Seconds)
    {
        Visible = () => OptionGroupSingleton<BountyHunterOptions>.Instance.LimitedHuntingTime
    };


    #region Reward Stuff
    [ModdedToggleOption("Show Reward Weights")]
    public bool ShowOptions { get; set; } = false;

    public ModdedNumberOption RandFactMod { get; set; } = new("Faction Modifier (Crewmate Only)", 10f, 0f, 100f, 5f, MiraNumberSuffixes.Percent)
    {
        Visible = () => OptionGroupSingleton<BountyHunterOptions>.Instance.ShowOptions
    };
    public ModdedNumberOption RandUnivMod { get; set; } = new("Universal Modifier", 10f, 0f, 100f, 5f, MiraNumberSuffixes.Percent)
    {
        Visible = () => OptionGroupSingleton<BountyHunterOptions>.Instance.ShowOptions
    };

    /*
    public ModdedNumberOption LowerCooldowns { get; set; } = new("Lower Cooldowns", 10f, 0f, 100f, 5f, MiraNumberSuffixes.Percent)
    {
        Visible = () => OptionGroupSingleton<BountyHunterOptions>.Instance.ShowOptions
    };
    *//*

    public ModdedNumberOption GiveVentable { get; set; } = new("Allow Venting", 10f, 0f, 100f, 5f, MiraNumberSuffixes.Percent)
    {
        Visible = () => OptionGroupSingleton<BountyHunterOptions>.Instance.ShowOptions
    };
    public ModdedNumberOption GiveExtraVote { get; set; } = new("+1 Vote", 10f, 0f, 100f, 5f, MiraNumberSuffixes.Percent)
    {
        Visible = () => OptionGroupSingleton<BountyHunterOptions>.Instance.ShowOptions
    };
    public ModdedNumberOption RevealRole { get; set; } = new("Reveal Role (Crewmate Only)", 10f, 0f, 100f, 5f, MiraNumberSuffixes.Percent)
    {
        Visible = () => OptionGroupSingleton<BountyHunterOptions>.Instance.ShowOptions
    };
    public ModdedNumberOption GiveDblShot { get; set; } = new("Double Shot", 10f, 0f, 100f, 5f, MiraNumberSuffixes.Percent)
    {
        Visible = () => OptionGroupSingleton<BountyHunterOptions>.Instance.ShowOptions
    };
    public ModdedNumberOption ShieldNextRound { get; set; } = new("Temporary Shield", 10f, 0f, 100f, 5f, MiraNumberSuffixes.Percent)
    {
        Visible = () => OptionGroupSingleton<BountyHunterOptions>.Instance.ShowOptions
    };
    #endregion
}
*/