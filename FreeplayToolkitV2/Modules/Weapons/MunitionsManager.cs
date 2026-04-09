using System.Collections.Generic;
using VTOLAPI;

namespace FreeplayToolkitV2.Modules.Weapons;

public class MunitionsManager
{
    public static Dictionary<MissileLauncher, int> MissileMagazineTracker = new();
    public static Dictionary<RocketLauncher, int> RocketMagazineTracker = new();
    public static Dictionary<Gun, int> GunMagazineTracker = new();
    public static Dictionary<Countermeasure, int> CMMagazineTracker = new();
    

    public static void OnSceneLoaded(VTScenes scene)
    {
        if (scene != VTScenes.CustomMapBase)
        {
            Log("New Scene Loaded, Clearing magazines");
            MissileMagazineTracker.Clear();
            RocketMagazineTracker.Clear();
            GunMagazineTracker.Clear();

            var playerGO = VTAPI.GetPlayersVehicleGameObject();
            if (playerGO != null)
            {
                var cms = playerGO.GetComponentsInChildren<Countermeasure>();
                foreach (var cm in cms)
                {
                    CounterMeasuresPatcher.Getmagazine(cm);
                }
            }
        }
    }
}