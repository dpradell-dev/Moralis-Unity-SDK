// Depends on UniTask to support cancellation token and GetAwaiter: https://github.com/Cysharp/UniTask
// Otherwise, the code can be adapted using https://gist.github.com/krzys-h/9062552e33dd7bd7fe4a6c12db109a1a

using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;

namespace MoralisUnity.Tools
{
	public interface IHttpClient {
	public Uri BaseAddress { get; set; }
	public HttpRequestHeaders DefaultRequestHeaders { get; }

	public UniTask<HttpResponseMessage> SendAsync(HttpRequestMessage message, HttpCompletionOption option,
		CancellationToken token);

	public void Dispose();
}

	public class UwrHttpClient : IHttpClient
	{
		public UwrHttpClient(){}

		/*
		public UwrHttpClient(string baseUri)
		{
			BaseAddress = new Uri(baseUri);
		}

		public UwrHttpClient(Uri baseUri)
		{
			BaseAddress = baseUri;
		}
		 */

		public Uri BaseAddress { get; set; } // We don't use it but we need it because we are an IHttpClient
		public HttpRequestHeaders DefaultRequestHeaders => _httpClient.DefaultRequestHeaders;

		private readonly HttpClient _httpClient = new HttpClient();

		public async UniTask<HttpResponseMessage> SendAsync(HttpRequestMessage message, HttpCompletionOption option,
			CancellationToken token)
		{
			var content = await (message.Content?.ReadAsStringAsync() ?? Task.FromResult(""));
			var webRequest = GetUnityWebRequest(message.Method.Method, message.RequestUri, content);
			
			AppendHeaders(webRequest);

			try
			{
				await webRequest
					.SendWebRequest();//.WithCancellation(cancellationToken: token);
			}
			catch (Exception)
			{
				webRequest.Dispose();
				throw;
			}

			var responseMessage = CreateHttpResponseMessage(webRequest);
			webRequest.Dispose();
			return responseMessage;
		}

		public void Dispose()
		{
			_httpClient.Dispose();
			DefaultRequestHeaders.Clear();
			BaseAddress = null;
		}

		private UnityWebRequest GetUnityWebRequest(string method, Uri endpoint, string content = "") {
			//var requestUri = BaseAddress.AbsoluteUri + endpoint;
			var webRequest = UnityWebRequest.Get(endpoint);
			
			webRequest.method = method;
			webRequest.disposeUploadHandlerOnDispose = true;
			webRequest.disposeDownloadHandlerOnDispose = true;
			
			if (!string.IsNullOrEmpty(content)) {
				var data = new System.Text.UTF8Encoding().GetBytes(content);
				webRequest.uploadHandler = new UploadHandlerRaw(data);
				webRequest.SetRequestHeader("Content-Type", "application/json");
			}
			return webRequest;
		}

		private HttpResponseMessage CreateHttpResponseMessage(UnityWebRequest webRequest)
		{
			var response = new HttpResponseMessage();
			var responseContent = webRequest.downloadHandler?.text;
			
			response.Content = new StringContent(responseContent);
			response.StatusCode = (HttpStatusCode) webRequest.responseCode;

			return response;
		}
		
		private void AppendHeaders(UnityWebRequest webRequest)
		{
			using var enumerator = DefaultRequestHeaders.GetEnumerator();

			while (enumerator.MoveNext())
			{
				var (key, value) = enumerator.Current;
				webRequest.SetRequestHeader(key, value.First());
			}
		}
	}
}
