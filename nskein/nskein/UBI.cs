using System;

namespace nskein
{
    internal class UBI
    {
        //
        // Unique block iteration mode as per spec. Hard coded to use Threefish at the moment
        //

        // Intermediate value so far
        private UInt64[] _Hi;
        private Threefish.ThreeFishBlockSize _blockSize;

        public UInt64[] CurrentOutputState { get { return _Hi; } }
 
        internal UBI(UInt64[] g, UInt32 blockSizeInBytes){
            _Hi = g ;
            _blockSize = (Threefish.ThreeFishBlockSize)(blockSizeInBytes);

        }

        internal UBI(UInt32 blockSizeInBytes) : 
            this(new UInt64[blockSizeInBytes / sizeof(UInt64)], blockSizeInBytes){
          //
          // Construct empty internal state
          //           
        }



        internal void Update(UInt64[] message, Tweak tweak){
            //
            // Takes encoded tweak value and message, encrypts using current state as key and XORs
            // result with message. The block size is determined by looking at input message length
            // TODO: Reuse block cipher instance
            //

            if (_Hi.Length != message.Length){
                throw new InvalidOperationException("Can't change block size in mid flight");

            }
            Threefish threeFish = new Threefish(_Hi, tweak, _blockSize);
            threeFish.Encrypt(message, _Hi);

            for (int i = 0; i < _Hi.Length; i++){

                _Hi[i] ^= message[i]; 
            }

        }
    }
}
