using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.IO;

namespace MvcPlatform.Models
{
    public class NpoiMemoryStream : MemoryStream 
    {
        public NpoiMemoryStream()
        {
            AllowClose = true;
        }

        public bool AllowClose { get; set; }

        public override void Close()
        {
            if (AllowClose)
                base.Close();
        }
    }
}