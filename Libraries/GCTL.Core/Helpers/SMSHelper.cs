using GCTL.Core.Configurations;
using System.Net;
using System.Text;

namespace GCTL.Core.Helpers
{
    public static class SMSHelper
    {
        public static async Task SendSMS(this SMSSetting setting, List<string> phoneNumbers, string message)
        {
           // string result = "";
            try
            {
                if (setting.IsEnabled)
                {
                    string number = string.Join(",", phoneNumbers);
                    message = System.Uri.EscapeDataString(message); //do not use single quotation (') in the message to avoid forbidden result
                    string url = $"http://66.45.237.70/api.php?username={setting.UserId}&password={setting.Password}&number={number}&message={message}";
                    using (var client = new HttpClient())
                    {
                        var response = await client.GetAsync(url);
                        if (response.IsSuccessStatusCode)
                        {
                            //result = await response.Content.ReadAsStringAsync();
                            //Stream stream = response.Content.ReadAsStream();
                            //Encoding ec = System.Text.Encoding.GetEncoding("utf-8");
                            //StreamReader reader = new System.IO.StreamReader(stream, ec);
                            //result = reader.ReadToEnd();
                            //reader.Close();
                            //stream.Close();
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.ToString());
            }
            finally
            {
            }
        }
        public static async Task SendSMS2(this SMSSetting setting, List<string> phoneNumbers, string message)
        {
            string result = "";
            WebRequest request = null;
            HttpWebResponse response = null;
            if (setting.IsEnabled)
            {
                try
                {
                    string number = string.Join(",", phoneNumbers);
                    message = System.Uri.EscapeDataString(message); //do not use single quotation (') in the message to avoid forbidden result
                    string url = $"http://66.45.237.70/api.php?username={setting.UserId}&password={setting.Password}&number={number}&message={message}";

                    request = WebRequest.Create(url);

                    // Send the 'HttpWebRequest' and wait for response.
                    response = (HttpWebResponse)request.GetResponse();
                    Stream stream = response.GetResponseStream();
                    Encoding ec = System.Text.Encoding.GetEncoding("utf-8");
                    StreamReader reader = new System.IO.StreamReader(stream, ec);
                    result = await reader.ReadToEndAsync();
                    //Console.WriteLine(result);
                    reader.Close();
                    stream.Close();
                }
                catch (Exception exp)
                {
                    //Console.WriteLine(exp.ToString());
                }
                finally
                {
                    if (response != null)
                        response.Close();
                }
            }
        }
    }
}
