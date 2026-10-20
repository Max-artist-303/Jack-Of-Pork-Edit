using AmongUs.GameOptions;
using BepInEx.Configuration;
using BepInEx.Core.Logging.Interpolation;
using HarmonyLib;
using Hazel;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSystem.Text.Json;
using JAM.Modules.Components;
using JAM.Options.Roles.Neutral;
using JAM.Roles.Neutral;
using MiraAPI;
using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.GameOptions;
using MiraAPI.GameOptions.OptionTypes;
using MiraAPI.Networking;
using MiraAPI.Roles;
using MiraAPI.Translation;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using Reactor.Networking.Attributes;
using Reactor.Networking.Rpc;
using Reactor.Utilities;
using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using TownOfUs.Utilities;
using UnityEngine;
using Object = UnityEngine.Object;

namespace JAM.Options.OptionTypes;

// Make Saves for when you close the game and reopen it
// Make View Lobby Rules work

public class ModdedRolesOption(string title, List<RoleBehaviour> defaultList, Func<RoleBehaviour, bool> CheckValidRole = null, bool includeInPreset = true) : ModdedOption<RoleTypes>(title, RoleTypes.Crewmate, includeInPreset), IModdedOption
{
    public ModdedRolesMenu? rolesMenu;
    public List<RoleBehaviour> rolesList = new List<RoleBehaviour>();

    public override OptionBehaviour CreateOption(ToggleOption toggleOpt, NumberOption numberOpt, StringOption stringOpt, PlayerOption playerOpt, Transform container)
    {
        ToggleOption toggleOption = Object.Instantiate<ToggleOption>(toggleOpt, Vector3.zero, Quaternion.identity, container);
        toggleOption.name = base.ParentMod.OptionsTitleText + ".RoleOption." + base.StringName;
        toggleOption.SetUpFromData(base.Data, 20);
        toggleOption.Title = base.StringName;
        toggleOption.TitleText.text = base.Title.Translate();
        toggleOption.CheckMark.enabled = false;
        toggleOption.OnValueChanged = new Action<OptionBehaviour>(OnRoleButtonClicked);


        base.OptionBehaviour = toggleOption;

        return toggleOption;
    }

    void OnRoleButtonClicked(OptionBehaviour optionBehaviour)
    {
        Info("Opening Roles Menu");

        if (rolesMenu != null || !PlayerControl.LocalPlayer.IsHost()) return;

        rolesMenu = ModdedRolesMenu.Create(this);
        rolesMenu.Begin(IsRoleValid, ClickRoleHandle);

        (OptionBehaviour as ToggleOption)?.CheckMark.enabled = false;
    }

    protected override void OnValueChanged(RoleTypes newValue)
    {
        if (newValue == RoleTypes.Crewmate || newValue == RoleTypes.Impostor) return;

        RoleBehaviour role = DestroyableSingleton<RoleManager>.Instance.GetRole(newValue);

        if (!PlayerControl.LocalPlayer.IsHost()) ClickRoleHandle(role, false);

        bool addedRole = rolesList.Any(r => r.Role == newValue);

        StringOption? opt = base.OptionBehaviour as StringOption;
        if (opt != null)
        {
            opt.Value = (int)RoleTypes.Crewmate;
        }

        if (PlayerControl.LocalPlayer.IsHost())
        {
            SyncRoles(this);
        }

        ModdedOptionsManager.AddSettingsChangeMessage(DestroyableSingleton<HudManager>.Instance.Notifier, base.StringName, (rolesList.Any(r => r.Role == newValue) ? "Added " : "Removed ") + DestroyableSingleton<RoleManager>.Instance.GetRole((RoleTypes)newValue).GetRoleName(), this.Configuration.PopUpTextColor, this.Configuration.PopUpIconTmp, false);
    }

    private void ClickRoleHandle(RoleBehaviour role)
    {
        ClickRoleHandle(role, true);
    }

    bool IsRoleValid(RoleBehaviour role)
    {
        var predicate = CheckValidRole ?? (x => true);

        return predicate(role);
    }
    void ClickRoleHandle(RoleBehaviour role, bool isSetting)
    {
        bool addedRole = false;
        if (rolesList.Any(r => r.Role == role.Role))
        {
            rolesList.RemoveAll(r => r.Role == role.Role);
        }
        else
        {
            rolesList.Add(role);
            addedRole = true;
        }

        if (!isSetting) return;

        SetValue(role.Role, true);

        rolesMenu?.RefreshRoles();
    }

    public override float GetFloatData()
    {
        return 0;
    }

    public override NetData GetNetData()
    {
        return new NetData(base.Id, Encoding.Unicode.GetBytes(Convert.ToString(base.Value, NumberFormatInfo.InvariantInfo)));
    }

    public override void HandleNetData(byte[] data)
    {
        base.SetValue((RoleTypes)((object)Enum.Parse(typeof(RoleTypes), Encoding.Unicode.GetString(data))), true);
    }

    public override RoleTypes GetValueFromOptionBehaviour(OptionBehaviour optionBehaviour)
    {
        return (RoleTypes)((object)Enum.Parse(typeof(RoleTypes), optionBehaviour.GetInt().ToString(NumberFormatInfo.InvariantInfo)));
    }

    internal static uint currOptionId = 0;
    public static void SyncRoles(ModdedRolesOption? option)
    {
        if (option == null)
        {
            Info("Option Is Null");
            return;
        }

        SyncRolesListRpc.currOptionId = option.Id;

        RoleTypes[] roleTypes = option.rolesList.Select(r => r.Role).ToArray();

        Rpc<SyncRolesListRpc>.Instance.Send(roleTypes);
    }

    public static void SyncAllRoles()
    {
        if (!PlayerControl.LocalPlayer.IsHost()) return;

        List<AbstractOptionGroup>? optionsGroups = (AccessTools.Field(typeof(ModdedOptionsManager), "Groups").GetValue(null) as List<AbstractOptionGroup>);
        if (optionsGroups == null)
        {
            Info("Options Is Null");
            return;
        }

        foreach (AbstractOptionGroup options in optionsGroups)
        {
            foreach (ModdedRolesOption option in options.Children.OfType<ModdedRolesOption>())
            {
                SyncRoles(option);
            }
        }
    }

    // Events And Such
    [RegisterEvent(0)]
    public static void OnRoundStartEvent(RoundStartEvent @event)
    {
        if (@event.TriggeredByIntro)
        {
            SyncAllRoles();
        }
    }
    [HarmonyPatch]
    public static class RefreshModdedRolesOptionsOnJoin
    {
        [HarmonyTargetMethod]
        public static MethodBase TargetMethod()
        {
            System.Type internalClass = AccessTools.TypeByName("TownOfUs.Networking.SendClientModInfoRpc");

            return AccessTools.Method(internalClass, "ReceiveClientModInfo", [typeof(PlayerControl), typeof(Dictionary<byte, string>)]);
        }

        public static void Postfix()
        {
            SyncAllRoles();
        }
    }

    [HarmonyPatch(typeof(MiscUtils), "AppendOptionsText", [typeof(Type)])]
    public static class AppendRoleOptionsPatch
    {
        public static void Postfix(Type classType, ref string __result)
        {
            List<IModdedOption>? options = MiscUtils.GetModdedOptionsForRole(classType)?.Where(option => option is ModdedRolesOption).ToList();
            if (options == null)
            {
                return;
            }

            foreach (IModdedOption option in options)
            {
                Il2CppReferenceArray<Il2CppSystem.Object> tempArray = null;
                __result += DestroyableSingleton<TranslationController>.Instance.GetString(option.StringName, tempArray) + ": ";

                ModdedRolesOption? rolesOption = (option as ModdedRolesOption);

                if (rolesOption != null)
                {
                    List<RoleBehaviour> roles = rolesOption.rolesList;

                    __result += string.Join(" ", roles.Select(role => MiscUtils.GetRoleTmpIcon(role)));
                }
            }
        }
    }
}

[RegisterCustomRpc((uint)JAMRpcCalls.SyncRolesList)]
public class SyncRolesListRpc(Plugin plugin, uint id) : PlayerCustomRpc<JAM.Plugin, RoleTypes[]>(plugin, id)
{
    public static uint currOptionId = 0;
    public override RpcLocalHandling LocalHandling => RpcLocalHandling.After;

    public override void Handle(PlayerControl sender, RoleTypes[]? data)
    {
        ModdedRolesOption? option = GetOptionWithId(currOptionId);

        if(option == null)
        {
            Info("Error: Option For handling role options is null");
            return;
        }

        option.rolesList.RemoveAll(r => true);

        foreach (RoleTypes roleType in data)
        {
            option.rolesList.Add(DestroyableSingleton<RoleManager>.Instance.GetRole(roleType));
        }
    }

    static ModdedRolesOption? GetOptionWithId(uint Id)
    {
        List<AbstractOptionGroup>? optionsGroups = (AccessTools.Field(typeof(ModdedOptionsManager), "Groups").GetValue(null) as List<AbstractOptionGroup>);


        foreach (AbstractOptionGroup options in optionsGroups)
        {
            foreach (ModdedRolesOption option in options.Children.OfType<ModdedRolesOption>())
            {
                if (option.Id == Id)
                    return option;
            }
        }

        return null;
    }

    public override RoleTypes[] Read(MessageReader reader)
    {
        int length = reader.ReadInt32();
        RoleTypes[] roleTypes = new RoleTypes[length];
        currOptionId = (uint)reader.ReadInt32();

        for (int i = 0; i < length; i++)
        {
            roleTypes[i] = (RoleTypes)reader.ReadUInt32();
        }

        return roleTypes;
    }

    public override void Write(MessageWriter writer, RoleTypes[]? data)
    {
        writer.Write(data.Length);
        writer.Write(currOptionId);

        foreach (var value in data)
        {
            writer.Write((uint)value);
        }
    }
}