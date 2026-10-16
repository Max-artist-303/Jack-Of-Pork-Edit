using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using MiraAPI.Utilities;
using JAM.Roles.Crewmate;
using JAM.Roles.Neutral;
using TownOfUs.Extensions;
using TownOfUs.Modules.Localization;

namespace JAM.Options.Roles.Crewmate;

public sealed class ProfessorOptions : AbstractOptionGroup<ProfessorRole>
{
    public override string GroupName => "Professor Options";

    [ModdedNumberOption("Max Uses", 1f, 5f, 1f)]
    public float MaxUses { get; set; } = 2;

    [ModdedToggleOption("Get More Uses From Tasks")]
    public bool GetMoreUses { get; set; } = false;
    public ModdedNumberOption TasksPerUse { get; set; } = new("Number Of Tasks Per Extra Use", 3f, 1f, 5f, 1f, MiraNumberSuffixes.None)
    {
        Visible = () => OptionGroupSingleton<ProfessorOptions>.Instance.GetMoreUses
    };
}
