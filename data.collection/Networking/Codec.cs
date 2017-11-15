
using System;
using System.Collections.Generic;
using Carto.Core;

namespace data.collection
{
    public class Codec
    {
        public const string Schema = Conf.Schema;
        public const string FunctionName = Conf.FunctionName;

		public const string BaseUrl = "https://" + Schema + ".carto.com/api/v2/sql";
		public const string SQLUrl = BaseUrl + "?q=";

        public const string InternalServerError = "Internal Server Error";

        public static string DataToSql(List<Data> data)
        {
            string[] jsons = new string[data.Count];

            for (int i = 0; i < data.Count; i++)
            {
                jsons[i] = "'" + data[i] + "'";
            }

            string sql = "SELECT " + Schema + "." + FunctionName + "(ARRAY[";

            for (int i = 0; i < jsons.Length; i++)
            {
                string item = jsons[i];
                sql += item;

                if (i < jsons.Length - 1)
                {
                    sql += ",";
                }
            }

            sql += "]::jsonb[])";

            return sql;
        }

        public static CartoResponse DecodePostResponse(string encoded)
		{
            CartoResponse response = new CartoResponse();

            if (encoded.Contains(InternalServerError))
            {
                response.Error = InternalServerError;
                return response;
            }

			Variant json = Variant.FromString(encoded);

			Variant error = json.GetObjectElement("error");

			if (!error.String.Equals("null"))
			{
				response.Error = error.GetArrayElement(0).String;
				response.Hint = json.GetObjectElement("hint").String;
				return response;
			}

			Variant rows = json.GetObjectElement("rows");

			for (int i = 0; i < rows.ArraySize; i++)
			{
				Variant row = rows.GetArrayElement(i);
				int id = int.Parse(row.GetObjectElement(FunctionName).String);
				response.InstertedRowIds.Add(id);
			}

			response.TotalRows = int.Parse(json.GetObjectElement("total_rows").String);

			response.Message = "Great success! " + response.TotalRows + " row(s) uploaded";

			return response;
		}
    }
}
