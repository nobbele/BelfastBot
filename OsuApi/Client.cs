using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace OsuApi
{
    public class Client
    {
        public static readonly string BaseUrl = "https://osu.ppy.sh/api/";

        public static async Task<UserResult> GetUser(string user)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                string json = await httpClient.GetStringAsync($"{BaseUrl}get_user?u={user}&k=612b8b552875159a65a733bfcf59e2acc89a4379");

                json = json.TrimStart(new char[] { '[' }).TrimEnd(new char[] { ']' });

                dynamic obj = JObject.Parse(json);

                dynamic jsonResult = (dynamic)obj.ToObject<dynamic>();

                UserResult result = new UserResult
                {
                    UserId = jsonResult.user_id,
                    UserName = jsonResult.username,
                    PP = jsonResult.pp_raw,
                    Level = jsonResult.level,
                    PlayCount = jsonResult.playcount,
                    Accuracy = jsonResult.accuracy,
                };
                return result;
            }
        }
    }
}