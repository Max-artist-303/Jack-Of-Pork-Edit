using JAM.Modifiers.Role;
using JAM.Options.Roles.Crewmate;
using JAM.Roles.Crewmate;
using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Player;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using TownOfUs.Modules;

namespace JAM.Events.Crewmate
{
    public static class ProfessorEvents
    {
        [RegisterEvent(0)]
        public static void CompleteTaskEvent(CompleteTaskEvent @event)
        {
            if (OptionGroupSingleton<ProfessorOptions>.Instance.GetMoreUses && @event.Player.Data.Role is ProfessorRole professor)
            {
                professor.TasksUntilUse--;
                if(professor.TasksUntilUse <= 0)
                {
                    professor.Uses++;
                    professor.TasksUntilUse = (int)OptionGroupSingleton<ProfessorOptions>.Instance.TasksPerUse;
                }
            }
        }
    }
}
