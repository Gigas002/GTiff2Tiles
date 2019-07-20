using MaterialDesignColors;
using MaterialDesignThemes.Wpf;

namespace GTiff2Tiles.GUI.Models
{
    /// <summary>
    /// Static class for changing material design themes in app.
    /// </summary>
    internal static class ChangeThemeModel
    {
        /// <summary>
        /// Sets current theme.
        /// </summary>
        /// <param name="isDarkTheme">Do you want dark or light theme?</param>
        internal static void ChangeTheme(bool isDarkTheme)
        {
            ITheme theme = new Theme();

            if (isDarkTheme)
                theme.SetBaseTheme(new MaterialDesignDarkTheme());
            else
                theme.SetBaseTheme(new MaterialDesignLightTheme());

            theme.SetPrimaryColor(SwatchHelper.Lookup[MaterialDesignColor.Cyan]);
            //theme.SetSecondaryColor(SwatchHelper.Lookup[MaterialDesignColor.Teal]);

            PaletteHelper paletteHelper = new PaletteHelper();
            paletteHelper.SetTheme(theme);
        }
    }
}
