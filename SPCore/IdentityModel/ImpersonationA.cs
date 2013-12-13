using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Security.Principal;

namespace SPCore.IdentityModel
{
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    public class ImpersonationA : IDisposable
    {
        private readonly string _domain;
        private readonly string _username;
        private readonly string _password;
        private SafeTokenHandle _handle;
        private SafeTokenHandle _handleDuplicate;
        private WindowsImpersonationContext _context;

        // Intended for users who will be interactively using the computer.
        private const int LOGON32_LOGON_INTERACTIVE = 2;
        // Use the standard logon provider for the system.
        private const int LOGON32_PROVIDER_DEFAULT = 0;

        public bool Authenticated { get; private set; }

        public int ErrorCode { get; private set; }

        public ImpersonationA(string domain, string username, string password)
        {
            _domain = domain;
            _username = username;
            _password = password;
            Impersonate();
        }

        public void Impersonate()
        {
            Authenticated = false;

            // Remove the current impersonation by calling RevertToSelf()
            if (RevertToSelf())
            {
                Authenticated = LogonUserA(_username, _domain, _password,
                                    LOGON32_LOGON_INTERACTIVE,
                                    LOGON32_PROVIDER_DEFAULT,
                                    out _handle) != 0;

                if (!Authenticated)
                {
                    ErrorCode = Marshal.GetLastWin32Error();
                }

                // Make a copy of the token for the windows identity private member.
                if (DuplicateToken(this._handle, 2, out this._handleDuplicate) != 0)
                {
                    // set the private member for the current impersonation context.
                    this._context = WindowsIdentity.Impersonate(this._handleDuplicate.DangerousGetHandle());
                }
            }
        }

        public void Dispose()
        {
            if (_context != null) _context.Dispose();
            if (_handle != null) this._handle.Dispose();
            if (_handleDuplicate != null) this._handleDuplicate.Dispose();
        }

        // Attempts to log a user on to the local computer. The local computer is the 
        // computer from which LogonUser was called. You cannot use LogonUser to log on 
        // to a remote computer.
        // Additional documentation here:
        // http://msdn.microsoft.com/en-us/library/aa378184(VS.85).aspx
        [DllImport("advapi32.dll")]
        private static extern int LogonUserA(String lpszUserName,
            String lpszDomain,
            String lpszPassword,
            int dwLogonType,
            int dwLogonProvider,
            out SafeTokenHandle phToken);

        // Creates a new access token that duplicates one already in existence.
        // Additional documentation here:
        // http://msdn.microsoft.com/en-us/library/aa446616(VS.85).aspx
        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int DuplicateToken(SafeTokenHandle hToken,
            int impersonationLevel,
            out SafeTokenHandle hNewToken);

        // Terminates the impersonation of a client application.
        // Additional documentation here:
        // http://msdn.microsoft.com/en-us/library/aa379317(VS.85).aspx
        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool RevertToSelf();
    }

}
