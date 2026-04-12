using VTOLAPI;
using VTOLVR.Multiplayer;

namespace FreeplayToolkitV2.Modules;

/// <summary>
/// A couple locks to disable usage in multiplayer
/// </summary>
public class MultiplayerLock
{


    public static bool IsMultiplayer = true;
    
    public static void OnSceneLoaded(VTScenes scene)
    {

        IsMultiplayer = VTOLMPUtils.IsMultiplayer();
        
    }
    
}