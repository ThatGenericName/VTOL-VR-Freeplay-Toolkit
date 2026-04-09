using System;
using System.Linq;
using HarmonyLib;

namespace FreeplayToolkitV2.Modules.Weapons;

[HarmonyPatch(typeof(HMDWeaponInfo), nameof(HMDWeaponInfo.Update))]
public class HMDAmmoDisplayPatcher
{


    public enum WepGroup
    {
        None,
        Guns,
        RocketLauncher,
        MissileLauncher
    }

    private static WepGroup LastWepGroup = WepGroup.None;
    
    [HarmonyPostfix]
    public static void Postfix(HMDWeaponInfo __instance)
    {
        if (!__instance.wm || !__instance.wm.currentEquip)
        {
            return;
        }

        int currentCount = 0;
        int spareCount = 0;

        var firstCombEquip = __instance.wm.combinedEquips[0];
        WepGroup lastWepGroup = LastWepGroup;
        if (firstCombEquip is RocketLauncher)
        {
            foreach (var rl in __instance.wm.combinedEquips.Cast<RocketLauncher>())
            {
                spareCount += RocketLauncherPatcher.GetMagazine(rl);
                currentCount += rl.GetCount();
            }
        }
        else if (firstCombEquip is HPEquipMissileLauncher)
        {
            foreach (var mlhp in __instance.wm.combinedEquips.Cast<HPEquipMissileLauncher>())
            {
                spareCount += MissileLauncherPatcher.GetMagazine(mlhp.ml);
                currentCount += mlhp.GetCount();
            }
        }
        else if (firstCombEquip is HPEquipGun)
        {
            foreach (var ghp in __instance.wm.combinedEquips.Cast<HPEquipGun>())
            {
                if (ghp.gun == null)
                {
                    Log("Gun for GHP is Null!");
                }
                spareCount += GunPatcher.GetMagazine(ghp.gun);
                currentCount += ghp.GetCount();
            }
        }
        else
        {
            Log($"Player switched to unknown weapon type {firstCombEquip.GetType()}");
            return;
        }

        if (spareCount == 0)
        {
            return;
        }

        __instance.weaponCountText.text = $"{currentCount} ({spareCount})";
    }
}