using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FreeBox.Client.WpfClient.Entitities;
using FreeBox.Client.WpfClient.Model;
using FreeBox.Shared.Dtos;

namespace FreeBox.Client.WpfClient.Operations
{
    internal class ApiOperations
    {
        private readonly Uri _baseUrl;

        public ApiOperations()
        {
            this._baseUrl = new Uri("https://localhost:7233/");
        }

        public User? AuthenticateUser(string login, string password)
        {
            var endpoint = new Uri(_baseUrl, "api/accounts/auth");
            try
            {

                var httpClient = new HttpClient();
                HttpResponseMessage response = httpClient.PostAsJsonAsync(endpoint, new UserCredentialsDto(login, password)).Result;
                if (!response.IsSuccessStatusCode)
                    return null;
                var authInfo =
                    JsonConvert.DeserializeObject<AuthInfoDto>(response.Content.ReadAsStringAsync().Result);
                return authInfo is null ? null : new User(authInfo.Login, password, authInfo.Token);
            }
            catch (Exception)
            {
                return null;
            }


        }

        public User? RegisterUser(string login, string password)
        {
            var endpoint = new Uri(_baseUrl, "api/accounts/register");
            try
            {

                var httpClient = new HttpClient();
                HttpResponseMessage response = httpClient.PostAsJsonAsync(endpoint, new UserCredentialsDto(login, password)).Result;
                if (!response.IsSuccessStatusCode)
                    return null;
                var authInfo =
                    JsonConvert.DeserializeObject<AuthInfoDto>(response.Content.ReadAsStringAsync().Result);
                return authInfo is null ? null : new User(authInfo.Login, password, authInfo.Token);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public bool Delete()
        {
            var endpoint = new Uri(_baseUrl, "api/accounts/delete");
            
            try
            {
                ActualizeToken();

                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", Globals.LoggedInUser.Token);
                HttpResponseMessage response = httpClient.DeleteAsync(endpoint).Result;
                if (!response.IsSuccessStatusCode)
                    return false;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void ActualizeToken()
        {
            var endpoint = new Uri(_baseUrl, "api/accounts/auth");

            var httpClient = new HttpClient();
            HttpResponseMessage response = httpClient.PostAsJsonAsync(endpoint, new UserCredentialsDto(Globals.LoggedInUser.Login,
                Globals.LoggedInUser.Password)).Result;
            if (!response.IsSuccessStatusCode)
                throw new WebException(response.StatusCode.ToString());
            var authInfo =
                JsonConvert.DeserializeObject<AuthInfoDto>(response.Content.ReadAsStringAsync().Result);
            Globals.LoggedInUser.Token = authInfo.Token;
        }
    }
}
