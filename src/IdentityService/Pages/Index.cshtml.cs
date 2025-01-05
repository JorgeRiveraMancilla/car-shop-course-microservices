// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.

using System.Reflection;
using Duende.IdentityServer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityService.Pages
{
    [AllowAnonymous]
    public class Index(IdentityServerLicense? license = null) : PageModel
    {
        public string Version
        {
            get =>
                typeof(Duende.IdentityServer.Hosting.IdentityServerMiddleware)
                    .Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                    ?.InformationalVersion.Split('+')
                    .First() ?? "unavailable";
        }
        public IdentityServerLicense? License { get; } = license;
    }
}
