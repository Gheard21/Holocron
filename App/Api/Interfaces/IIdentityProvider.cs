using System;

namespace Holocron.App.Api.Interfaces;

public interface IIdentityProvider
{
    string? GetCurrentUserId();
}
