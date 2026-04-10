using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nocturne.Core.Contracts.Multitenancy;
using Nocturne.Core.Models.Authorization;

namespace Nocturne.API.Controllers;

/// <summary>
/// RFC 8414 OAuth 2.0 Authorization Server Metadata.
/// Served per tenant subdomain so clients can discover the issuer and
/// endpoints without hardcoding URIs.
/// </summary>
[ApiController]
[Tags("OAuth")]
public class OAuthMetadataController : ControllerBase
{
    /// <summary>
    /// RFC 8414 well-known discovery endpoint.
    /// </summary>
    [HttpGet("/.well-known/oauth-authorization-server")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthorizationServerMetadata), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<AuthorizationServerMetadata> GetMetadata()
    {
        // Tenant resolution is mandatory — the discovery doc is per-tenant.
        if (HttpContext.Items["TenantContext"] is not TenantContext)
        {
            return NotFound();
        }

        var baseUrl = $"{Request.Scheme}://{Request.Host}";

        var metadata = new AuthorizationServerMetadata
        {
            Issuer = baseUrl,
            AuthorizationEndpoint = $"{baseUrl}/api/oauth/authorize",
            TokenEndpoint = $"{baseUrl}/api/oauth/token",
            DeviceAuthorizationEndpoint = $"{baseUrl}/api/oauth/device_authorization",
            RegistrationEndpoint = $"{baseUrl}/api/oauth/register",
            RevocationEndpoint = $"{baseUrl}/api/oauth/revoke",
            ScopesSupported = OAuthScopes.AllScopes.ToList(),
            ResponseTypesSupported = ["code"],
            GrantTypesSupported =
            [
                "authorization_code",
                "refresh_token",
                "urn:ietf:params:oauth:grant-type:device_code",
            ],
            CodeChallengeMethodsSupported = ["S256"],
            TokenEndpointAuthMethodsSupported = ["none"],
            ServiceDocumentation = "https://github.com/nightscout/nocturne",
        };

        return Ok(metadata);
    }
}

/// <summary>
/// RFC 8414 Authorization Server Metadata document body.
/// </summary>
public class AuthorizationServerMetadata
{
    [JsonPropertyName("issuer")]
    public string Issuer { get; set; } = "";

    [JsonPropertyName("authorization_endpoint")]
    public string AuthorizationEndpoint { get; set; } = "";

    [JsonPropertyName("token_endpoint")]
    public string TokenEndpoint { get; set; } = "";

    [JsonPropertyName("device_authorization_endpoint")]
    public string DeviceAuthorizationEndpoint { get; set; } = "";

    [JsonPropertyName("registration_endpoint")]
    public string RegistrationEndpoint { get; set; } = "";

    [JsonPropertyName("revocation_endpoint")]
    public string RevocationEndpoint { get; set; } = "";

    [JsonPropertyName("scopes_supported")]
    public List<string> ScopesSupported { get; set; } = [];

    [JsonPropertyName("response_types_supported")]
    public List<string> ResponseTypesSupported { get; set; } = [];

    [JsonPropertyName("grant_types_supported")]
    public List<string> GrantTypesSupported { get; set; } = [];

    [JsonPropertyName("code_challenge_methods_supported")]
    public List<string> CodeChallengeMethodsSupported { get; set; } = [];

    [JsonPropertyName("token_endpoint_auth_methods_supported")]
    public List<string> TokenEndpointAuthMethodsSupported { get; set; } = [];

    [JsonPropertyName("service_documentation")]
    public string? ServiceDocumentation { get; set; }
}
