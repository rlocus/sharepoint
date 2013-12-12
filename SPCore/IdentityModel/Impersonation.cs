using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Security.Principal;

namespace SPCore.IdentityModel
{
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    public class Impersonation : IDisposable
    {
        private readonly string _domain;
        private readonly string _username;
        private readonly string _password;
        private SafeTokenHandle _handle;
        private WindowsImpersonationContext _context;

        private const int LOGON32_LOGON_NETWORK = 3;
        //private const int LOGON32_LOGON_NEW_CREDENTIALS = 9;
        private const int LOGON32_PROVIDER_DEFAULT = 0;

        public bool Authenticated { get; private set; }

        public int ErrorCode { get; private set; }

        public Impersonation(string domain, string username, string password)
        {
            _domain = domain;
            _username = username;
            _password = password;
            Impersonate();
        }

        private bool Impersonate()
        {
            Authenticated = LogonUser(_username, _domain, _password,
                           LOGON32_LOGON_NETWORK/*LOGON32_LOGON_NEW_CREDENTIALS*/, LOGON32_PROVIDER_DEFAULT, out this._handle);

            if (!Authenticated)
            {
                ErrorCode = Marshal.GetLastWin32Error();
            }

            this._context = WindowsIdentity.Impersonate(this._handle.DangerousGetHandle());
            return Authenticated;
        }

        public void Dispose()
        {
            if (_context != null) _context.Dispose();
            if (_handle != null) this._handle.Dispose();
        }

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool LogonUser(String lpszUsername, String lpszDomain, String lpszPassword, int dwLogonType, int dwLogonProvider, out SafeTokenHandle phToken);

    }

}
