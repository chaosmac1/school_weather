using System;
using System.Text;
using Org.BouncyCastle.Crypto.Digests;

namespace LibAllOver {
    public static class Sha3 {
        public static string GetSha3Ascii(string msg) {
            var hashAlgo = new Sha3Digest(512);

            var input = Encoding.ASCII.GetBytes(msg);

            hashAlgo.BlockUpdate(input, 0, input.Length);

            var result = new byte[512 / 8];
            hashAlgo.DoFinal(result, 0);

            return BitConverter.ToString(result).Replace("-", "").ToLowerInvariant();
        }

        public static string GetSha3Utf8(string msg) {
            var hashAlgo = new Sha3Digest(512);

            var input = Encoding.UTF8.GetBytes(msg);

            hashAlgo.BlockUpdate(input, 0, input.Length);

            var result = new byte[512 / 8];
            hashAlgo.DoFinal(result, 0);

            return BitConverter.ToString(result).Replace("-", "").ToLowerInvariant();
        }

        public static string GetSha3Byte(byte[] input) {
            var hashAlgo = new Sha3Digest(512);

            hashAlgo.BlockUpdate(input, 0, input.Length);

            var result = new byte[512 / 8];
            hashAlgo.DoFinal(result, 0);

            return BitConverter.ToString(result).Replace("-", "").ToLowerInvariant();
        }
    }
}