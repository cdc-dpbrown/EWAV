using System;
using System.Collections.Generic;

namespace Ewav.DTO
{
    public class PointDTO
    {
        public double LonX { get; set; }
        public double LatY { get; set; }
        public string MapTip { get; set; }
        public List<string> Data { get; set; }
        public string StrataValue { get; set; }


    }

    public class PointDTOCollection
    {
        public List<PointDTO> Collection { get; set; }
    }
}

