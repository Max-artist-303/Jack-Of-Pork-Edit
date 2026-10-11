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
}
