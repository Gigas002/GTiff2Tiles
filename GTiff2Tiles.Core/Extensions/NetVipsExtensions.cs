using System.Collections.Generic;
using System.Linq;

namespace GTiff2Tiles.Core.Extensions
{
    public static class NetVipsExtensions
    {
        public static NetVips.Image AddBands(this NetVips.Image image, int bands)
        {
            //TODO modify
            for (; image.Bands < bands;) image = image.Bandjoin(255);

            return image;
        }
        public static IEnumerable<NetVips.Image> AddBands(this IEnumerable<NetVips.Image> images, int bands) =>
            images.Select(image => image.AddBands(bands));
    }
}
