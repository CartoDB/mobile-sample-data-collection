using System;
using System.Collections.Generic;

namespace data.collection
{
    public class Response
    {
        public string Message { get; set; }

        public string Error { get; set; }

        public bool IsOk 
        { 
            get { return string.IsNullOrWhiteSpace(Error); } 
        }
    }
}
