using Newtonsoft.Json;
using System.Net;
using System.IO;
using System.Dynamic;
using System.Diagnostics;

namespace GLPIApi
{
    public class glpi
    {
        public glpi(string user, string password, string appToken, string userToken, string glpiURL)
        {
            this.user = user;
            this.password = password;
            this.glpiURL = glpiURL;
            this.appToken = appToken;
            this.userToken = userToken;
            if (user != null) this.basicAuth = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(this.user + ":" + this.password));
        }

        private string user = null;
        private string password;
        private string glpiURL;
        private string appToken = null;
        private string userToken = null;
        private string sessionToken = null;
        private string basicAuth = null;

        public void login()
        {
            dynamic userAuth = new ExpandoObject();
            userAuth.user = user;
            userAuth.password = password;
            ExpandoObject glpiResponse = objectResponse("initSession", null);
            sessionToken = ((dynamic)glpiResponse).result.session_token;
        }

        public bool logout()
        {
            ExpandoObject glpiResponse = objectResponse("killSession", null);
            return true;
        }

        public string jsonResponse(string method, object parameters)
        {
            string jsonParams = null;
            if (parameters != null)
            {
                jsonParams = JsonConvert.SerializeObject(parameters);
            }
            return sendRequest(method, jsonParams);
        }


        public ExpandoObject objectResponse(string method, object parameters)
        {
            string jsonParams = null;
            if (parameters != null)
            {
                jsonParams = JsonConvert.SerializeObject(parameters);
            }
            string jsonResponse = sendRequest(method, jsonParams);
            if(jsonResponse.Length > 0)
            {
                jsonResponse = "{\"result\" : " + jsonResponse + "}";
                Debug.WriteLine(jsonResponse);
            }
            ExpandoObject response = JsonConvert.DeserializeObject<ExpandoObject>(jsonResponse);
            return response;
        }

        private string sendRequest(string method, string jsonParams)
        {
            WebRequest request = WebRequest.Create(glpiURL + method + "/");
            if (basicAuth != null) request.Headers.Add("Authorization", "Basic " + basicAuth);
            request.ContentType = "application/json";
            if (sessionToken != null) request.Headers.Add("Session-Token", sessionToken);
            if (userToken != null) request.Headers.Add("Authorization", "user_token " + userToken);
            if (appToken != null) request.Headers.Add("App-Token", appToken);
            if(jsonParams == null)
            {
                request.Method = "GET";
            }
            else
            {
                request.Method = "POST";
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(jsonParams);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
            }
            string jsonResult;
            WebResponse response = request.GetResponse();
            using (var streamReader = new StreamReader(response.GetResponseStream()))
            {
                jsonResult = streamReader.ReadToEnd();
            }
            return jsonResult;
        }

    }
}