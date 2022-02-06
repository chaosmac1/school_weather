using BackendApi.DataBase;
using BackendApi.DataBase.Type;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using MongoDB.Libmongocrypt;
using Org.BouncyCastle.Crypto.Tls;
using Org.BouncyCastle.Utilities;

namespace BackendApi.Controllers; 

[ApiController]
[Route("[controller]")]
public class ConLogin : ControllerBase {
    private static readonly string DefaultUsernameHash = Sha3.GetSha3Utf8("root");
    private static readonly string DefaultPasswordHash = Sha3.GetSha3Utf8("root");
    private readonly ILogger<ConLogin> _logger;

    public ConLogin(ILogger<ConLogin> logger) => _logger = logger;

    public class ResponseRes<T> where T: class {
        public int Response { get; set; }
        public string? Msg { get; set; }
        public T? Data { get; set; }

        public static ResponseRes<T> FactoryRight(T? data) {
            return new ResponseRes<T> {
                Data = data,
                Msg = "",
                Response = StatusCodes.Status200OK
            };
        }
    }

    private static (bool Error, string msg) CheckRootUser(string username, string passwd, out (bool UsernameRight, bool PasswordRight) res) {
        static (bool Error, string msg) ThrowMsg(string msg) {
#if DEBUG
            throw new Exception(msg);
#else
            return (true, msg);
#endif
        }

        if (!GetAccount(out var acc))
            return ThrowMsg("acc not found");

        if (acc is null && !CreateDefaultAcc())
            return ThrowMsg("CreateDefaultAcc Error");

        if (!GetAccount(out acc))
            return ThrowMsg("acc not found");

        if (acc is null)
            return ThrowMsg("CreateDefaultAcc Error");

        if (acc.UsernameHash != Sha3.GetSha3Utf8(username)) {
            res = (false, false);
            return (false, String.Empty);
        }
        
        if (acc.PasswdHash != Sha3.GetSha3Utf8(passwd)) {
            res = (true, false);
            return (false, String.Empty);
        }
        
        res = (true, true);
        return (false, String.Empty);
    }
    
    public class ResponseResLoginData {
        public bool UsernameRight { get; set; }
        public bool PasswordRight { get; set; }
    }
    
    public class LoginBody {
        public string? Username { get; set; }
        public string? Passwd { get; set; }
    }
    
    [HttpPost("login")]
    public ResponseRes<ResponseResLoginData> Login(LoginBody body) {
        if (string.IsNullOrEmpty(body.Username))
            return new() {
                Data = new() {PasswordRight = false, UsernameRight = false},
                Msg = "Username is Null Or Empty",
                Response = 200,
            };
        if (string.IsNullOrEmpty(body.Passwd))
            return new() {
                Data = new() {PasswordRight = false, UsernameRight = false},
                Msg = "Password is Null Or Empty",
                Response = 200
            };
        
        static ResponseRes<ResponseResLoginData> Get500() => new() {
            Data = null,
            Msg = "Internal Server Error",
            Response = StatusCodes.Status500InternalServerError
        };
            
        static ResponseRes<ResponseResLoginData> TsCookie500Err(string msg) {
#if DEBUG
            throw new Exception(msg);
#else
            return TsCookie500();
#endif
        }

        var resErr = CheckRootUser(body.Username, body.Passwd, out var res);
        if (resErr.Error) {
            return new ResponseRes<ResponseResLoginData>() {
                Data = null,
                Msg = resErr.msg,
                Response = 500
            };
        }
        return new ResponseRes<ResponseResLoginData>() {
            Data = new ResponseResLoginData() {
                PasswordRight = res.PasswordRight,
                UsernameRight = res.UsernameRight
            },
            Msg = string.Empty,
            Response = 200
        };
    }

    public class ResponseResChangePasswd: ResponseResLoginData { }
    
    public class ChangePasswdBody {
        public string? Username { get; set; }
        public string? OldPasswd { get; set; }
        public string? NewPasswd { get; set; }
    }
    [HttpPost("changepasswd")]
    public ResponseRes<ResponseResChangePasswd> ChangePasswd(ChangePasswdBody body) {
        var login = Login(
            new LoginBody() { Passwd = body.OldPasswd, Username = body.Username });

        static ResponseRes<ResponseResChangePasswd> ThrowMsg(string msg) {
#if DEBUG
            throw new Exception(msg);
#else
            return new ResponseRes<ResponseResChangePasswd>() {
                Data = null,
                Msg = msg,
                Response = 500
            };
#endif
        }
        
        if (login.Response != 200 || login.Data is null || !login.Data.PasswordRight || !login.Data.UsernameRight) {
            return new ResponseRes<ResponseResChangePasswd>() {
                Data = null,
                Msg = login.Msg,
                Response = login.Response
            };
        }

        if (string.IsNullOrEmpty(body.NewPasswd))
            return ThrowMsg("newPasswd Is Empty Or Null");
        
        if (!UpdatePassword(Sha3.GetSha3Utf8(body.NewPasswd))) 
            return ThrowMsg("UpdatePassword Error");

        return ResponseRes<ResponseResChangePasswd>.FactoryRight(new ResponseResChangePasswd() {
            PasswordRight = true,
            UsernameRight = true
        });
    }


    public class ResponseResLog : ResponseResLoginData {
        public class IpLog {
            public long DateTime { get; set; }
            public string? Ip { get; set; }
        }
        public IpLog[] Ips { get; set; }
    }

    public class PostLogsBody {
        public string? Username { get; set; }
        public string? Passwd { get; set; }
    }

    [HttpPost("postlogs")]
    public ResponseRes<ResponseResLog> PostLogs(PostLogsBody body) {
        static ResponseRes<ResponseResLog> Get500() => new() {
            Data = null,
            Msg = "Internal Server Error",
            Response = StatusCodes.Status500InternalServerError
        };
        
        var login = Login(
            new LoginBody() { Passwd = body.Passwd, Username = body.Username });

        static ResponseRes<ResponseResLog> ThrowMsg(string msg) {
#if DEBUG
            throw new Exception(msg);
#else
            return new ResponseRes<ResponseResLog>() {
                Data = null,
                Msg = msg,
                Response = 500
            };
#endif
        }

        var mongo = GetCol.GetLog(StaticConf.DbUrl);
        if (mongo is null) return Get500();

        List<ResponseResLog.IpLog> res = mongo.FindSync(Builders<Log>.Filter.Empty, 
            new FindOptions<Log>() {Limit = 200, Sort = Builders<Log>.Sort.Ascending(x => x.DateTime)})
            .ToEnumerable().Select(log => 
                new ResponseResLog.IpLog() {
                    Ip = log.IP,
                    DateTime = (long)log.DateTime.Subtract(new DateTime(1970,1,1,0,0,0,DateTimeKind.Utc))
                        .TotalMilliseconds
                }).ToList();


        return new ResponseRes<ResponseResLog>() {
            Response = StatusCodes.Status200OK,
            Msg = "",
            Data = new ResponseResLog() {
                PasswordRight = true,
                UsernameRight = true,
                Ips = res.ToArray() // (from i in logs select (i.DateTime.Ticks,i.IP)).ToArray()
            }
        };
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