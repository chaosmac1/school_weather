using System;
using System.Reflection.Metadata.Ecma335;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BackendApi.Controllers {
    [ApiController]
    [Route("[controller]")]
    public class ConLogin : ControllerBase {
        private readonly ILogger<ConLogin> _logger;

        public ConLogin(ILogger<ConLogin> logger) => _logger = logger;
        
        public class TsCookie {
            public bool Error { get; set; }
            public string? ErrorMsg { get; set; }
            public string? Key { get; set; }
            public string? Username { get; set; }
        }

        [HttpPost("login")]
        public TsCookie Login(string? username, string? passwd) {
            if (string.IsNullOrEmpty(username))
                return new TsCookie() {Error = true, ErrorMsg = "Username is Null Or Empty"};

            throw new NotImplementedException("Login");
        }

        [HttpPost("changepasswd")]
        public bool ChangePasswd(string? username, string? key, string? oldPasswd, string? newPasswd) {
            throw new NotImplementedException("ChangePasswd");
        } 
    }
}