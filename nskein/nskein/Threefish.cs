using System;
using System.Security.Cryptography;

namespace nskein
{
    public class Threefish
    {
        //
        //TODO: Perf sucks right now. The reference impl. precomputes the key schedule and unrolls this loop
        // It also avoids memory allocations as much as possible whereas we allocate arrays all over the place
        //

        public enum ThreeFishBlockSize
        {
            BLOCKSIZE_256_BITS = 32,
            BLOCKSIZE_512_BITS = 64,
            BLOCKSIZE_1024_BITS = 128
        }

        private enum ThreeFishRounds
        {
            ROUNDS_72 = 72,
            ROUNDS_80 =80
        }

        //
        // Define word permutation Pi(i) for various Nw as per spec (Table 3)
        //
        private static byte[] PI_I_Nw_4 = new byte[4]{ 0, 3, 2, 1 };
        private  static byte[] PI_I_Nw_8 = new byte[8] { 2, 1, 4, 7, 6, 5, 0, 3 };
        private  static byte[] PI_I_Nw_16 = new byte[16] { 0, 9, 2, 13, 6, 11, 4, 15, 10, 7, 12, 3, 14, 5, 8, 1 };

        private byte[] PermutationPi { get; set; }
        //
        //Rotation constants Rd,j for each Nw as per spec (Table 4)
        //

        private static int[][] Rdj_Nw_4 = {
                                      new int[]{5,56},
                                      new int[]{36,28},
                                      new int[]{13,46},
                                      new int[]{58,44},
                                      new int[]{26,20},
                                      new int[]{53,35},
                                      new int[]{11,42},
                                      new int[]{59,50}
                                    };

        private static int[][] Rdj_Nw_8 = {

                                             new int[]{38,30,50,53},
                                             new int[]{48,20,43,31},
                                             new int[]{34,14,15,27},
                                             new int[]{26,12,58,7},
                                             new int[]{33,49,8,42},
                                             new int[]{39,27,41,14},
                                             new int[]{29,26,11,9},
                                             new int[]{33,51,39,35},
                                         };

        private static int[][] Rdj_Nw_16 = {

                                              new int[]{55,43,37,40,16,22,38,12},
                                              new int[]{25,25,46,13,14,13,52, 57},
                                              new int[]{33,8,18,57,21,12,32,54},
                                              new int[]{34, 43,25,60, 44,9,59,34},
                                              new int[]{28,7,47,48,51,9,35,41},
                                              new int[]{17,6,18,25,43,42,40,15},
                                              new int[]{58, 7, 32,45,19,18,2,56},
                                              new int[]{47,49,27,58,37,48,53,56}


                                          };

        private int[][] RotationConstants = null;


        public  ThreeFishBlockSize BlockSize { get; set; }

        //
        // Key as  UInt64 words
        //
    
        public UInt64[] KeyWords { get; set; }

       

        //
        // No. of rounds for the cipher. Function of block size
        //
        private ThreeFishRounds  _rounds = 0;

        //
        // No. of words in key/plain text. Nw from the paper
        //
        private int _numWords = 0;

        //
        // Vd, Ed, fd and Ksd from paper
        //

        private UInt64[] _vd;
        private UInt64[] _ed;
        private UInt64[] _fd;
        private UInt64[] _kdi;

        //
        // Ts from paper (t0, t1 and t2)
        //

        private UInt64[] _ts = { 0, 0, 0 };

        //
        // kNw from paper. Set to 2^64/3
        //
        private UInt64 _kNw =  0x5555555555555555L;


        public Threefish(UInt64[] key, Tweak tweak, ThreeFishBlockSize blockSize)
        {
            //
            // TODO: Move non-onetime initialization so that consumers can cache instances
            //
            if (key.Length != (int)blockSize/8) {
                throw new ArgumentException("Key size should match block size");
            }

            
            this.BlockSize = blockSize;

            //
            // Set number of words rounds based on block size
            //

            _numWords = (int)BlockSize / sizeof(UInt64);

            switch (BlockSize) {

                case ThreeFishBlockSize.BLOCKSIZE_256_BITS:
                    this._rounds = ThreeFishRounds.ROUNDS_72;
                    this.RotationConstants = Rdj_Nw_4;
                    this.PermutationPi = Threefish.PI_I_Nw_4;
                    break;
                case ThreeFishBlockSize.BLOCKSIZE_512_BITS:
                    this._rounds = ThreeFishRounds.ROUNDS_72;
                    this.RotationConstants = Rdj_Nw_8;
                    this.PermutationPi = Threefish.PI_I_Nw_8;
                    break;
                case ThreeFishBlockSize.BLOCKSIZE_1024_BITS:
                    this._rounds = ThreeFishRounds.ROUNDS_80;
                    this.RotationConstants = Rdj_Nw_16;
                    this.PermutationPi = Threefish.PI_I_Nw_16;
                    break;
            }

     
          

            //
            // KeyWords is special in that it has an extra element kNw at the end
            //
            this.KeyWords = new UInt64[key.Length + 1];
            Array.Copy(key, KeyWords, key.Length) ;
            

            
            //
            // Initialize Vd, Ed, fd and Ksd
            //

            _vd = new UInt64[_numWords];
            _ed = new UInt64[_numWords];
            _kdi = new UInt64[_numWords];
            _fd = new UInt64[_numWords];

            //
            // Initialize ts values(as per section 3.3.2)
            //
            _ts[0] = tweak.low64;
            _ts[1] = tweak.high64;
            _ts[2] = _ts[0] ^ _ts[1];
            
            //
            // Initialize kNw (as per section 3.3.2). Already set to 2^64/3
            //

            for (int i = 0; i < _numWords; i++) {
                _kNw ^= KeyWords[i];
            }
            KeyWords[_numWords] = _kNw;

        }

        public void Encrypt(UInt64[] plainText, UInt64[] cipherText) {

            //
            // Core workhorse where actual encryption gets done. We ask the caller to pass
            // the ciphertext byte array so that they have an easier time of reusing it between calls
            //
   

            //
            // Init v0,i = pi for i=0,...Nw-1
            //
            Array.Copy(plainText, _vd, _numWords);

            //
            //Loop correct number of rounds
            //
            for (UInt32 d = 0; d < (int)_rounds; d++) {

                //
                // ed,i := (vd,i + kd/4,i) mod 2^64 if d mod 4==0
                //      := (vd,i) otherwise
                //

                if (d % 4 == 0) {

                    SetSubKeyPerSchedule(d);
                    for (int i = 0; i < _numWords; i++) {
                        _ed[i] = _vd[i] + _kdi[i];
                    }
                }
                else {
                    Array.Copy(_vd, _ed, _vd.Length) ;
                }


                //
                //  y0,y1 for the Mix function. Why can't .NET return tuples yet?
                //
                
                for (UInt32 j = 0; j < (_numWords / 2); j++) {
                    UInt64 y0, y1;
                    Mix(_ed[j * 2], _ed[j * 2 + 1], j, d, out y0, out y1);

                    //
                    // (fd,2j, fd,2j+1) := MIXd,j(ed,2j, ed,2j+1)
                    //
                    _fd[j * 2] = y0;
                    _fd[j * 2 + 1] = y1;
                }

                for (int i = 0; i < _numWords; i++) {
                    //
                    // vd+1,i := fd, pi(i)
                    _vd[i] = _fd[this.PermutationPi[i]];
                }

            }

            //
            // Final schedule
            //
            SetSubKeyPerSchedule((UInt32)_rounds);

            for (int i = 0; i < _numWords; i++) {
                //
                // ci :=  (vNri + kNr/4, i) mod 2^64 for i=0,..., Nw-1
                //
                cipherText[i] = _vd[i] + _kdi[i];
            }

        }

        private void Mix(UInt64 x0, UInt64 x1, UInt32 j, UInt32 d, out UInt64 y0, out UInt64 y1) {

            //
            // Mix d,j as defined by section 3.3.1. It is defined as 
            //
            // y0 := ( x0 +x1) mod 2^64
            // y1 := ( x1 <<< R (d mod 8) j xor y0
            //

            y0 = x0 + x1;
            int rotation = RotationConstants[ d%8][j];

            //
            // Left rotate our UInt64. 
            //

            y1=  (x1 <<rotation) | x1 >> (sizeof(UInt64)*8 - rotation);
            y1 ^= y0;


        }


        private void SetSubKeyPerSchedule(UInt32 currentRound) {

            //
            //
            // Implementation of core key schedule. Takes current round as input
            // and sets ks,i
            //

            if (currentRound % 4 != 0) {
                throw new ArgumentException("Key schedule only on every 4th round");
            }

            UInt32 s = currentRound / 4;

            for (int i = 0; i < _numWords; i++) {

                //
                //
                // First, fill in with key material. This expression is common to all
                // i. This will take care of
                //ks,i := k(s+1) mod (Nw +1) for i=0,...,Nw-4
                //

                _kdi[i] = KeyWords[(s+i) % (_numWords +1)];

                //
                // ks,i := k(s+1) mod (Nw +1) + ts mod 3 for i=Nw-3
                //

                if (i == _numWords - 3) {
                    _kdi[i] = _kdi[i] + _ts[ s % 3] ;
                }

                //
                // ks,i := k(s+1) mod (Nw +1) + ts+1 mod 3 for i=Nw-2
                //
                if (i == _numWords - 2) {
                    _kdi[i] = _kdi[i] + _ts[(s+1) % 3];
                }

                //
                // ks,i := k(s+1) mod (Nw +1) + s for i=Nw-1
                //
                if (i == _numWords - 1) {
                    _kdi[i] = _kdi[i] + s;
                } 

            }

        }



    }
}
