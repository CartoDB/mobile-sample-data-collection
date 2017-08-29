using System;
using System.Collections.Generic;

namespace data.collection
{
    public class CartoResponse : Response
    {
		public int TotalRows { get; set; }

		public string Hint { get; set; }

		public List<int> InstertedRowIds { get; set; } = new List<int>();
    }
}
