using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WaDesktop.Domain.Common;

namespace WaDesktop.Infrastructure.Data.Remote.DataSources
{
    /// <summary>
    /// Basis DataSource ala onpay: HTTP tipis, respons dibungkus Result,
    /// unwrap standar backend { "data": ..., "message": ... }.
    /// </summary>
    public abstract class BaseDataSource
    {
        protected readonly HttpClient Http;
        protected readonly string BaseUrl;

        protected BaseDataSource(HttpClient http, string baseUrl)
        {
            Http = http;
            BaseUrl = baseUrl;
        }

        // ── Eksekusi inti ──

        protected async Task<Result<string>> SendAsync(HttpMethod method, string path, object body)
        {
            try
            {
                using (var request = BuildRequest(method, path, body))
                using (var response = await Http.SendAsync(request))
                {
                    var content = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                        return Result<string>.Success(content);

                    return Result<string>.Failure(MapStatusCode(response.StatusCode, content));
                }
            }
            catch (HttpRequestException ex) when (ex.Message == "Session expired")
            {
                return Result<string>.Failure(Error.Unauthorized());
            }
            catch (Exception ex)
            {
                return Result<string>.Failure(Error.Network(ex.Message));
            }
        }

        // ── Parser standar (dipakai subclass) ──

        protected Result<T> ParseData<T>(string json)
        {
            try
            {
                var wrapper = JObject.Parse(json);
                var data = wrapper["data"];
                return Result<T>.Success(data != null ? data.ToObject<T>() : default(T));
            }
            catch (Exception ex)
            {
                return Result<T>.Failure(Error.Unknown("Respons tidak valid: " + ex.Message));
            }
        }

        protected Result<List<T>> ParseList<T>(string json)
        {
            try
            {
                var wrapper = JObject.Parse(json);
                var data = wrapper["data"] as JArray;
                var list = data != null ? data.ToObject<List<T>>() : new List<T>();
                return Result<List<T>>.Success(list ?? new List<T>());
            }
            catch (Exception ex)
            {
                return Result<List<T>>.Failure(Error.Unknown("Respons tidak valid: " + ex.Message));
            }
        }

        // ── Helper ──

        /// <summary>GET biner (gambar). Semantik khusus: 404 mengembalikan Success(null).</summary>
        protected async Task<Result<byte[]>> GetBytesAsync(string url)
        {
            try
            {
                using (var response = await Http.GetAsync(url))
                {
                    if (response.StatusCode == HttpStatusCode.NotFound)
                        return Result<byte[]>.Success(null);

                    if (!response.IsSuccessStatusCode)
                    {
                        var errBody = await response.Content.ReadAsStringAsync();
                        return Result<byte[]>.Failure(MapStatusCode(response.StatusCode, errBody));
                    }

                    return Result<byte[]>.Success(await response.Content.ReadAsByteArrayAsync());
                }
            }
            catch (Exception ex)
            {
                return Result<byte[]>.Failure(Error.Network(ex.Message));
            }
        }

        /// <summary>Kirim HttpContent mentah (mis. multipart upload).</summary>
        protected async Task<Result<string>> SendContentAsync(HttpMethod method, string path, HttpContent content)
        {
            try
            {
                using (var request = new HttpRequestMessage(method, path) { Content = content })
                using (var response = await Http.SendAsync(request))
                {
                    var body = await response.Content.ReadAsStringAsync();
                    return response.IsSuccessStatusCode
                        ? Result<string>.Success(body)
                        : Result<string>.Failure(MapStatusCode(response.StatusCode, body));
                }
            }
            catch (HttpRequestException ex) when (ex.Message == "Session expired")
            {
                return Result<string>.Failure(Error.Unauthorized());
            }
            catch (Exception ex)
            {
                return Result<string>.Failure(Error.Network(ex.Message));
            }
        }

        private static HttpRequestMessage BuildRequest(HttpMethod method, string path, object body)
        {
            var request = new HttpRequestMessage(method, path);
            if (body != null)
            {
                var json = JsonConvert.SerializeObject(body);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            }
            return request;
        }

        private static Error MapStatusCode(HttpStatusCode status, string body)
        {
            switch ((int)status)
            {
                case 400:
                case 422:
                    return Error.Validation(ExtractMessage(body) ?? "Permintaan tidak valid");
                case 401:
                    return Error.Unauthorized(ExtractMessage(body));
                case 403:
                    return new Error(ErrorType.Forbidden, ExtractMessage(body) ?? "Akses ditolak");
                case 404:
                    return new Error(ErrorType.NotFound, ExtractMessage(body) ?? "Data tidak ditemukan");
                case 409:
                    return new Error(ErrorType.Conflict, ExtractMessage(body) ?? "Konflik data");
                default:
                    if ((int)status >= 500)
                        return Error.Server(ExtractMessage(body) ?? $"Server error ({(int)status})");
                    return Error.Unknown(ExtractMessage(body) ?? $"Request failed ({(int)status})");
            }
        }

        /// <summary>Mirip ApiErrorResponse di Core.cs: message / error / errors.</summary>
        private static string ExtractMessage(string body)
        {
            if (string.IsNullOrEmpty(body)) return null;
            try
            {
                var obj = JObject.Parse(body);

                var direct = obj["message"];
                if (direct != null && direct.Type == JTokenType.String)
                    return direct.ToString();

                var token = obj["error"] ?? obj["errors"];
                if (token == null) return null;

                if (token.Type == JTokenType.String)
                    return token.ToString();

                if (token.Type == JTokenType.Object)
                    return token["message"]?.ToString();

                if (token.Type == JTokenType.Array)
                {
                    var parts = new List<string>();
                    foreach (var item in token)
                    {
                        var field = item["field"]?.ToString();
                        var msg = item["message"]?.ToString();
                        parts.Add(string.IsNullOrEmpty(field) ? msg : $"{field}: {msg}");
                    }
                    if (parts.Count > 0) return string.Join("\n", parts);
                }
            }
            catch { /* bukan JSON — biarkan fallback */ }
            return null;
        }
    }
}
