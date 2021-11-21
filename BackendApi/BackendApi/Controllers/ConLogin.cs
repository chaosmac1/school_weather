using System;
using BackendApi.DataBase;
using BackendApi.DataBase.Type;
using BackendApi.Ulitis;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace BackendApi.Controllers {
    [ApiController]
    [Route("[controller]")]
    public class ConLogin : ControllerBase {
        private static readonly string DefaultUsernameHash = Sha3.GetSha3Utf8("root");
        private static readonly string DefaultPasswordHash = Sha3.GetSha3Utf8("root");
        private readonly ILogger<ConLogin> _logger;

        public ConLogin(ILogger<ConLogin> logger) {
            _logger = logger;
        }

        [HttpPost("login")]
        public TsCookie Login(string? username, string? passwd) {
            if (string.IsNullOrEmpty(username)) return new TsCookie {Error = true, ErrorMsg = "Username is Null Or Empty"};
            if (string.IsNullOrEmpty(passwd)) return new TsCookie {Error = true, ErrorMsg = "Password is Null Or Empty"};

            static TsCookie TsCookie500() => new() {Error = true, ErrorMsg = "500"};
            
            static TsCookie TsCookie500Err(string msg) {
#if DEBUG
                throw new Exception(msg);
#else
                return TsCookie500();
#endif
            }

            if (!GetAccount(out var acc))
                return TsCookie500Err("acc not found");

            if (acc is null && !CreateDefaultAcc())
                return TsCookie500Err("CreateDefaultAcc Error");

            if (!GetAccount(out acc))
                return TsCookie500Err("acc not found");

            if (acc is null)
                return TsCookie500Err("CreateDefaultAcc Error");

            if (acc.UsernameHash != Sha3.GetSha3Utf8(username))
                return new TsCookie {Error = true, ErrorMsg = "Username Is False"};

            if (acc.PasswdHash != Sha3.GetSha3Utf8(passwd))
                return new TsCookie {Error = true, ErrorMsg = "Password Is False"};

            return new TsCookie {Error = false, Key = PushNewKey()};
        }

        [HttpPost("changepasswd")]
        public bool ChangePasswd(string? key, string? oldPasswd, string? newPasswd) {
            if (string.IsNullOrEmpty(key)) return ThrowErr("key Is Empty Or Null");
            if (string.IsNullOrEmpty(oldPasswd)) return ThrowErr("oldPasswd Is Empty Or Null");
            if (string.IsNullOrEmpty(newPasswd)) return ThrowErr("newPasswd Is Empty Or Null");

            if (!Cookie.CookieManger.KeyExist(key)) return ThrowErr("key Not Exist");
            if (!GetAccount(out var acc)) return ThrowErr("Acc Not Found");
            if (acc!.PasswdHash != Sha3.GetSha3Utf8(oldPasswd)) return ThrowErr("Password Is False");

            if (!UpdatePassword(Sha3.GetSha3Utf8(newPasswd))) return ThrowErr("UpdatePassword Error");

            return true;
        }

        private static bool UpdatePassword(string passwordHash) {
            var db = GetCol.GetAcc(StaticConf.DbUrl);
            if (db is null) return ThrowErr("db not found");

            db.UpdateOne(
                Builders<Acc>.Filter.Eq(x => x.UsernameHash, DefaultUsernameHash),
                Builders<Acc>.Update.Set(x => x.PasswdHash, passwordHash)
            );
            return true;
        }

        private static bool GetAccount(out Acc? acc) {
            var db = GetCol.GetAcc(StaticConf.DbUrl);
            if (db is null) {
                acc = null;
                return ThrowErr("db not found");
            }
            acc = db.FindSync<Acc>(Builders<Acc>.Filter.Eq(x => x.UsernameHash, DefaultUsernameHash)).FirstOrDefault();
            return true;
        }

        private static bool CreateDefaultAcc() {
            var db = GetCol.GetAcc(StaticConf.DbUrl);
            if (db is null) return ThrowErr("db not found");

            try {
                db.InsertOne(new Acc {
                    PasswdHash = DefaultPasswordHash,
                    UsernameHash = DefaultUsernameHash
                });
                return true;
            }
            catch (Exception) {
                return false;
            }
        }

        private static string? PushNewKey() {
            var key = Sha3.GetSha3Utf8(Cookie.CookieBuilder.Build());
            Cookie.CookieManger.Push(key);
            return key;
        }

        private static bool ThrowErr(string msg) {
#if DEBUG
            throw new Exception(msg);
#else
            return false;
#endif
        }

        public class TsCookie {
            public bool Error { get; set; }
            public string? ErrorMsg { get; set; }
            public string? Key { get; set; }
        }
    }
}