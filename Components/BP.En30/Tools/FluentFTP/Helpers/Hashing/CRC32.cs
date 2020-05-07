namespace FluentFTP.Helpers.Hashing
{
    using System;
    using System.Security.Cryptography;

    internal class CRC32 : HashAlgorithm
    {
        public const uint DefaultPolynomial = 0xedb88320;
        public const uint DefaultSeed = uint.MaxValue;
        private static uint[] defaultTable;
        private uint hash;
        private readonly uint seed;
        private readonly uint[] table;

        public CRC32()
        {
            this.table = InitializeTable(0xedb88320);
            this.seed = uint.MaxValue;
            this.Initialize();
        }

        public CRC32(uint polynomial, uint seed)
        {
            this.table = InitializeTable(polynomial);
            this.seed = seed;
            this.Initialize();
        }

        private static uint CalculateHash(uint[] table, uint seed, byte[] buffer, int start, int size)
        {
            uint num = seed;
            for (int i = start; i < size; i++)
            {
                num = (num >> 8) ^ table[buffer[i] ^ (num & 0xff)];
            }
            return num;
        }

        public static uint Compute(byte[] buffer)
        {
            return ~CalculateHash(InitializeTable(0xedb88320), uint.MaxValue, buffer, 0, buffer.Length);
        }

        public static uint Compute(uint seed, byte[] buffer)
        {
            return ~CalculateHash(InitializeTable(0xedb88320), seed, buffer, 0, buffer.Length);
        }

        public static uint Compute(uint polynomial, uint seed, byte[] buffer)
        {
            return ~CalculateHash(InitializeTable(polynomial), seed, buffer, 0, buffer.Length);
        }

        protected override void HashCore(byte[] array, int ibStart, int cbSize)
        {
            this.hash = CalculateHash(this.table, this.hash, array, ibStart, cbSize);
        }

        protected override byte[] HashFinal()
        {
            byte[] buffer = this.UInt32ToBigEndianBytes(~this.hash);
            base.HashValue = buffer;
            return buffer;
        }

        public override void Initialize()
        {
            this.hash = this.seed;
        }

        private static uint[] InitializeTable(uint polynomial)
        {
            if ((polynomial == 0xedb88320) && (defaultTable != null))
            {
                return defaultTable;
            }
            uint[] numArray = new uint[0x100];
            for (int i = 0; i < 0x100; i++)
            {
                uint num2 = (uint) i;
                for (int j = 0; j < 8; j++)
                {
                    if ((num2 & 1) == 1)
                    {
                        num2 = (num2 >> 1) ^ polynomial;
                    }
                    else
                    {
                        num2 = num2 >> 1;
                    }
                }
                numArray[i] = num2;
            }
            if (polynomial == 0xedb88320)
            {
                defaultTable = numArray;
            }
            return numArray;
        }

        private byte[] UInt32ToBigEndianBytes(uint x)
        {
            return new byte[] { ((byte) ((x >> 0x18) & 0xff)), ((byte) ((x >> 0x10) & 0xff)), ((byte) ((x >> 8) & 0xff)), ((byte) (x & 0xff)) };
        }

        public override int HashSize
        {
            get
            {
                return 0x20;
            }
        }
    }
}

