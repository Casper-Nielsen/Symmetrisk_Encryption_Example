using System;
using System.Collections.Generic;
using System.Text;

namespace Symmetrisk_Encryption_Example
{
    class EncryptionType
    {
        public string Name { get; set; }
        public EncryptionTypeEnum Type { get; set; }
        public int KeySize { get; set; }
    }

    enum EncryptionTypeEnum
    {
        
                AES = 0,
                DES = 1,
                TripleDES = 2,
                Rijndael = 3
    }
}
