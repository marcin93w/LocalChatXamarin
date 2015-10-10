using System;
using System.Collections.Generic;
using System.Text;

namespace LocalConnect.Models
{
    public class RegistrationInfo
    {
        public bool Registered { set; get; }
        public string ErrorMsg { set; get; }
        public int ErrorCode { set; get; }
        public SessionInfo SessionInfo { set; get; }
    }
}
