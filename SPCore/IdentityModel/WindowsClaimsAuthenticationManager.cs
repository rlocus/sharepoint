using System;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Web;
using Microsoft.IdentityModel.Claims;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Administration.Claims;
using Microsoft.SharePoint.IdentityModel;
using Microsoft.SharePoint.WebControls;

namespace SPCore.IdentityModel
{
    public class WindowsClaimsAuthenticationManager
    {
        private SPIisSettings _iisSettings;

        protected SPIisSettings IisSettings
        {
            get
            {
                if (_iisSettings == null)
                {
                    SPSite site = SPContext.Current.Site;

                    if (site != null && site.WebApplication != null)
                    {
                        _iisSettings = site.WebApplication.GetIisSettingsWithFallback(site.Zone);
                    }
                    else if (HttpContext.Current != null)
                    {
                        site = SPControl.GetContextSite(HttpContext.Current);
                        _iisSettings = site.WebApplication.GetIisSettingsWithFallback(site.Zone);
                    }
                }
                return _iisSettings;
            }
        }

        private static SecurityToken GetSecurityTokenFromWindowsIdentity(WindowsIdentity wi, HttpContext context)
        {
            IHttpModule windowsClaimsAuthentication = context.ApplicationInstance.Modules["SPWindowsClaimsAuthentication"];

            if (windowsClaimsAuthentication == null)
                throw new Exception("Could not find the SPWindowsClaimsAuthentication module");

            MethodInfo getSecurityTokenFromWindowsIdentity = windowsClaimsAuthentication.GetType().GetMethod("GetSecurityTokenFromWindowsIdentity", BindingFlags.NonPublic | BindingFlags.Static);

            if (getSecurityTokenFromWindowsIdentity == null)
                throw new Exception("Could not find the SPWindowsClaimsAuthenticationHttpModule.GetSecurityTokenFromWindowsIdentity method");

            SecurityToken securityToken = (SecurityToken)
                getSecurityTokenFromWindowsIdentity.Invoke(null, new object[] { wi, context });

            return securityToken;
        }

        private static void SetPrincipalAndWriteSessionToken(SecurityToken securityToken, SPSessionTokenWriteType sessionCookie)
        {
            SPFederationAuthenticationModule fam = SPFederationAuthenticationModule.Current;

            if (fam == null)
                throw new Exception("Could not find an instance of the SPFederationAuthenticationModule");

            MethodInfo setPrincipalAndWriteSessionToken =
                typeof(SPFederationAuthenticationModule).GetMethod("SetPrincipalAndWriteSessionToken",
                                                                    BindingFlags.Instance |
                                                                    BindingFlags.InvokeMethod |
                                                                    BindingFlags.NonPublic) ??
                typeof(SPFederationAuthenticationModule).GetMethod("SetPrincipalAndWriteSessionToken",
                                                                    new[]
                                                                        {
                                                                            typeof (SecurityToken),
                                                                            typeof (SPSessionTokenWriteType)
                                                                        });
            if (setPrincipalAndWriteSessionToken == null)
            {
                throw new Exception(
                    "Could not find the SPFederationAuthenticationModule.SetPrincipalAndWriteSessionToken method");
            }

            setPrincipalAndWriteSessionToken.Invoke(fam, new object[] { securityToken, sessionCookie });

            //fam.SetPrincipalAndWriteSessionToken(securityToken, sessionCookie);
        }

        public bool Authenticate(string userName, string password, bool rememberMe)
        {
            string[] parts = userName.Split(new[] { "\\" }, StringSplitOptions.None);
            string domen = string.Empty;

            if (parts.Length == 2)
            {
                domen = parts[0];
                userName = parts[1];
            }
            return Authenticate(domen, userName, password, rememberMe);
        }

        public bool Authenticate(string domain, string userName, string password, bool rememberMe)
        {
            using (var impersonation = new Impersonation(domain, userName, password))
            {
                if (impersonation.Authenticated)
                {
                    using (var wi = WindowsClaimsIdentity.GetCurrent())
                    {
                        SecurityToken securityToken = GetSecurityTokenFromWindowsIdentity(wi, HttpContext.Current);

                        SPSessionTokenWriteType writeCookie = SPSecurityTokenServiceManager.Local.UseSessionCookies && rememberMe
                                                                  ? SPSessionTokenWriteType.WriteDefaultCookie
                                                                  : SPSessionTokenWriteType.WriteSessionCookie;

                        if (securityToken == null)
                        {
                            throw new ArgumentNullException("securityToken");
                        }

                        SPSecurity.RunWithElevatedPrivileges(
                            () => SetPrincipalAndWriteSessionToken(securityToken, writeCookie));
                    }

                    return true;
                }
            }
            return false;
        }

        public void SignOut(bool isIpRequest)
        {
            if (IsClaimsWindowsAuthenticationOnly())
            {
                // Clear session state.
                if (HttpContext.Current.Session != null)
                {
                    HttpContext.Current.Session.Clear();
                }

                string cookieValue = null;

                if (HttpContext.Current.Request.Browser["supportsEmptyStringInCookieValue"] == "false")
                {
                    cookieValue = "NoCookie";
                }

                // Remove cookies for authentication. 
                HttpCookie cookieSession = HttpContext.Current.Request.Cookies["WSS_KeepSessionAuthenticated"];

                if (cookieSession != null)
                {
                    HttpContext.Current.Response.Cookies.Remove("WSS_KeepSessionAuthenticated");
                    cookieSession.Value = cookieValue;
                    cookieSession.Expires = DateTime.Now.AddDays(-1D);
                    HttpContext.Current.Response.SetCookie(cookieSession);
                }

                HttpCookie cookiePersist = HttpContext.Current.Request.Cookies["MSOWebPartPage_AnonymousAccessCookie"];

                if (cookiePersist != null)
                {
                    HttpContext.Current.Response.Cookies.Remove("MSOWebPartPage_AnonymousAccessCookie");
                    cookiePersist.Value = cookieValue;
                    cookiePersist.Expires = DateTime.Now.AddDays(-1D);
                    HttpContext.Current.Response.SetCookie(cookiePersist);
                }

                SPFederationAuthenticationModule.Current.SignOut(isIpRequest);
            }
        }

        public bool IsClaimsWindowsAuthenticationOnly()
        {
            return IisSettings != null && (IisSettings.UseClaimsAuthentication && IisSettings.UseWindowsClaimsAuthenticationProvider && IisSettings.ClaimsAuthenticationProviders.Count() == 1);
        }
    }
}
