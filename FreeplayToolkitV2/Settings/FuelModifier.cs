using ModLoader.Framework.Settings;

namespace FreeplayToolkitV2.Settings;

public class FuelModifier : BaseSettings
{
    public float FuelDrainModifier = 1.0f;
    
    public void PrintoutCurrentSettings()
    {
        Log($"Fuel Drain Modifier: {FuelDrainModifier}");
    }
}