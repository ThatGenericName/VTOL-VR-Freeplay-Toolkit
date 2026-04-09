using System;
using System.Collections;
using System.Collections.Generic;
using FreeplayToolkitV2.Settings;
using HarmonyLib;
using UnityEngine;

namespace FreeplayToolkitV2.Modules.Weapons;

[HarmonyPatch(typeof(Countermeasure), nameof(Countermeasure.ConsumeCM))]
public class CounterMeasuresPatcher
{

    [HarmonyPrefix]
    public static bool Prefix(Countermeasure __instance, int side)
    {
        Side = side;
        return true;
    }

    public static int Side;
    
    public static WaitForSeconds AmmoReloadWait => new(Main.MunitionsModifier.ReloadTime);
    
    private class ReloadCoroutineClass
    {
        public Countermeasure instance;
        
        public IEnumerator ReloadCoroutine()
        {
            Log("Started Countermeasures Reload Coroutine");
            Log($"Waiting for {Main.MunitionsModifier.ReloadTime} seconds");
            yield return AmmoReloadWait;
            Log("Countermeasures Reload Wait Finished");
            // calculate how many countermeasures we can actually reload
            int currentCount = instance.count;
            
            if (MunitionsManager.CMMagazineTracker.TryGetValue(instance, out var remainingCount))
            {
                int emptySpace = instance.maxCount - currentCount;
                int refill = Math.Min(emptySpace, remainingCount);
                Log($"Countermeasure reload reloaded {refill} units");
                MunitionsManager.CMMagazineTracker[instance] -= refill;
                instance.SetCount(refill + currentCount);
            }
            ActiveCoroutines.Remove(instance);
        }

        public ReloadCoroutineClass(Countermeasure instance)
        {
            this.instance = instance;
        }
    }

    private static Dictionary<Countermeasure, ReloadCoroutineClass> ActiveCoroutines = new(); 
        
    [HarmonyPostfix]
    public static void Postfix(Countermeasure __instance)
    {
        // I apologize for this but this is literally the only way u can get to the actor through a Countermeasure

        
        if (VTScenario.current == null)
        { 
            return;
        }
        
        bool isPlayer = MunitionsManager.CMMagazineTracker.ContainsKey(__instance);

        if (VTScenario.current.infiniteAmmo)
        {
            return;
        }

        if (Main.MunitionsModifier.InfiniteAmmo)
        {
            VTScenario.current.infAmmoReloadDelay = Main.MunitionsModifier.ReloadTime;
            __instance.StartCoroutine(__instance.InfReloadRoutine());
        }
        else if (Main.MunitionsModifier.HasReloads)
        {
            var magazine = Getmagazine(__instance);
            Log($"Countermeasure Dispenser Id {-1} has {magazine} reloads remaining");
            if (magazine > 0)
            {
                if (!ActiveCoroutines.ContainsKey(__instance))
                {
                    var reloadCoroutineClass = new ReloadCoroutineClass(__instance);
                    ActiveCoroutines.Add(__instance, reloadCoroutineClass);
                    Log($"Creating Countermeasures reload coroutine!");
                    __instance.StartCoroutine(reloadCoroutineClass.ReloadCoroutine());
                }
                else
                {
                    Log($"Countermeasure reload coroutine already in progress");
                }
            }
            else
            {
                Log($"Countermeasure has no more reloads!");
            }
        }
        
        return;
    }

    public static int Getmagazine(Countermeasure __instance)
    {
        if (!MunitionsManager.CMMagazineTracker.TryGetValue(__instance, out int magazine))
        {
            magazine = __instance.maxCount * Main.MunitionsModifier.ReloadCount;
            MunitionsManager.CMMagazineTracker[__instance] = magazine;
        }

        return magazine;
    }
}