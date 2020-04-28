using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cw5.DTOs.Responses;

namespace cw5.Models
{
    public class Response
    {
        public string Type { get; set; }
        public string Message { get; set; }
        public Object ResponseObject { get; set; }

        public Response(string Type, string Message)
        {
            this.Type = Type;
            this.Message = Message;
        }

        public Response(string Type, string Message, IResponse ResponseObject)
        {
            this.Type = Type;
            this.Message = Message;
            this.ResponseObject = ResponseObject;
        }

    }
}
