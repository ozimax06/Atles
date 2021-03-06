﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Atles.Server.Services
{
    public interface IIntegrityService
    {
        Task EnsureUserCreatedAsync(IdentityUser identityUser, bool confirm = false);
        Task EnsureUserConfirmedAsync(IdentityUser identityUser);
    }
}