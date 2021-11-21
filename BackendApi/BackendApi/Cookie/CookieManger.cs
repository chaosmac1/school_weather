using System;
using System.Collections.Generic;

namespace BackendApi.Cookie {
    public static class CookieManger {
        private static readonly long MaxAgeTicks = TimeSpan.TicksPerDay;
        private static readonly List<(long createDay, string key)> Buffer = new(10);

        public static void Push(string key) {
            lock (Buffer) {
                Buffer.Add((DateTime.Now.Ticks, key));
                UpdateBuffer();
            }
        }

        public static bool KeyExist(string key) {
            lock (Buffer) {
                UpdateBuffer();
                foreach (var i in Buffer)
                    if (i.key == key)
                        return true;
                return false;
            }
        }

        private static void UpdateBuffer() {
            var nowTicks = DateTime.Now.Ticks;

            for (var i = Buffer.Count - 1; i >= 0; i--) {
                var cookie = Buffer[i];
                if (cookie.createDay + MaxAgeTicks > nowTicks) continue;
                Buffer.RemoveAt(i);
            }
        }
    }
}