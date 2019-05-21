using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IQ.Entities.VastMetaDB
{
    public enum DataFormat
    {
        Unknown = 0,
        Unformatted = 1,
        PhoneNumber = 2,
        Money = 3,
        ValidFileOrPath = 4,    // does not contain Path.GetInvalidFileNameChars()) or Path.GetInvalidPathChars()
        Number = 5
    }
}
