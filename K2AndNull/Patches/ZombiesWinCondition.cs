using System.Runtime.CompilerServices;
using JackOfAllMods.Roles.Neutral;
using MiraAPI.GameEnd;
using MiraAPI.Utilities;
using TMPro;
using TownOfUs.Modules;
using UnityEngine;

namespace JackOfAllMods.Patches.WinConditions;

public sealed class ZombieGameOver : CustomGameOver
{
		public override bool VerifyCondition(PlayerControl playerControl, NetworkedPlayerInfo[] winners)
    {

        if (Helpers.GetAlivePlayers().Any(p => p.Data.Role is ZombieLeaderRole))
        {
            int numNonZombies = Helpers.GetAlivePlayers().Count(p => p.Data.Role is not ZombieRole && p.Data.Role is not ZombieLeaderRole);
            IEnumerable<NetworkedPlayerInfo> zombies = PlayerControl.AllPlayerControls.ToArray().Where(p => p.Data.Role is ZombieRole || p.Data.Role is ZombieLeaderRole).Select(p => p.Data);

            if (numNonZombies < zombies.Count() && TownOfUs.Utilities.MiscUtils.KillersAliveCount == 0)
            {
                Info("Zombies Should Win!");
				return true;
            }
        }
		return false;
    }

		public override void AfterEndGameSetup(EndGameManager endGameManager)
	{
		endGameManager.BackgroundBar.material.SetColor(ShaderID.Color, JackOfAllMods.Colors.Zombie);
		TextMeshPro text = UnityEngine.Object.Instantiate<TextMeshPro>(endGameManager.WinText);
		text.text = "Zombies Win!";
		text.color = JackOfAllMods.Colors.Zombie;
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(17, 2);
		defaultInterpolatedStringHandler.AppendLiteral("<color=#");
		defaultInterpolatedStringHandler.AppendFormatted(JackOfAllMods.Colors.Zombie);
		defaultInterpolatedStringHandler.AppendLiteral(">");
        defaultInterpolatedStringHandler.AppendLiteral("Zombies Win!</color>");
        GameHistory.WinningFaction = defaultInterpolatedStringHandler.ToStringAndClear();
		Vector3 pos = endGameManager.WinText.transform.localPosition;
		pos.y = 1.5f;
		pos += Vector3.down * 0.15f;
		text.transform.localScale = new Vector3(1f, 1f, 1f);
		text.transform.position = pos;
		text.text = "<size=4>" + text.text + "</size>";
	}
}
