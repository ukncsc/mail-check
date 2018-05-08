using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Utilities.Encoders;

namespace Dmarc.Common.Tls.BouncyCastle.Mapping
{
    public class AdditionalGroups
    {
        private static BigInteger FromHex(string hex)
        {
            return new BigInteger(1, Hex.Decode(hex));
        }

        private static DHParameters FromPg(string hexP, string hexG)
        {
            return new DHParameters(FromHex(hexP), FromHex(hexG));
        }

        private static readonly string Java1024P = "FD7F53811D75122952DF4A9C2EECE4E7F611B7523CEF4400" +
                                                   "C31E3F80B6512669455D402251FB593D8D58FABFC5F5BA30" +
                                                   "F6CB9B556CD7813B801D346FF26660B76B9950A5A49F9FE8" +
                                                   "047B1022C24FBBA9D7FEB7C61BF83B57E7C6A8A6150F04FB" +
                                                   "83F6D3C51EC3023554135A169132F675F3AE2B61D72AEFF2" +
                                                   "2203199DD14801C7";

        private static readonly string Java1024G = "F7E1A085D69B3DDECBBCAB5C36B857B97994AFBBFA3AEA82" +
                                                   "F9574C0B3D0782675159578EBAD4594FE67107108180B449" +
                                                   "167123E84C281613B7CF09328CC8A6E13C167A8B547C8D28" +
                                                   "E0A3AE1E2BB3A675916EA37F0BFA213562F1FB627A01243B" +
                                                   "CCA4F1BEA8519089A883DFE15AE59F06928B665E807B5525" +
                                                   "64014C3BFECF492A";

        public static readonly DHParameters Java1024 = FromPg(Java1024P, Java1024G);
    }
}