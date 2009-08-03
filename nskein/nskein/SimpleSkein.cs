using System;
using System.Security.Cryptography;
namespace nskein
{
   public class SimpleSkeinManaged:HashAlgorithm
    {

     
       private byte[] _blockBuffer;
       private UInt64[] _blockWords;
       private byte[] _config;
       private UInt32 _outputSize;
       private UInt32 _blockSize;
       private bool _initialized;

       private UBI _ubi;

       public override int HashSize { 
           get { return (int)_outputSize; }
       }
       public override int InputBlockSize   {
           get { return (int)_blockSize;  }
       }
       public override int OutputBlockSize
       {
           get { return (int)_outputSize; }
       }

       public SimpleSkeinManaged(UInt32  blockSizeInBits, UInt32 outputSizeInBits )
           : base()
       {
           //
           // Convert all internal sizes to bytes
           //
           this._outputSize = outputSizeInBits/8;
           this._blockSize = blockSizeInBits/8;

       }

       public override void Initialize()
       {
           //
           // TODO: Initial UBI can be statically specified as per Appendix B
           //
         

           _initialized = true;
           //
           // Initialize some book keeping
           //
           _ubi = new UBI((UInt32)this.InputBlockSize);
           _blockBuffer= new byte[this.InputBlockSize];

           // Do first UBI with config
           // Per spec this is
           // K' := 0^Nb
           // G0 := UBI( K', C, Tcfg 2^120)
           //
           // Copy config into buffer (also zero-pads the rest of the buffer)
           //
           _config = EncodeConfig();
           Array.Copy(_config, _blockBuffer, _config.Length);
           _blockWords = Util.ConvertLSBFirstToUInt64(_blockBuffer);

           //
           // In Skein 1.0, there was a mistake in the reference impl. (and the test vectors)
           // where block size was used here instead of config size. This was fixed in 1.1's test vectors
           //
           Tweak tweak = Tweak.EncodeTweak((UInt64)_config.Length, false, TweakType.Configuration, true, true);
           _ubi.Update(_blockWords, tweak);
           

       }

     

       protected override void HashCore(byte[] data, int indexStart, int size)
       {
           if (!_initialized){
               Initialize();
           }
           int positionInInput = 0; // Index into input array
           int nextBlockSize = 0;
 
           
           while (positionInInput < indexStart + size){
               nextBlockSize = Math.Min(InputBlockSize, (indexStart+size)-positionInInput);
               

               //
               // Common case - we have a block that 'fits' our buffer
               //
               Array.Copy(data, positionInInput, _blockBuffer, 0, nextBlockSize);
               if (nextBlockSize != InputBlockSize){
                   //
                   // Pad out remaining part of block buffer
                   //
                   Array.Clear(_blockBuffer, nextBlockSize , InputBlockSize - nextBlockSize);
               }

               Util.ConvertLSBFirstToUInt64(_blockBuffer, ref _blockWords);
               //
               // Construct tweak value
               // Position = current position/bytes processed
               // First - whether the current no. of bytes processed is <= a full block
               // Final = whether we've finished all data
               //
               positionInInput += nextBlockSize; //update before tweak
               Tweak tweak = Tweak.EncodeTweak( (UInt64)(positionInInput-indexStart), false, TweakType.Message, 
                                                 positionInInput-indexStart<= InputBlockSize,
                                                ((indexStart+size)-positionInInput)==0 );

               _ubi.Update(_blockWords, tweak);
           }

       }

       protected override byte[] HashFinal()
       {
           UInt64[] output = new UInt64[this.InputBlockSize/sizeof(UInt64)];

           //
           // Output function 
           //
           // Output ( G, No) := UBI ( G, ToBytes(0,8), Tout 2^120) ||
           //                    UBI ( G, ToBytes(1,8), Tout 2^120) ||...
           //
           Tweak tweak;
           UInt32 outputBlocks = (_outputSize - 1) / (UInt32)InputBlockSize + 1;
           for (int i = 0; i < outputBlocks; i++){
               output[0] = (UInt64)i;
               tweak = Tweak.EncodeTweak(sizeof(UInt64), false, TweakType.Output, i == 0, i == outputBlocks - 1);
               _ubi.Update(output, tweak);
           }
           if (_ubi.CurrentOutputState.Length * sizeof(UInt64) > _outputSize){
               //
               // We don't want to return more bytes than asked for
               //
               UInt64[] outputSmallHash = new UInt64[_outputSize / sizeof(UInt64)];
               Array.Copy(_ubi.CurrentOutputState, outputSmallHash, outputSmallHash.Length);
               return Util.ConvertUInt64ToLSBFirst(outputSmallHash);
           }
           else{
               return Util.ConvertUInt64ToLSBFirst(_ubi.CurrentOutputState);
           }
       }



       internal byte[] EncodeConfig()
       {

           //
           // Returns a 32 byte array with configuration encoded as per spec.
           //
           // 0-4 Schema Identifier ('SHA3')
           // 4-6 Version Number (1)
           // 6-8 reserved (0)
           // 8-16 output length in bits (and not bytes)
           // 16-17 tree-leaf size
           // 17-18 tree leaf fan out
           // 18-19 max tree height
           // 19-32 reserved (0)
           // N.B we don't support tree mode so we don't take as many params as we should/
           //

           //
           // TODO: Have static versions for the known output sizes and intiail UBI per appendix b
           // TODO: Fix getbytes call with bit shift if it is faster than BitConverter 
           //(doubtful since BitConverter seems to do a direct assignment using unsafe code)
           // TODO: Make it work regardless of machine endianness (this doesn't work on non LSB machines now)
           //
           byte[] l = BitConverter.GetBytes((UInt64)this.OutputBlockSize*8);
           return new byte[32] { 
               (byte)'S', (byte)'H', (byte)'A', (byte)'3', // SHA3 string
               1,0,  // Version number (LSB)
               0,0, // reserved
               l[0], l[1], l[2], l[3], l[4],l[5],l[6],l[7], // 8 bytes of length
               0,0,0,  // No support for tree mode
               0,0,0,0,0,0,0,0,0,0,0,0,0 // Reserved
               };


       }
    }
}
