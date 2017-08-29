using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace data.collection
{
    public class Networking
    {
        public static async Task<CartoResponse> Post(Data item)
        {
            return await Post(new List<Data> { item });
        }

        public static async Task<CartoResponse> Post(List<Data> data)
		{
			CartoResponse response = new CartoResponse();

			if (data.Count == 0)
			{
				// Realistically should never reach this block. 
				// Size check done before. Here for debugging purposes
				response.Error = "No data to send";
				return response;
			}

			string url = Codec.BaseUrl;
			string sql = Codec.DataToSql(data);

			using (var client = new HttpClient())
			{
				var stringPayload = "{ \"q\": \"" + sql.Replace("\"", @"\""") + "\" }";

				var content = new StringContent(stringPayload, Encoding.UTF8, "application/json");

				HttpResponseMessage httpResponse = null;

				try
				{
					httpResponse = await client.PostAsync(url, content);
				}
				catch (Exception e)
				{
					response.Error = e.Message;
					return response;
				}

				var responseString = await httpResponse.Content.ReadAsStringAsync();

				response = Codec.DecodePostResponse(responseString);
			}

			// For debugging purposes -> Get the row you just inserted
			//await GetById(response.InstertedRowIds[0]);

			return response;
		}

    }
}
