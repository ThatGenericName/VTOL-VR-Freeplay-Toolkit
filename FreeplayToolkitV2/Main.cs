global using static FreeplayToolkitV2.Logger;
using System.IO;
using System.Reflection;
using FreeplayToolkitV2.Modules;
using FreeplayToolkitV2.Modules.Weapons;
using FreeplayToolkitV2.Settings;
using ModLoader.Framework;
using ModLoader.Framework.Attributes;
using ModLoader.Framework.Settings;
using VTOLAPI;

namespace FreeplayToolkitV2;

[ItemId("ccw.FreeplayToolkitV2")] // Harmony ID for your mod, make sure this is unique
public class Main : VtolMod
{
    public string ModFolder;

    private void Awake()
    {
        ModFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        Log($"Awake at {ModFolder}");
        
        MunitionsModifier = BaseSettings.New<MunitionsModifier>(this);
        MunitionsModifier.ComputeProperties();
        MunitionsModifier.Save();
        MunitionsModifier.PrintoutCurrentSettings();
        
        FuelModifier = BaseSettings.New<FuelModifier>(this);
        FuelModifier.Save();
        FuelModifier.PrintoutCurrentSettings();

        DamageModifier = BaseSettings.New<DamageModifier>(this);
        DamageModifier.Save();
        MunitionsModifier.PrintoutCurrentSettings();

        VTAPI.SceneLoaded += MunitionsManager.OnSceneLoaded;
    }

    public static MunitionsModifier MunitionsModifier;
    public static FuelModifier FuelModifier;
    public static DamageModifier DamageModifier;
    
    
    public override void UnLoad()
    {
        // Destroy any objects
    }
    
    
}