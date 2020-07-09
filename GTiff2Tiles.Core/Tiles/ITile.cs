using System;
using System.Collections.Generic;
using System.IO;
using GTiff2Tiles.Core.Geodesic;
using GTiff2Tiles.Core.Image;

namespace GTiff2Tiles.Core.Tiles
{
    public interface ITile : IDisposable, IAsyncDisposable
    {
        #region Properties

        public bool IsDisposed { get; set; }
        public Coordinate MinCoordinate { get; set; }
        public Coordinate MaxCoordinate { get; set; }
        public Number Number { get; set; }
        public int Z { get; set; }
        public IEnumerable<byte> D { get; set; }
        public Size Size { get; set; }
        public FileInfo FileInfo { get; set; }
        public bool TmsCompatible { get; set; }
        public string Extension { get; set; }

        #endregion

        public void FlipNumber();
        public bool Validate(bool isCheckFileInfo);
        public int CalculatePosition();
        public (Number minNumber, Number maxNumber) GetNumbersFromCoords(bool tmsCompatible);
        public (Number minNumber, Number maxNumber) GetLowerNumbers(int zoom);
        public void SetBounds();
        public int GetCount(int minZ, int maxZ);
    }
}
