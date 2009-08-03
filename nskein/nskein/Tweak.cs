using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nskein
{
    //
    // Various Type values for tweaks as per spec.
    public enum TweakType{
        Key = 0,
        Configuration = 4,
        Personalization = 8,
        PublicKey = 12,
        KeyIdentifier = 16,
        Nonce = 20,
        Message = 48,
        Output = 63
    };

    public struct Tweak    {
        //
        // Simple wrapper around a 128-bit integer
        //

        public UInt64 low64 { get; set; }
        public UInt64 high64 { get; set; }

        private Tweak(UInt64 low, UInt64 high):this(){

            low64 = low;
            high64 = high;
        }

        internal static Tweak EncodeTweak(UInt64 position, bool bitPad, TweakType type, bool first, bool final)
        {

            //
            // Takes tweak parameters and encodes them in a 128 bit value. The lower half of the 128 byte value (only the position)
            // is stuck in low64 and the high half in stuck in high64.
            //
            // N.B SimpleSkeinManaged specifies that position can be upto 2^96 but we support only 2^64. We could add support by rolling our own BigInteger
            //
            // The encoding is as follows (ASCII art not to scale)
            //
            // 128             120                   112            96                                    0
            // --------------------------------------------------------------------------------------------
            // F1|F2|    Type   |B|       TreeLevel   |   reserved  |      Position                       |
            //   |  |           | |                   |             |                                     |
            // --------------------------------------------------------------------------------------------
            //  F1 = First
            //  F2 = Final
            //  B  - BitPad
            //

            Tweak tweak = new Tweak(0,0);
            tweak.high64 = 0;

            //
            // The shift numbers are calculated using 64 - (128 - end-byte-from-diagram-above)
            //
            if (final) tweak.high64 |= (UInt64)1L << 63; // Set F1 in diagram above
            if (first) tweak.high64 |= (UInt64)1L << 62; // Set F2 in diagram above
            tweak.high64 |= (UInt64)type << 56; // Set type
            if (bitPad) tweak.high64 |= (UInt64)1L << 55; // Set bit pad

            tweak.low64 = position;
            return tweak;
        }
    }
}
