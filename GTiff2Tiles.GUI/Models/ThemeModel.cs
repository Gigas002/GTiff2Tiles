#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable CA1308 // Normalize strings to uppercase

using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using Theme = GTiff2Tiles.GUI.Enums.Theme;

namespace GTiff2Tiles.GUI.Models
{
    /// <summary>
    /// Static class for changing material design themes in app
    /// </summary>
    internal static class ThemeModel
    {
        /// <summary>
        /// Sets current theme
        /// </summary>
        /// <param name="theme">New theme</param>
        /// <returns><see cref="BaseTheme"/> that was set</returns>
        internal static BaseTheme SetTheme(Theme theme)
        {
            IBaseTheme baseTheme = theme switch
            {
                Theme.Dark => new MaterialDesignDarkTheme(),
                _ => new MaterialDesignLightTheme() as IBaseTheme
            };

            ITheme itheme = new MaterialDesignThemes.Wpf.Theme();
            itheme.SetBaseTheme(baseTheme);
            itheme.SetPrimaryColor(SwatchHelper.Lookup[MaterialDesignColor.Cyan]);
            //itheme.SetSecondaryColor(SwatchHelper.Lookup[MaterialDesignColor.Teal]);

            PaletteHelper paletteHelper = new();
            paletteHelper.SetTheme(itheme);

            return theme switch
            {
                Theme.Dark => BaseTheme.Dark,
                _ => BaseTheme.Light
            };
        }

        /// <summary>
        /// Parse theme <see cref="string"/>
        /// into <see cref="Theme"/> enum
        /// <remarks><para/>Is input if not underatandable,
        /// sets the theme to <see cref="Theme.Light"/></remarks>
        /// </summary>
        /// <param name="theme">Theme string to parse</param>
        /// <returns>Parsed <see cref="Theme"/></returns>
        internal static Theme GetTheme(string theme) => theme.ToLowerInvariant() switch
        {
            "dark" => Theme.Dark,
            _ => Theme.Light
        };

        /// <summary>
        /// Parse <see cref="Theme"/> enum
        /// into <see cref="string"/>
        /// <remarks><para/>Is input if not underatandable,
        /// sets the theme to <see cref="Theme.Light"/></remarks>
        /// </summary>
        /// <param name="theme"><see cref="Theme"/> enum to parse</param>
        /// <returns>Parsed <see cref="string"/></returns>
        internal static string GetTheme(Theme theme) => theme switch
        {
            Theme.Dark => nameof(Theme.Dark).ToLowerInvariant(),
            _ => nameof(Theme.Light).ToLowerInvariant()
        };
    }
}

#pragma warning restore CA1308 // Normalize strings to uppercase
#pragma warning restore IDE0079 // Remove unnecessary suppression
