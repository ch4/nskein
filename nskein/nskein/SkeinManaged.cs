using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nskein
{

    //
    // Implement a few specific Skein implementations. Users can always use the base class
    // to construct any block size x output size combination
    //
    public class Skein512Managed:SimpleSkeinManaged {
        //
        // Skein 512 with 256 bit output by default. 
        //
        public Skein512Managed() : base(512, 256) { }
        public Skein512Managed(UInt32 outputInBits) : base(512, outputInBits) { }
    }

    public class Skein256Managed : SimpleSkeinManaged
    {
        //
        // Skein 256 with 128 bit output by default.
        //
        public Skein256Managed() : base(256, 128) { }
        public Skein256Managed(UInt32 outputInBits) : base(256, outputInBits) { }
    }

    public class Skein1024Managed : SimpleSkeinManaged
    {
        //
        // Skein 1024 with 512 bit output by default.
        //
        public Skein1024Managed() : base(1024, 1024) { }
        public Skein1024Managed(UInt32 outputInBits) : base(1024, outputInBits) { }
    }
}
