using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using JSON;
namespace ControllerHelpers
{
    public static class ControllerHelper
    {

        public static ActionResult JsonToJson<TRequest, TResponse>(HttpRequest httpRequest,
            Func<TRequest, TResponse> callback)
        {
                string body = new StreamReader(httpRequest.Body).ReadToEnd();
                TRequest request = Json.Deserialize<TRequest>(body);
                TResponse response = callback(request);
                string responseString = Json.Serialize(response);
                return new ContentResult
                {
                    ContentType = "application/json",
                    Content = responseString
                };
        }
        public static ContentResult ToJson<TResponse>(HttpRequest requet, Func<TResponse> callback)
        {
            string body = new StreamReader(requet.Body).ReadToEnd();
            TResponse response = callback();
            string responseString = Json.Serialize(response);
            return new ContentResult
            {
                ContentType = "application/json",
                Content = responseString
            };
        }
    }
}