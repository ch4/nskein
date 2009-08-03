using System;
using nskein;

namespace nskeintest
{
    class Program
    {
        static void Main(string[] args) {
            RunThreeFishTests();
            RunSkeinTests();
        }

        private static void RunSkeinTests(){
            //
            // Run tests based on data from official submission.
            //

            Skein256Managed skein256_256 = new Skein256Managed(256);
            string input8 = "FF";
            string input256 = @"FF FE FD FC  FB FA F9 F8  F7 F6 F5 F4  F3 F2 F1 F0
                             EF EE ED EC  EB EA E9 E8  E7 E6 E5 E4  E3 E2 E1 E0";
            string input512 = @"FF FE FD FC  FB FA F9 F8  F7 F6 F5 F4  F3 F2 F1 F0
                             EF EE ED EC  EB EA E9 E8  E7 E6 E5 E4  E3 E2 E1 E0
                             DF DE DD DC  DB DA D9 D8  D7 D6 D5 D4  D3 D2 D1 D0
                             CF CE CD CC  CB CA C9 C8  C7 C6 C5 C4  C3 C2 C1 C0";
            string input1024 = @" FF FE FD FC  FB FA F9 F8  F7 F6 F5 F4  F3 F2 F1 F0
                                   EF EE ED EC  EB EA E9 E8  E7 E6 E5 E4  E3 E2 E1 E0
                                   DF DE DD DC  DB DA D9 D8  D7 D6 D5 D4  D3 D2 D1 D0
                                   CF CE CD CC  CB CA C9 C8  C7 C6 C5 C4  C3 C2 C1 C0
                                   BF BE BD BC  BB BA B9 B8  B7 B6 B5 B4  B3 B2 B1 B0
                                   AF AE AD AC  AB AA A9 A8  A7 A6 A5 A4  A3 A2 A1 A0
                                   9F 9E 9D 9C  9B 9A 99 98  97 96 95 94  93 92 91 90
                                   8F 8E 8D 8C  8B 8A 89 88  87 86 85 84  83 82 81 80";
            string input2048 = @"FF FE FD FC  FB FA F9 F8  F7 F6 F5 F4  F3 F2 F1 F0
                                EF EE ED EC  EB EA E9 E8  E7 E6 E5 E4  E3 E2 E1 E0
                                DF DE DD DC  DB DA D9 D8  D7 D6 D5 D4  D3 D2 D1 D0
                                CF CE CD CC  CB CA C9 C8  C7 C6 C5 C4  C3 C2 C1 C0
                                BF BE BD BC  BB BA B9 B8  B7 B6 B5 B4  B3 B2 B1 B0
                                AF AE AD AC  AB AA A9 A8  A7 A6 A5 A4  A3 A2 A1 A0
                                9F 9E 9D 9C  9B 9A 99 98  97 96 95 94  93 92 91 90
                                8F 8E 8D 8C  8B 8A 89 88  87 86 85 84  83 82 81 80
                                7F 7E 7D 7C  7B 7A 79 78  77 76 75 74  73 72 71 70
                                6F 6E 6D 6C  6B 6A 69 68  67 66 65 64  63 62 61 60
                                5F 5E 5D 5C  5B 5A 59 58  57 56 55 54  53 52 51 50
                                4F 4E 4D 4C  4B 4A 49 48  47 46 45 44  43 42 41 40
                                3F 3E 3D 3C  3B 3A 39 38  37 36 35 34  33 32 31 30
                                2F 2E 2D 2C  2B 2A 29 28  27 26 25 24  23 22 21 20
                                1F 1E 1D 1C  1B 1A 19 18  17 16 15 14  13 12 11 10
                                0F 0E 0D 0C  0B 0A 09 08  07 06 05 04  03 02 01 00";


            string expected256_256 = @" A4 7B E7 1A  18 5B A0 AF  82 0B 3C E8  45 A3 D3 5A
                                        80 EC 64 F9  6A 0D 6A 36  E3 F5 36 36  24 D8 A0 91";
            RunSpecificSkeinTest(skein256_256, input8, expected256_256);

            skein256_256 = new Skein256Managed(256);
            
            expected256_256 = @"CC 2D A8 2F  39 73 C2 F7  A8 CE D0 BB  B5 4A A0 28
                                EC AF 6B 59  11 62 8D 0F  FA BB 20 08  E4 11 D1 71";
            RunSpecificSkeinTest(skein256_256, input256, expected256_256);

            skein256_256 = new Skein256Managed(256);
            expected256_256 = @"FA 1A 76 2B  6B 1C 72 B7  0D 52 92 63  53 E1 0E B8
                                FB 0E DD 73  13 DA 20 A2  41 31 80 B8  E2 89 B8 72";
            RunSpecificSkeinTest(skein256_256, input512, expected256_256);

            Skein512Managed skein512_512 = new Skein512Managed(512);                   
            string expected512_512=  @"8F CA 8D 27  05 F9 9A 56  90 43 08 A4  00 4C 64 EF
                                       B6 68 81 8B  58 B0 89 5B  F7 29 6A 2C  5A 54 F9 30
                                       14 83 D6 22  C4 A5 AE C8  55 AC 30 08  7E 1E B0 E8
                                       39 40 90 6E  7B 05 5D 70  D4 46 C8 D2  85 F2 7F 01";
            RunSpecificSkeinTest(skein512_512, input8, expected512_512);

            expected512_512 = @"0F C4 2E 10  0B 2C D0 B0  C6 9F 39 38  3F 9D 2D 17
                                AF 6C F7 4E  2A A8 D4 E2  D9 1C BF 94  A5 99 35 A3
                                12 3B 7F 92  50 F9 82 22  4B F0 C3 E1  90 BE 10 AB
                                 41 AD D8 C1  E3 5C BE C4  B1 B3 C3 5D  BB A5 86 9C";

            RunSpecificSkeinTest(skein512_512, input512, expected512_512);

            string expected512_256 = @"9F A2 41 15  FF 41 C8 47  EB 69 40 BF  2E 8B CC C4
                                09 7B 57 F7  F8 F0 02 87  A8 AB 8A D7  75 80 AF 80";

            RunSpecificSkeinTest(new Skein512Managed(256), input1024, expected512_256);


            string expected512_384 = @"E4 6B CB 15  4E FD B0 F2  DC 30 CB 6E  B0 CC 52 52
                                        F1 35 C8 DB  65 9C 34 22  99 0F 4D 9D  B6 94 B1 B8
                                        92 DD 74 9B  A2 18 34 33  53 64 59 74  D9 13 A4 A0";

            RunSpecificSkeinTest(new Skein512Managed(384), input1024, expected512_384);

            expected512_512 = @"0F 01 9E 7C  18 49 16 7C  EC B9 A0 D8  F1 B0 0C CD
                                5B 14 15 9C  5A AE E4 49  DA B5 5A 1B  C6 C8 51 03
                                E8 37 84 54  91 2E 46 AF  63 06 79 50  F8 63 04 3C
                                CF A3 69 98  87 A2 57 73  37 CA A6 65  31 BD BF 9E";
            
            RunSpecificSkeinTest(skein512_512, input1024, expected512_512);

            string expected1024_1024 = @"2F 2E 87 C1  4F 43 11 28  E2 69 B2 3B  45 BE B3 64
                                         B6 37 84 53  17 7C 44 1B  B6 AF 57 8A  86 FC 0F C9
                                         AD BE 93 5F  8E 73 B0 0E  04 E9 AF 05  4E 3C B4 87
                                         E8 74 2D 76  79 CF 6A 84  26 1F 6B D7  C8 BC 1D 71
                                         D5 0A 97 D9  22 70 1C 6E  F4 C2 9C 52  76 4E CC 6A
                                         EC 45 57 CC  4C 51 16 9A  CF 0A 4B FA  B4 57 5D E9
                                         CB 81 64 4A  E6 86 8B 49  98 69 06 57  13 5A 6C C1
                                            EC FE 6C 8E  1F 9F A7 29  E3 FF 5E E5  5D 9C E7 78";

            RunSpecificSkeinTest(new Skein1024Managed(1024), input8, expected1024_1024);

            string expected1024_384 = @"D7 80 B3 95  33 E7 D2 93  6A B1 02 2A  DF BD 3D 78
                                        3E AD 03 47  60 93 FE E2  3E 7F 9C FB  EA 78 8B 41
                                        D8 D1 9A 36  21 FE 1F B3  C8 14 D8 DD  F3 3D 44 E1";

            RunSpecificSkeinTest(new Skein1024Managed(384), input1024, expected1024_384);

            string expected1024_512 = @"E1 EE AE C3  8B 77 1D 8F  24 59 54 F8  A3 0A 7E 12
                                        B5 59 B3 23  13 D9 E7 BA  4C 9E 4C 5B  02 9D 3D 5D
                                        12 F8 D9 6A  6B C7 F7 56  B4 86 2F EB  64 C4 66 DD
                                        BE 3D 5D B1  68 BD D0 3C  56 6C 70 BD  27 6F 94 54";

            RunSpecificSkeinTest(new Skein1024Managed(512), input1024, expected1024_512);

             expected1024_1024 = @"6D 8F D0 3D  AA 40 6B 2F  E6 D3 6E CE  F4 EE 1A FE
                                         37 D2 2D 7C  59 B7 67 CF  87 1A 75 20  15 09 58 E0
                                         2D E7 CB 36  BF 4D 13 DC  44 D0 5F 32  DB C5 D2 A2
                                         B4 55 7D 77  46 83 9F 09  BC F6 96 81  3A A6 09 BE
                                            9F CF 83 9D  24 24 1A A6  4C 1C 1F B7  D9 C9 D8 4C
                                        85 ED A0 CA  19 C4 44 3E  19 4F 98 A2  22 AA 17 64
                                        58 94 EE BD  F6 A3 1B 6F  1D 13 C3 B6  0D CB BB 1D
                                    BF 46 91 5D  D4 EB AB 25  D8 1F 85 57  6B CB 0D 07";

            RunSpecificSkeinTest(new Skein1024Managed(1024), input1024, expected1024_1024);

            expected1024_1024 = @"36 1F 0F C0  76 52 BB C3  93 C7 C6 02  8E A3 52 35
                                    63 90 10 B8  0B 25 6D 84  F6 66 2C 34  2A 65 FF A6
                                    47 50 78 A1  98 A2 29 43  6F 27 0B 10  20 DE F5 E6
                                    47 FE 9A 43  CC 7E C3 93  CD 47 AF 1A  01 F3 D7 B4
                                    8A 45 52 06  92 95 92 92  00 32 50 FA  AB 40 CB 81
                                    59 9E 57 D8  3D 8F 73 76  00 A0 E7 BF  26 0D 83 00
                                    D9 F9 5D FC  C8 28 5D 3C  16 32 7D 36  89 4D AD DF
                                    64 A6 C3 E1  FF 29 3E E3  A2 94 3E 48  6F 86 F8 3B";

            RunSpecificSkeinTest(new Skein1024Managed(1024), input2048, expected1024_1024);
          
        }

        private static void RunSpecificSkeinTest(SimpleSkeinManaged skein, string message, string expectedOutput) {

            message = message.Replace(" ", "").Replace("\r\n", "");
            expectedOutput = expectedOutput.Replace(" ", "").Replace("\r\n", "");
            byte[] input = Util.ByteArrayfromHex(message);
            byte[] output = skein.ComputeHash(input);

            string outputString = Util.HexStringFromByteArray(output);
            if (String.Compare(outputString.ToLower(), expectedOutput.ToLower(), true) != 0)
            {
                System.Diagnostics.Debugger.Break();
                Console.WriteLine("\n!!Test failed!! \n Message:{0} \n Expected:{1} \n Got:{2}", message, expectedOutput, outputString);
            }

        }

        private static void RunThreeFishTests()
        {

            // Test data from http://bartosz.malkowski.googlepages.com/threefish

            ThreefishTest("0000000000000000000000000000000000000000000000000000000000000000",
                "00000000000000000000000000000000",
                "0000000000000000000000000000000000000000000000000000000000000000",
                "e39756f9f3b6cf3ff91d2bc3d324ce618574ea1623b2367f88382e2a93afa858", Threefish.ThreeFishBlockSize.BLOCKSIZE_256_BITS);


            ThreefishTest("101112131415161718191a1b1c1d1e1f202122232425262728292a2b2c2d2e2f",
                "000102030405060708090a0b0c0d0e0f",
                "FFFEFDFCFBFAF9F8F7F6F5F4F3F2F1F0EFEEEDECEBEAE9E8E7E6E5E4E3E2E1E0",
                "1e9b8f641bed9511be4f40df57c3d7a1bc42718edd7af7139b3d4c52b2a920f8", Threefish.ThreeFishBlockSize.BLOCKSIZE_256_BITS);

            ThreefishTest("00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000",
                "00000000000000000000000000000000",
                "00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000",
                "408be942494492eab19daa3e96ad19aedfc41f4e55f8a2626c1e46d54547a713d43b21f0de1a10881ed5c4adefdad1c4172cd768c8fc28d0dde9df018042fe3e", Threefish.ThreeFishBlockSize.BLOCKSIZE_512_BITS
            );


            ThreefishTest("101112131415161718191a1b1c1d1e1f202122232425262728292a2b2c2d2e2f303132333435363738393a3b3c3d3e3f404142434445464748494a4b4c4d4e4f",
                "000102030405060708090a0b0c0d0e0f",
                "fffefdfcfbfaf9f8f7f6f5f4f3f2f1f0efeeedecebeae9e8e7e6e5e4e3e2e1e0dfdedddcdbdad9d8d7d6d5d4d3d2d1d0cfcecdcccbcac9c8c7c6c5c4c3c2c1c0",
                "869ae12210e51d3b0736399f2acb400de230600b13e62f1df7596a146232d281dfbf127a65571b9a798906c719678394c50d995138fd83f2bfa54a3bc350d2f0", Threefish.ThreeFishBlockSize.BLOCKSIZE_512_BITS);

            ThreefishTest(
            "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000",
            "00000000000000000000000000000000",
            "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000",
            "43CF2A34CB1668E38C2E19EA1757D6B31AC6DEAD02FEA99459D8A0331BDC7273A1F7E9495D60402D1F8B43E48A5AC4F9D9D30965835E07F5455B87F963FDBCA6DF66B4446B91FFDD27634573F6E0E4C19633CF80DA8FE11B890BCF639AC67B347F87C5DAA1ACC1B8CD0303F4A9168C0B9B7B78BAA6FC68DB2CBD3337B8519170",
             Threefish.ThreeFishBlockSize.BLOCKSIZE_1024_BITS);

            ThreefishTest(
            "101112131415161718191a1b1c1d1e1f202122232425262728292a2b2c2d2e2f303132333435363738393a3b3c3d3e3f404142434445464748494a4b4c4d4e4f505152535455565758595a5b5c5d5e5f606162636465666768696a6b6c6d6e6f707172737475767778797a7b7c7d7e7f808182838485868788898a8b8c8d8e8f",
            "000102030405060708090a0b0c0d0e0f",
            "fffefdfcfbfaf9f8f7f6f5f4f3f2f1f0efeeedecebeae9e8e7e6e5e4e3e2e1e0dfdedddcdbdad9d8d7d6d5d4d3d2d1d0cfcecdcccbcac9c8c7c6c5c4c3c2c1c0bfbebdbcbbbab9b8b7b6b5b4b3b2b1b0afaeadacabaaa9a8a7a6a5a4a3a2a1a09f9e9d9c9b9a999897969594939291908f8e8d8c8b8a89888786858483828180",
            "44E66B3125AA434261ADBEF4C310101CEF1D185272B43132D6E67E75692B2851A7E3ACF8DFD3D6C6DFEA2724150D287E8C2BABFF27E971FAC163781BE181F2BF572AB84862259EF8FA628A77A61D128F2F1517BD518592FE9382FF670D848ADAB31582DCCF36C2C607153AAE34A2853FF0C14CF462C903CC2860734AE50C04B1",
      Threefish.ThreeFishBlockSize.BLOCKSIZE_1024_BITS);
        }

        static bool ThreefishTest(string hexKey, string hexTweak, string hexMessage, string hexExpectedCipherText, Threefish.ThreeFishBlockSize blockSize){

            UInt64[] plainText = Util.ConvertLSBFirstToUInt64(Util.ByteArrayfromHex(hexMessage));
            UInt64[] key = Util.ConvertLSBFirstToUInt64(Util.ByteArrayfromHex(hexKey));
            UInt64[] tweak = Util.ConvertLSBFirstToUInt64(Util.ByteArrayfromHex(hexTweak));
            UInt64[] cipherText = new UInt64[plainText.Length];
            Threefish threefish = new Threefish(key, new Tweak { low64 = tweak[0], high64 = tweak[1] }, blockSize);
            threefish.Encrypt(plainText, cipherText);
            string hexObtainedCipherText = Util.HexStringFromByteArray(Util.ConvertUInt64ToLSBFirst(cipherText));
            if (String.Compare(hexObtainedCipherText.ToLower(), hexExpectedCipherText.ToLower(), true) != 0)
            {
                System.Diagnostics.Debugger.Break();
                Console.WriteLine("\n\n!!!Test failed!!!");
                Console.WriteLine(" \n Key: {0} \n Tweak: {1} \n Message: {2} \n ExpectedCipher: {3}\n GotCipher: {4}", hexKey, hexTweak, hexMessage, hexExpectedCipherText, hexObtainedCipherText);
                return false;
            }

            return true;
            

        }
    }
}
