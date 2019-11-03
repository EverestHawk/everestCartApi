using System.Net;

namespace Services.ViewModels
{
    public class ResponseMetadata<T>
    {
        public int StatusCode { get; set; }
        public T[] Results { get; set; }
        public string[] Errors { get; set; }
        public bool Succeeded => StatusCode == 200;
        public ResponseMetadata()
        {
            StatusCode = (int)HttpStatusCode.BadRequest;
            Results = null;
            Errors = null;
        }
        public ResponseMetadata(int statusCode, T[] contents, string[] errors)
        {
            StatusCode = statusCode;
            Results = contents;
            Errors = errors;
        }
    }
}
