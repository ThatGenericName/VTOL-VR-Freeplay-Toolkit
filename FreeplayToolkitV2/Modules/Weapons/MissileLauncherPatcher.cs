using System;
using System.Collections;
using System.Collections.Generic;
using FreeplayToolkitV2.Settings;
using HarmonyLib;
using UnityEngine;

namespace FreeplayToolkitV2.Modules.Weapons;

[HarmonyPatch(typeof(MissileLauncher), nameof(MissileLauncher.FinallyFire))]
public class MissileLauncherPatcher
{
    [HarmonyPrefix]
    public static bool Prefix(MissileLauncher __instance, ref int mIdx)
    {
        MIDX = mIdx;
        Log("Missile object Launched, testing conditions");
        var missileInstance = __instance.missiles[mIdx];
        Log($"Missile Object: {missileInstance.gameObject.name}");
        
        if (VTScenario.current == null || !__instance.parentActor.isPlayer)
        {
            Log($"conditions failed; VTScenario: {VTScenario.current == null} {!__instance.parentActor.isPlayer}");
            return true;
        }
        Log("conditions passed, expecting reload");

        return true;
    }

    private static int MIDX = 0;
    
    public static WaitForSeconds AmmoReloadWait => new(Main.MunitionsModifier.ReloadTime);

    private class ReloadCoroutineClass
    {
        public MissileLauncher instance;
        public int mIdx;
        
        public IEnumerator ReloadCoroutine()
        {
            yield return (object) AmmoReloadWait;
            if (ConsumeFromMagazine(instance, 1) == 1)
            {
                instance.LoadMissile(mIdx);
            }
        }

        public ReloadCoroutineClass(MissileLauncher instance, int midx)
        {
            this.instance = instance;
            mIdx = midx;
        }
    }
    

    [HarmonyPostfix]
    public static void Postfix(MissileLauncher __instance)
    {   

        if (VTScenario.current == null || !__instance.parentActor.isPlayer)
        {
            Log($"conditions failed; VTScenario: {VTScenario.current == null} {!__instance.parentActor.isPlayer}");
            return;
        }
        Log("launch completed, checking for active coroutine");
        // basically, if the scenario already has infinite reload enabled, the 
        // inf reload coroutine would have already been fired, so we ignore it.
        // we're gonna run the same check the method originally does minus the opposite to the infinite ammo part
        // and execute it.
        // note: if the plane is an IWB, it doesn't execute, which means presumably IWBs have different
        // logic for reloads.
        
        bool infiniteAmmoActive = VTScenario.current.infiniteAmmo;
        bool isIWB = (__instance.iwb == null);

        if (!infiniteAmmoActive)
        {
            if (Main.MunitionsModifier.InfiniteAmmo)
            {
                var currentScene = VTScenario.current;
                currentScene.infAmmoReloadDelay = Main.MunitionsModifier.ReloadTime;
                if (isIWB)
                {
                    Log("Is Internal Weapons Bay, executing special operation");
                    // fuck it, running it anyways and seeing if it breaks;
                    __instance.StartCoroutine(__instance.InfReloadRoutine());
                }
                else
                {
                    Log("Normal Launcher, executing coroutine");
                    __instance.StartCoroutine(__instance.InfReloadRoutine());
                }
            }
            else if (Main.MunitionsModifier.HasReloads)
            {
                int launcherId = -1;
                Log($"Checking Missile Launcher Id {launcherId} for reloads");
                
                var magazine = GetMagazine(__instance);
                Log($"Missile Launcher Id {launcherId} has {magazine} reloads remaining!");
                if (magazine > 0)
                {
                    var reloadCoroutineClass = new ReloadCoroutineClass(__instance, MIDX);
                    __instance.StartCoroutine(reloadCoroutineClass.ReloadCoroutine());
                }
            }
        }
            
        return;
    }

    public static int GetMagazine(MissileLauncher __instance)
    {
        if (__instance == null)
        {
            Log("Null instance of missile Launcher passed to GetMagazine");
            return 0;
        }
        if (!__instance.parentActor?.isPlayer ?? false)
        {
            return 0;
        }
        if (!MunitionsManager.MissileMagazineTracker.TryGetValue(__instance, out int magazine))
        {
            Log($"First time checking launcher, launcher has {__instance.hardpoints.Length} hardpoints");
            magazine = __instance.hardpoints.Length * Main.MunitionsModifier.ReloadCount;
            MunitionsManager.MissileMagazineTracker.Add(__instance, magazine);
        }

        return magazine;
    }

    /// <summary>
    /// Consumes <c>amount</c> munitions from the magazine, returns actual amount consumed
    /// </summary>
    /// <param name="instance"></param>
    /// <param name="amount"></param>
    /// <returns></returns>
    public static int ConsumeFromMagazine(MissileLauncher instance, int amount)
    {
        if (!MunitionsManager.MissileMagazineTracker.TryGetValue(instance, out int magazine))
        {
            return 0;
        }

        int realAmount = Math.Min(amount, magazine);
        MunitionsManager.MissileMagazineTracker[instance] -= realAmount;
        return realAmount;
    }
}