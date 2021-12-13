using BitMiracle.LibTiff.Classic;
using GTiff2Tiles.Core.Constants;
using GTiff2Tiles.Core.Coordinates;
using GTiff2Tiles.Core.Enums;
using GTiff2Tiles.Core.Helpers;
using GTiff2Tiles.Core.Images;
using GTiff2Tiles.Core.Localization;

namespace GTiff2Tiles.Core.GeoTiffs;

/// <summary>
/// Base abstract class for GeoTiffs
/// </summary>
public abstract class GeoTiff : IGeoTiff
{
    #region Properties

    /// <inheritdoc />
    public Size Size { get; init; }

    /// <inheritdoc />
    public GeoCoordinate MinCoordinate { get; init; }

    /// <inheritdoc />
    public GeoCoordinate MaxCoordinate { get; init; }

    /// <inheritdoc />
    public CoordinateSystem GeoCoordinateSystem { get; init; }

    /// <inheritdoc />
    public bool IsDisposed { get; protected set; }

    #endregion

    #region Dispose

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc cref="Dispose()"/>
    /// <param name="disposing">Dispose static fields?</param>
    protected virtual void Dispose(bool disposing)
    {
        if (IsDisposed) return;

        if (disposing)
        {
            // Occurs only if called by programmer. Dispose static things here
        }

        IsDisposed = true;
    }

    /// <inheritdoc />
    public ValueTask DisposeAsync()
    {
#pragma warning disable CA1031 // Do not catch general exception types

        try
        {
            Dispose();

            return default;
        }
        catch (Exception exception)
        {
            return ValueTask.FromException(exception);
        }

#pragma warning restore CA1031 // Do not catch general exception types
    }

    #endregion

    #region GetBorders

    /// <summary>
    /// Gets minimal and maximal coordinates from input GeoTiff
    /// </summary>
    /// <param name="inputStream">Any kind of stream with GeoTiff's data</param>
    /// <param name="coordinateSystem">GeoTiff's <see cref="CoordinateSystem"/>
    /// <remarks><para/>If set to <see cref="CoordinateSystem.Other"/> throws
    /// <see cref="NotSupportedException"/></remarks></param>
    /// <returns><see cref="ValueTuple{T1,T2}"/> of
    /// <see cref="GeoCoordinate"/>s of image's borders</returns>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="NotSupportedException"/>
    /// <exception cref="ArgumentException"/>
    public static (GeoCoordinate minCoordinate, GeoCoordinate maxCoordinate) GetBorders(Stream inputStream, CoordinateSystem coordinateSystem)
    {
        #region Preconditions checks

        if (inputStream == null) throw new ArgumentNullException(nameof(inputStream));

        string err = string.Format(Strings.Culture, Strings.IsBroken, nameof(inputStream));

        if (!inputStream.CanRead) throw new ArgumentException(err);
        // CoordinateSystem checked lower

        #endregion

        // Disable warnings from libtiff
        Tiff.SetErrorHandler(new LibTiffHelper());

        // Don't dispose -- it disposes the inputStream as well
        Tiff tiff = Tiff.ClientOpen(string.Empty, "r", inputStream, new TiffStream());

        if (tiff == null) throw new ArgumentException(err);

        // Get origin coordinates
        FieldValue[] tiePointTag = tiff.GetField(TiffTag.GEOTIFF_MODELTIEPOINTTAG);

        // Get pixel scale
        FieldValue[] pixScaleTag = tiff.GetField(TiffTag.GEOTIFF_MODELPIXELSCALETAG);

        // Image's sizes
        int width = tiff.GetField(TiffTag.IMAGEWIDTH)[0].ToInt();
        int height = tiff.GetField(TiffTag.IMAGELENGTH)[0].ToInt();

        byte[] tiePoints = tiePointTag[1].GetBytes();
        double pixelScale = BitConverter.ToDouble(pixScaleTag[1].GetBytes(), 0);

        double minX = BitConverter.ToDouble(tiePoints, 24);
        double maxY = BitConverter.ToDouble(tiePoints, 32);
        double maxX = minX + width * pixelScale;
        double minY = maxY - height * pixelScale;

        // Reset stream reading position
        inputStream.Seek(0, SeekOrigin.Begin);

        err = string.Format(Strings.Culture, Strings.NotSupported, coordinateSystem);

        switch (coordinateSystem)
        {
            case CoordinateSystem.Epsg4326:
                {
                    GeodeticCoordinate minCoordinate = new(minX, minY);
                    GeodeticCoordinate maxCoordinate = new(maxX, maxY);

                    return (minCoordinate, maxCoordinate);
                }
            case CoordinateSystem.Epsg3857:
                {
                    MercatorCoordinate minCoordinate = new(minX, minY);
                    MercatorCoordinate maxCoordinate = new(maxX, maxY);

                    return (minCoordinate, maxCoordinate);
                }
            default: throw new NotSupportedException(err);
        }
    }

    /// <inheritdoc cref="GetBorders(Stream, CoordinateSystem)"/>
    /// <param name="filePath">Full path to GeoTiff file</param>
    /// <param name="coordinateSystem"></param>
    public static (GeoCoordinate minCoordinate, GeoCoordinate maxCoordinate) GetBorders(string filePath, CoordinateSystem coordinateSystem)
    {
        #region Preconditions checks

        CheckHelper.CheckFile(filePath, true, FileExtensions.Tif);
        // CoordinateSystem checked lower

        #endregion

        // Disable warnings from libtiff
        Tiff.SetErrorHandler(new LibTiffHelper());

        using Tiff tiff = Tiff.Open(filePath, "r");

        string err = string.Format(Strings.Culture, Strings.IsBroken, filePath);

        if (tiff == null) throw new ArgumentException(err);

        // Get origin coordinates
        FieldValue[] tiePointTag = tiff.GetField(TiffTag.GEOTIFF_MODELTIEPOINTTAG);

        // Get pixel scale
        FieldValue[] pixScaleTag = tiff.GetField(TiffTag.GEOTIFF_MODELPIXELSCALETAG);

        // Image's sizes
        int width = tiff.GetField(TiffTag.IMAGEWIDTH)[0].ToInt();
        int height = tiff.GetField(TiffTag.IMAGELENGTH)[0].ToInt();

        byte[] tiePoints = tiePointTag[1].GetBytes();
        double pixelScale = BitConverter.ToDouble(pixScaleTag[1].GetBytes(), 0);

        double minX = BitConverter.ToDouble(tiePoints, 24);
        double maxY = BitConverter.ToDouble(tiePoints, 32);
        double maxX = minX + width * pixelScale;
        double minY = maxY - height * pixelScale;

        err = string.Format(Strings.Culture, Strings.NotSupported, coordinateSystem);

        switch (coordinateSystem)
        {
            case CoordinateSystem.Epsg4326:
                {
                    GeodeticCoordinate minCoordinate = new(minX, minY);
                    GeodeticCoordinate maxCoordinate = new(maxX, maxY);

                    return (minCoordinate, maxCoordinate);
                }
            case CoordinateSystem.Epsg3857:
                {
                    MercatorCoordinate minCoordinate = new(minX, minY);
                    MercatorCoordinate maxCoordinate = new(maxX, maxY);

                    return (minCoordinate, maxCoordinate);
                }
            default: throw new NotSupportedException(err);
        }
    }

    #endregion
}
