using System.Xml.Serialization;

namespace MiniApi.ResponseFormats;

public class XmlResult<T> : IResult
{
    private static readonly XmlSerializer Serializer = new(typeof(T));
    private readonly T _result;

    public XmlResult(T result) => _result = result;

    public Task ExecuteAsync(HttpContext httpContext)
    {
        using var ms = new MemoryStream();
        Serializer.Serialize(ms, _result);
        httpContext.Response.ContentType = "application/xml";
        ms.Position = 0;
        return ms.CopyToAsync(httpContext.Response.Body);
    }
}