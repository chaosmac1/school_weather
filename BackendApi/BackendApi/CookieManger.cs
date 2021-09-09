using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using BackendApi.Ulitis;

namespace BackendApi {
    public static class CookieManger {
        private static readonly long MaxAgeTicks = TimeSpan.TicksPerDay;
        private static object LockObject;
        private static List<(long createDay, string key)> Buffer = new List<(long createDay, string key)>(10);
        
        public static void Push(string key) {
            lock (LockObject) {
                Buffer.Add((DateTime.Now.Ticks, key));
                UpdateBuffer();   
            }
        }

        public static bool keyExist(string key) {
            lock (LockObject) {
                UpdateBuffer();
                foreach (var i in Buffer) { if (i.key == key) return true; }
                return false;
            }
        } 

        private static void UpdateBuffer() {
            var nowTicks = DateTime.Now.Ticks;
            
            for (int i = Buffer.Count - 1; i >= 0; i--) {
                var cookie = Buffer[i];
                if (cookie.createDay + MaxAgeTicks > nowTicks) continue;
                Buffer.RemoveAt(i);
            }
        }
    }
}