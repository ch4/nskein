using System;
using System.Diagnostics;
namespace nskein
{
    public class Util
    {
        public static UInt64[] ConvertLSBFirstToUInt64(byte[] input){

            UInt64[] output = new UInt64[input.Length / sizeof(UInt64)];
            ConvertLSBFirstToUInt64(input, ref output);
            return output;
        }

        public static void ConvertLSBFirstToUInt64(byte[] input, ref UInt64[] output){

            //
            // Takes an array of LSB-first bytes and converts them into UInt64
            //


            if (input.Length/sizeof(UInt64) != output.Length)
            {
                throw new ArgumentException("Output UInt64 array not of right size.");
            }
            
            int sizeUInt64 = sizeof(UInt64);
            Debug.Assert(input.Length % sizeUInt64 == 0);
            if (input.Length % sizeUInt64 != 0)
            {
                throw new ArgumentException("Input LSB array not of right size. Did you forget to zero pad?");
            }
          
            for (int i = 0; i <= (input.Length - sizeUInt64); i += sizeUInt64)
            {
                //
                // TODO: This doesn't work on BigEndian machines
                //
                Debug.Assert(BitConverter.IsLittleEndian);
                if (!BitConverter.IsLittleEndian)
                {
                    throw new NotImplementedException("Not implemented for big endian machines yet");
                }
                output[i / sizeUInt64] = BitConverter.ToUInt64(input, i);
            }

        }

        public static byte[] ConvertUInt64ToLSBFirst(UInt64[] input){
            //
            // Takes an array of unsigned 64 bit integers and converts them into a 
            // a byte array with LSB first
            //

            byte[] output = new byte[input.Length * sizeof(UInt64)];
            for(int i=0;i<input.Length;i++) {
                Array.Copy(BitConverter.GetBytes(input[i]),0 ,output, i*sizeof(UInt64), sizeof(UInt64));
            }
            return output;
        }

        public static byte[] ZeroPad(byte[] input, int blockSizeInBytes) {

            //
            // Takes a block size and pads it until it becomes a multiple of blockSizeInBytes
            //

            if (input.Length % blockSizeInBytes == 0) {
                //
                // Already correct length - nothing for us to do
                //
                return input;
            }

            int finalBlocks = (input.Length - 1) / blockSizeInBytes + 1;

            byte[] output = new byte[finalBlocks * blockSizeInBytes];

            //
            // .NET makes sure arrays are initialized to 0. All that's left is to
            // copy old data over
            //

            Array.Copy(input, output, input.Length);
            return output;

        }

      
        public static byte[] ByteArrayfromHex(String hexString) {
            //
            // Takes an input string consisting of hex characters and returns a byte array
            //
		byte[] result = new byte[hexString.Length / 2];
		for (int i = 0; i < result.Length; i++) {
			result[i] = (byte) Convert.ToInt32(hexString.Substring(i * 2, 2), 16);
		}
		return result;
	}

        public static string HexStringFromByteArray(byte[] data){        
            //
            // Takes a byte array and returns a hex string as output
            // 
            string output = String.Empty;
            foreach(byte b in data) {
                output += String.Format("{0:X2}", b);
            }
	        return output;
        }


    }
}
