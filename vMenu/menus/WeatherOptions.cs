using System.Collections.Generic;

using CitizenFX.Core;

using MenuAPI;

using vMenuShared;

using static vMenuClient.CommonFunctions;
using static vMenuShared.PermissionsManager;

namespace vMenuClient.menus
{
    public class WeatherOptions
    {
        // Variables
        private Menu menu;
        public MenuCheckboxItem dynamicWeatherEnabled;
        public MenuCheckboxItem blackout;
        public MenuCheckboxItem snowEnabled;
        public static readonly List<string> weatherTypes = new()
        {
            "EXTRASUNNY",
            "CLEAR",
            "NEUTRAL",
            "SMOG",
            "FOGGY",
            "CLOUDS",
            "OVERCAST",
            "CLEARING",
            "RAIN",
            "THUNDER",
            "BLIZZARD",
            "SNOW",
            "SNOWLIGHT",
            "XMAS",
            "HALLOWEEN"
        };

        private void CreateMenu()
        {
            // Create the menu.
            menu = new Menu(Game.Player.Name, "Weather Options");

            dynamicWeatherEnabled = new MenuCheckboxItem("Toggle Dynamic Weather", "Enable or disable dynamic weather changes.", EventManager.DynamicWeatherEnabled);
            blackout = new MenuCheckboxItem("Toggle Blackout", "This disables or enables all lights across the map.", EventManager.IsBlackoutEnabled);
            snowEnabled = new MenuCheckboxItem("Enable Snow Effects", "This will force snow to appear on the ground and enable snow particle effects for peds and vehicles. Combine with X-MAS or Light Snow weather for best results.", ConfigManager.GetSettingsBool(ConfigManager.Setting.vmenu_enable_snow));
            var extrasunny = new MenuItem("Extra Sunny", "Set the weather to ~y~extra sunny~s~!") { ItemData = "EXTRASUNNY" };
            var clear = new MenuItem("Clear", "Set the weather to ~y~clear~s~!") { ItemData = "CLEAR" };
            var neutral = new MenuItem("Neutral", "Set the weather to ~y~neutral~s~!") { ItemData = "NEUTRAL" };
            var smog = new MenuItem("Smog", "Set the weather to ~y~smog~s~!") { ItemData = "SMOG" };
            var foggy = new MenuItem("Foggy", "Set the weather to ~y~foggy~s~!") { ItemData = "FOGGY" };
            var clouds = new MenuItem("Cloudy", "Set the weather to ~y~clouds~s~!") { ItemData = "CLOUDS" };
            var overcast = new MenuItem("Overcast", "Set the weather to ~y~overcast~s~!") { ItemData = "OVERCAST" };
            var clearing = new MenuItem("Clearing", "Set the weather to ~y~clearing~s~!") { ItemData = "CLEARING" };
            var rain = new MenuItem("Rainy", "Set the weather to ~y~rain~s~!") { ItemData = "RAIN" };
            var thunder = new MenuItem("Thunder", "Set the weather to ~y~thunder~s~!") { ItemData = "THUNDER" };
            var blizzard = new MenuItem("Blizzard", "Set the weather to ~y~blizzard~s~!") { ItemData = "BLIZZARD" };
            var snow = new MenuItem("Snow", "Set the weather to ~y~snow~s~!") { ItemData = "SNOW" };
            var snowlight = new MenuItem("Light Snow", "Set the weather to ~y~light snow~s~!") { ItemData = "SNOWLIGHT" };
            var xmas = new MenuItem("X-MAS Snow", "Set the weather to ~y~x-mas~s~!") { ItemData = "XMAS" };
            var halloween = new MenuItem("Halloween", "Set the weather to ~y~halloween~s~!") { ItemData = "HALLOWEEN" };
            var removeclouds = new MenuItem("Remove All Clouds", "Remove all clouds from the sky!");
            var randomizeclouds = new MenuItem("Randomize Clouds", "Add random clouds to the sky!");

            if (IsAllowed(Permission.WODynamic))
            {
                menu.AddMenuItem(dynamicWeatherEnabled);
            }
            if (IsAllowed(Permission.WOBlackout))
            {
                menu.AddMenuItem(blackout);
            }
            if (IsAllowed(Permission.WOSetWeather))
            {
                menu.AddMenuItem(snowEnabled);
                menu.AddMenuItem(extrasunny);
                menu.AddMenuItem(clear);
                menu.AddMenuItem(neutral);
                menu.AddMenuItem(smog);
                menu.AddMenuItem(foggy);
                menu.AddMenuItem(clouds);
                menu.AddMenuItem(overcast);
                menu.AddMenuItem(clearing);
                menu.AddMenuItem(rain);
                menu.AddMenuItem(thunder);
                menu.AddMenuItem(blizzard);
                menu.AddMenuItem(snow);
                menu.AddMenuItem(snowlight);
                menu.AddMenuItem(xmas);
                menu.AddMenuItem(halloween);
            }
            if (IsAllowed(Permission.WORandomizeClouds))
            {
                menu.AddMenuItem(randomizeclouds);
            }

            if (IsAllowed(Permission.WORemoveClouds))
            {
                menu.AddMenuItem(removeclouds);
            }

            menu.OnItemSelect += (sender, item, index2) =>
            {
                if (item == removeclouds)
                {
                    ModifyClouds(true);
                }
                else if (item == randomizeclouds)
                {
                    ModifyClouds(false);
                }
                else if (item.ItemData is string weatherType)
                {
                    Notify.Custom($"The weather will be changed to ~y~{item.Text}~s~. This will take {EventManager.WeatherChangeTime} seconds.");
                    UpdateServerWeather(weatherType, EventManager.IsBlackoutEnabled, EventManager.DynamicWeatherEnabled, EventManager.IsSnowEnabled);
                }
            };

            menu.OnCheckboxChange += (sender, item, index, _checked) =>
            {
                if (item == dynamicWeatherEnabled)
                {
                    Notify.Custom($"Dynamic weather changes are now {(_checked ? "~g~enabled" : "~r~disabled")}~s~.");
                    UpdateServerWeather(EventManager.GetServerWeather, EventManager.IsBlackoutEnabled, _checked, EventManager.IsSnowEnabled);
                    var logString = $"Dynamic weather {(_checked ? "enabled" : "disabled")} by " + Game.Player.Name + ". Current weather is: " + EventManager.GetServerWeather;
                    TriggerServerEvent("vMenu:discordLogs", "vMenu: Dynamic Weather", logString, 2045380);
                }
                else if (item == blackout)
                {
                    Notify.Custom($"Blackout mode is now {(_checked ? "~g~enabled" : "~r~disabled")}~s~.");
                    UpdateServerWeather(EventManager.GetServerWeather, _checked, EventManager.DynamicWeatherEnabled, EventManager.IsSnowEnabled);
                    var logString = $"Blackout {(_checked ? "enabled" : "disabled")} by " + Game.Player.Name;
                    TriggerServerEvent("vMenu:discordLogs", "vMenu: Blackout", logString, 2045380);
                }
                else if (item == snowEnabled)
                {
                    Notify.Custom($"Snow effects will now be forced {(_checked ? "~g~enabled" : "~r~disabled")}~s~.");
                    UpdateServerWeather(EventManager.GetServerWeather, EventManager.IsBlackoutEnabled, EventManager.DynamicWeatherEnabled, _checked);
                }
            };
        }



        /// <summary>
        /// Create the menu if it doesn't exist, and then returns it.
        /// </summary>
        /// <returns>The Menu</returns>
        public Menu GetMenu()
        {
            if (menu == null)
            {
                CreateMenu();
            }
            return menu;
        }
    }
}
