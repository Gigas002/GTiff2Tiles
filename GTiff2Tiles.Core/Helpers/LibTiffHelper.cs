using BitMiracle.LibTiff.Classic;

namespace GTiff2Tiles.Core.Helpers;
// See: https://stackoverflow.com/questions/17103628/how-to-disable-warnings-messages-displayed-on-console-using-libtiff-c-sharp</remarks>

/// <summary>
/// Class, that helps to disable libtiff's warnings
/// </summary>
internal class LibTiffHelper : TiffErrorHandler
{
    /// <inheritdoc />
    public override void WarningHandler(Tiff tif, string method, string format, params object[] args)
    {
        // do nothing, ie, do not write warnings to console
    }

    /// <inheritdoc />
    public override void WarningHandlerExt(Tiff tif, object clientData, string method, string format, params object[] args)
    {
        // do nothing ie, do not write warnings to console
    }
}