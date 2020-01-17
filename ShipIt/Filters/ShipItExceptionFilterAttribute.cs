using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http.Filters;
using ShipIt.Exceptions;
using ShipIt.Models.ApiModels;

namespace ShipIt.Filters
{
    public class ShipItExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            if (!(context.Exception is ClientVisibleException)) 
            {
                return;
            }         
            var exception = (ClientVisibleException)context.Exception;
            var response = new ErrorResponse()
            {
                Code = exception.ErrorCode,
                Error = exception.Message
            };
            context.Response = new HttpResponseMessage()
            {
                Content = new ObjectContent(typeof(ErrorResponse), response, new XmlMediaTypeFormatter())
            };
        }
    }
}