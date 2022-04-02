using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        public bool UploadFile(string path)
        {
            AuthorizationTest();
            var endpoint = new Uri(_baseUrl, "api/file/upload");
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
                ActualizeToken();
                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", Globals.LoggedInUser.Token);
                using var form = new MultipartFormDataContent();
                using var fileContent = new ByteArrayContent(File.ReadAllBytes(path));
                fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
                form.Add(fileContent, "fileForm", Path.GetFileName(path));
                HttpResponseMessage response = httpClient.PostAsync(endpoint, form).Result;
                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
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
                AuthInfoDto? authInfo =
                    JsonConvert.DeserializeObject<AuthInfoDto>(response.Content.ReadAsStringAsync().Result);
                return authInfo is null ? null : new User(authInfo.Login, password, authInfo.Token);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public bool DeleteUser()
        {
            AuthorizationTest();
            var endpoint = new Uri(_baseUrl, "api/accounts/delete");
            try
            {
                ActualizeToken();
                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", Globals.LoggedInUser.Token);
                HttpResponseMessage response = httpClient.DeleteAsync(endpoint).Result;
                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool DeleteFile(Guid id)
        {
            AuthorizationTest();
            var endpoint = new Uri(_baseUrl, $"api/file/delete/{id}");
            try
            {
                ActualizeToken();
                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", Globals.LoggedInUser.Token);
                HttpResponseMessage response = httpClient.DeleteAsync(endpoint).Result;
                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public IEnumerable<ContainerInfo>? GetContainerInfos()
        {
            AuthorizationTest();
            var endpoint = new Uri(_baseUrl, "api/file/get/all");
            try
            {
                ActualizeToken();
                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", Globals.LoggedInUser.Token);
                HttpResponseMessage response = httpClient.GetAsync(endpoint).Result;
                if (!response.IsSuccessStatusCode)
                    return null;
                List<ContainerInfoDto>? dtos = JsonConvert.DeserializeObject<List<ContainerInfoDto>>(response.Content.ReadAsStringAsync()
                    .Result);
                return dtos?.Select(x => new ContainerInfo(x.Id, x.Name, x.Size, x.SaveDate));
            }
            catch (Exception)
            {
                return null;
            }
        }

        public FileContainer? GetFile(Guid fileId)
        {
            AuthorizationTest();
            var endpoint = new Uri(_baseUrl, $"api/file/get/{fileId}");
            try
            {
                ActualizeToken();
                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", Globals.LoggedInUser.Token);
                HttpResponseMessage response = httpClient.GetAsync(endpoint).Result;
                if (!response.IsSuccessStatusCode)
                    return null;
                string fileName = response.Content.Headers.ContentDisposition.FileName;
                return new FileContainer(response.Content.ReadAsStream(), fileName);
            }
            catch (Exception)
            {
                return null;
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

        private void AuthorizationTest()
        {
            if (Globals.LoggedInUser is null)
                throw new InvalidOperationException("User is not logged in");
        }
    }
}
