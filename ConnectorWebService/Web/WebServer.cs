using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace ConnectorWebService.Web
{

	[ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single)]
	[ServiceContract]
	public class WebServer
	{
		public const string Name = "Web";
		private static readonly Assembly CurrentAssembly = typeof(WebServer).Assembly;

		private static readonly NameValueCollection MimeDictionary = new NameValueCollection
		{
			{".css" , "text/css"},
			{".html", "text/html"},
			{".js" , "text/javascript"},
			{".gif" , "image/gif"},
		};

		[OperationContract]
		[WebGet(UriTemplate = "*")]
		[OperationBehavior(AutoDisposeParameters = true)]
		public Stream GetFile()
		{
			var context = WebOperationContext.Current;
			var filePathParts = context.IncomingRequest.UriTemplateMatch.WildcardPathSegments;
			if (!filePathParts.Any())
			{
				filePathParts.Add("index.html");
			}
			string filePath = filePathParts.Aggregate(Name, Path.Combine);

			Stream result = TryLoadFile(filePath) ?? TryLoadResource(filePath);

			if (result == null)
			{
				throw new WebFaultException<string>("File not found", HttpStatusCode.NotFound);
			}

			try
			{
				context.OutgoingResponse.ContentType = GetMimeType(Path.GetExtension(filePath));
				//WCF will take care of calling Dispose() for file stream
				return result;

			}
			catch (Exception e)
			{
				throw new WebFaultException<string>($"File cannot be read: {e.Message}", HttpStatusCode.InternalServerError);
			}
		}

		private static Stream TryLoadResource(string path)
		{
			string resourcePath = "ConnectorWebService." + path.Replace('/', '.').Replace('\\', '.');
			return CurrentAssembly.GetManifestResourceStream(resourcePath);
		}

		private static Stream TryLoadFile(string path)
		{
			if (!File.Exists(path))
				return null;

			try
			{
				return new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
			}
			catch (Exception e)
			{
				throw new WebFaultException<string>($"File cannot be read: {e.Message}", HttpStatusCode.InternalServerError);
			}
		}

		private static string GetMimeType(string ext)
		{
			return MimeDictionary[ext] ?? "application/octet-stream";
		}
	}
}
