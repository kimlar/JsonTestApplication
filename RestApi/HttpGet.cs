using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Http;
using JsonTestApplication.Account;

namespace JsonTestApplication.RestApi
{
	class HttpGet
	{
		public string uri { get; set; }
		public string accessToken { get; set; }

		public string Get(string uri)
		{
			this.uri = uri;
			var result = Task.Run(() => GetStringAsync());

			return result.Result;
		}

        public async Task<string> GetStringAsync()
		{
			var httpClient = new HttpClient();
			httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

			string result = null;

			// Always catch network exceptions for async methods
			try
			{
				result = await httpClient.GetStringAsync(new Uri(uri));
			}
			catch (Exception ex)
			{
				// Details in ex.Message and ex.HResult.       
				string msg = ex.Message;
			}

			// Once your app is done using the HttpClient object call dispose to 
			// free up system resources (the underlying socket and memory used for the object)
			httpClient.Dispose();

			return result;
		}

	}
}

