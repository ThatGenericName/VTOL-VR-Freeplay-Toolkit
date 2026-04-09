using HarmonyLib;

namespace FreeplayToolkitV2.Modules.Weapons;

[HarmonyPatch(typeof(MFDHardpointInfo), nameof(MFDHardpointInfo.UpdateDisplay))]
public class MFDHardpointInfoAmmoDisplayPatcher
{
    // todo: finish this
}