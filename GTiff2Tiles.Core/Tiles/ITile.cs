using System;
using System.Collections.Generic;
using System.IO;

namespace GTiff2Tiles.Core.Tiles
{
    public interface ITile : IDisposable, IAsyncDisposable
    {
        #region Properties

        public bool IsDisposed { get; set; }
        public double MinLongtiude { get; set; }
        public double MinLatitude { get; set; }
        public double MaxLongitude { get; set; }
        public double MaxLatitude { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
        public IEnumerable<byte> D { get; set; }
        public int S { get; }
        public int Size { get; set; }
        public FileInfo FileInfo { get; set; }
        public bool TmsCompatible { get; set; }
        public string Extension { get; set; }

        #endregion

        public void FlipY();
        public bool Validate(bool isCheckFileInfo);
        public int CalculatePosition();
        public (int tileMinX, int tileMinY, int tileMaxX, int tileMaxY) GetNumbersFromCoords(bool tmsCompatible);
        public (int tileMinX, int tileMinY, int tileMaxX, int tileMaxY) GetLowerNumbers(int zoom);
        public void SetBounds();
    }
}
