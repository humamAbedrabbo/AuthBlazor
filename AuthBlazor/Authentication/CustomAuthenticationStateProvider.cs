using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Security.Claims;

namespace AuthBlazor.Authentication
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ProtectedSessionStorage _sessionStorage;
        private ClaimsPrincipal _anonymous = new(new ClaimsIdentity());

        public CustomAuthenticationStateProvider(ProtectedSessionStorage sessionStorage)
        {
            _sessionStorage = sessionStorage;
        }

        //This method will be called when the application opened
        //or when the page got refreshed
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                // This to see the message while Authorizing in App.razor
                // await Task.Delay(2000);



                // the protected session storage is key-value list
                var userSessionStorageResult = await _sessionStorage.GetAsync<UserSession>("UserSession");
                
                var userSession = userSessionStorageResult.Success ? userSessionStorageResult.Value : null;

                if(userSession == null)
                {
                    return await Task.FromResult(new AuthenticationState(_anonymous));
                }

                // not null then create claimsPrincipal
                var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim> 
                { 
                    new Claim(ClaimTypes.Name, userSession.UserName),
                    new Claim(ClaimTypes.Role, userSession.Role),
                }, "CustomAuth"));

                return await Task.FromResult(new AuthenticationState(claimsPrincipal));
            }
            catch 
            {
                // this happens when user tries to modify
                //encrypted session storage UserSession value
                // then we return anonymous
                return await Task.FromResult(new AuthenticationState(_anonymous));
            }
            
        }

        // this method will be used when user login or logout
        public async Task UpdateAuthenticationState(UserSession userSession)
        {
            ClaimsPrincipal claimsPrincipal;

            if(userSession != null)
            {
                // This called when user login
                await _sessionStorage.SetAsync("UserSession", userSession);

                claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
                {
                    new Claim(ClaimTypes.Name, userSession.UserName),
                    new Claim(ClaimTypes.Role, userSession.Role),
                }));
            }
            else
            {
                await _sessionStorage.DeleteAsync("UserSession");
                claimsPrincipal = _anonymous;
            }

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
        }
    }
}
