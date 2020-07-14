using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTiff2Tiles.Core.Constants
{
    public class Geodesic
    {
        public const double EquatorRadius = 6378137.0;

        public const double PolarRadius = 6356752.314245;

        /// <summary>
        /// 20037508.342789244
        /// 2.0 * Math.PI * EquatorRadius 2.0;
        /// </summary>
        public const double OriginShift = Math.PI * EquatorRadius;
    }
}
