using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Office.Server.UserProfiles;
using Microsoft.SharePoint;

namespace SPCore
{
    public static class SPSiteExtensions
    {
        public static SPUserToken GetSystemToken(this SPSite site)
        {
            SPUserToken token = null;
            bool tempCADE = site.CatchAccessDeniedException;

            try
            {
                site.CatchAccessDeniedException = false;
                token = site.SystemAccount.UserToken;
            }
            catch (UnauthorizedAccessException)
            {
                //token = SPUserToken.SystemAccount;

                SPSecurity.RunWithElevatedPrivileges(() =>
                {
                    using (SPSite elevSite = new SPSite(site.ID, site.Zone))
                        token = elevSite.SystemAccount.UserToken;
                });
            }
            finally
            {
                site.CatchAccessDeniedException = tempCADE;
            }

            return token;
        }

        public static void RunAsSystem(this SPSite site, Action<SPSite> action)
        {
            using (SPSite elevSite = new SPSite(site.ID, site.Zone, site.GetSystemToken()))
            {
                if (action != null) action(elevSite);
            }
        }

        public static void RunAsUser(this SPSite site, SPUser user, Action<SPSite> action)
        {
            using (SPSite elevSite = new SPSite(site.ID, site.Zone, user.UserToken))
            {
                if (action != null) action(elevSite);
            }
        }

        public static T SelectAsSystem<T>(this SPSite site, Func<SPSite, T> selector)
        {
            using (SPSite elevSite = new SPSite(site.ID, site.Zone, site.GetSystemToken()))
            {
                if (selector != null) return selector(elevSite);
            }
            return default(T);
        }

        public static T SelectAsUser<T>(this SPSite site, SPUser user, Func<SPSite, T> selector)
        {
            using (SPSite elevSite = new SPSite(site.ID, site.Zone, user.UserToken))
            {
                if (selector != null) return selector(elevSite);
            }
            return default(T);
        }

        public static void RunAsSystem(this SPSite site, Guid webId, Action<SPWeb> action)
        {
            site.RunAsSystem(s =>
                                 {
                                     using (SPWeb web = s.OpenWeb(webId))
                                     {
                                         if (action != null) action(web);
                                     }
                                 });
        }

        public static void RunAsUser(this SPSite site, Guid webId, SPUser user, Action<SPWeb> action)
        {
            site.RunAsUser(user, s =>
            {
                using (SPWeb web = s.OpenWeb(webId))
                {
                    if (action != null) action(web);
                }
            });
        }

        public static void RunAsSystem(this SPSite site, string webUrl, Action<SPWeb> action)
        {
            site.RunAsSystem(s =>
                                 {
                                     using (SPWeb web = s.OpenWeb(webUrl))
                                     {
                                         if (action != null) action(web);
                                     }
                                 });
        }

        public static void RunAsUser(this SPSite site, string webUrl, SPUser user, Action<SPWeb> action)
        {
            site.RunAsUser(user, s =>
            {
                using (SPWeb web = s.OpenWeb(webUrl))
                {
                    if (action != null) action(web);
                }
            });
        }

        public static T SelectAsSystem<T>(this SPSite site, Guid webId, Func<SPWeb, T> selector)
        {
            return site.SelectAsSystem(s =>
                                           {
                                               using (SPWeb web = s.OpenWeb(webId))
                                               {
                                                   if (selector != null) return selector(web);
                                                   else return default(T);
                                               }
                                           });
        }

        public static T SelectAsUser<T>(this SPSite site, Guid webId, SPUser user, Func<SPWeb, T> selector)
        {
            return site.SelectAsUser(user, s =>
            {
                using (SPWeb web = s.OpenWeb(webId))
                {
                    if (selector != null) return selector(web);
                    else return default(T);
                }
            });
        }

        public static T SelectAsSystem<T>(this SPSite site, string webUrl, Func<SPWeb, T> selector)
        {
            return site.SelectAsSystem(s =>
            {
                using (SPWeb web = s.OpenWeb(webUrl))
                {
                    if (selector != null) return selector(web);
                    else return default(T);
                }
            });
        }

        public static T SelectAsUser<T>(this SPSite site, string webUrl, SPUser user, Func<SPWeb, T> selector)
        {
            return site.SelectAsUser(user, s =>
            {
                using (SPWeb web = s.OpenWeb(webUrl))
                {
                    if (selector != null) return selector(web);
                    else return default(T);
                }
            });
        }

        public static IEnumerable<SPWeb> GetAllWebs(this SPSite site)
        {
            return site.AllWebs.AsSafeEnumerable();
        }

        public static IEnumerable<SPWeb> GetSubWebs(this SPSite site)
        {
            return site.RootWeb.GetSubwebsForCurrentUser().AsSafeEnumerable();
        }

        public static void ProcessUserProfile(this SPSite site, Action<UserProfileManager> action, bool ignoreUserPrivacy = false)
        {
            HttpContext currentContext = HttpContext.Current;

            try
            {
                HttpContext.Current = null;
                SPSecurity.RunWithElevatedPrivileges(() =>
                {
                    site.RunAsSystem(elevSite =>
                                     {
                                         SPServiceContext serviceContext = SPServiceContext.GetContext(elevSite);
                                         UserProfileManager upm = new UserProfileManager(serviceContext, ignoreUserPrivacy);

                                         if (action != null)
                                         {
                                             action(upm);
                                         }
                                     });
                });
            }
            finally
            {
                HttpContext.Current = currentContext;
            }
        }

        public static IEnumerable<string> GetSiteUrls(this SPSite site, bool full = false)
        {
            return full
                       ? site.AllWebs.WebsInfo.Select(webInfo => site.MakeFullUrl(webInfo.ServerRelativeUrl))
                       : site.AllWebs.WebsInfo.Select(webInfo => webInfo.ServerRelativeUrl);
        }
    }
}
