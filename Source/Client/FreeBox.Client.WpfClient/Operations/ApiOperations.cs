using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FreeBox.Client.WpfClient.Entitities;
using FreeBox.Client.WpfClient.Model;
using FreeBox.Shared.Dtos;
using Newtonsoft.Json;

namespace FreeBox.Client.WpfClient.Operations
{
    internal class ApiOperations
    {
        private readonly Uri _baseUrl;

        public ApiOperations()
        {
           _baseUrl = new Uri(ConfigurationManager.AppSettings["Url"]
                                    ?? throw new InvalidOperationException("No server url set in App.config"));
        }

        public async Task<User?> AuthenticateUser(string login, string password)
        {
            var endpoint = new Uri(_baseUrl, "api/accounts/auth");
            try
            {
                var httpClient = new HttpClient();
                HttpResponseMessage response = await httpClient.PostAsJsonAsync(endpoint, new UserCredentialsDto(login, password));
                if (!response.IsSuccessStatusCode)
                    return null;
                AuthInfoDto? authInfo =
                    JsonConvert.DeserializeObject<AuthInfoDto>(await response.Content.ReadAsStringAsync());
                return authInfo is null ? null : new User(authInfo.Login, password, authInfo.Token);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> UploadFile(string path)
        {
            AuthorizationTest();
            var endpoint = new Uri(_baseUrl, "api/files/upload");
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"File [{path}] not found.");
            }

            try
            {
                await ActualizeToken();
                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", Globals.LoggedInUser!.Token);
                using var form = new MultipartFormDataContent();
                using var fileContent = new ByteArrayContent(await File.ReadAllBytesAsync(path));
                fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
                form.Add(fileContent, "fileForm", Path.GetFileName(path));
                HttpResponseMessage response = await httpClient.PostAsync(endpoint, form);
                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<User?> RegisterUser(string login, string password)
        {
            var endpoint = new Uri(_baseUrl, "api/accounts/register");
            try
            {
                var httpClient = new HttpClient();
                HttpResponseMessage response = await httpClient.PostAsJsonAsync(endpoint, new UserCredentialsDto(login, password));
                if (!response.IsSuccessStatusCode)
                    return null;
                AuthInfoDto? authInfo =
                    JsonConvert.DeserializeObject<AuthInfoDto>(await response.Content.ReadAsStringAsync());
                return authInfo is null ? null : new User(authInfo.Login, password, authInfo.Token);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> DeleteUser()
        {
            AuthorizationTest();
            var endpoint = new Uri(_baseUrl, $"api/accounts/delete/{Globals.LoggedInUser!.Login}");
            try
            {
                await ActualizeToken();
                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", Globals.LoggedInUser.Token);
                HttpResponseMessage response = await httpClient.DeleteAsync(endpoint);
                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DeleteFile(Guid fileId)
        {
            AuthorizationTest();
            var endpoint = new Uri(_baseUrl, $"api/files/delete/{fileId}");
            try
            {
                await ActualizeToken();
                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", Globals.LoggedInUser!.Token);
                HttpResponseMessage response = await httpClient.DeleteAsync(endpoint);
                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<IEnumerable<ContainerInfo>?> GetContainerInfos()
        {
            AuthorizationTest();
            var endpoint = new Uri(_baseUrl, $"api/files/user/{Globals.LoggedInUser!.Login}/get/all");
            try
            {
                await ActualizeToken();
                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", Globals.LoggedInUser.Token);
                HttpResponseMessage response = await httpClient.GetAsync(endpoint);
                if (!response.IsSuccessStatusCode)
                    return null;
                List<ContainerInfoDto>? dtos =
                    JsonConvert.DeserializeObject<List<ContainerInfoDto>>(await response.Content.ReadAsStringAsync());
                return dtos?.Select(x => new ContainerInfo(x.Id, x.Name, x.Size, x.SaveDate));
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<FileContainer?> GetFile(Guid fileId)
        {
            AuthorizationTest();
            var endpoint = new Uri(_baseUrl, $"api/files/get/{fileId}");
            try
            {
                await ActualizeToken();
                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", Globals.LoggedInUser!.Token);
                HttpResponseMessage response = await httpClient.GetAsync(endpoint);
                if (!response.IsSuccessStatusCode)
                    return null;
                string fileName = response.Content.Headers.ContentDisposition!.FileName!;
                return new FileContainer(await response.Content.ReadAsStreamAsync(), fileName);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private async Task ActualizeToken()
        {
            var endpoint = new Uri(_baseUrl, "api/accounts/auth");

            var httpClient = new HttpClient();
            HttpResponseMessage response = await httpClient.PostAsJsonAsync(
                endpoint,
                new UserCredentialsDto(Globals.LoggedInUser!.Login, Globals.LoggedInUser.Password));
            if (!response.IsSuccessStatusCode)
                throw new WebException();
            var authInfo =
                JsonConvert.DeserializeObject<AuthInfoDto>(response.Content.ReadAsStringAsync().Result);
            Globals.LoggedInUser.Token = authInfo!.Token;
        }

        private void AuthorizationTest()
        {
            if (Globals.LoggedInUser is null)
                throw new InvalidOperationException("User is not logged in");
        }
    }
}
