using System;
using BackendApi.Ulitis;

namespace BackendApi.Cookie {
    public static class CookieBuilder {
        private const string Seal = "daspoidwajlkadslkladjsdasjlkwoejkwopek";
        
        public static string Build() {
            return Sha3.GetSha3Utf8(DateTime.Now.Ticks + Seal);
        }
    }
}