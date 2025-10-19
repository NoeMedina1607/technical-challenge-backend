using EnterpriseApp.Application.Dtos;
using EnterpriseApp.Application.Interfaces;
using EnterpriseApp.Application.Options;
using HtmlAgilityPack;
using Microsoft.Extensions.Options;
using System.Net;
using System.Text;
using System.Web;

namespace EnterpriseApp.Application.Services
{
    public sealed class DGIIService : IDgiiService
    {
        private readonly string _baseUrl;

        public DGIIService(IOptions<DGIIOptions> options)
        {
            _baseUrl = options.Value.RncWebUrl?.Trim() ?? throw new InvalidOperationException("Dgii:RncWebUrl not configured");
        }

        private static HttpClient CreateClient(out CookieContainer cookies)
        {
            cookies = new CookieContainer();
            var handler = new HttpClientHandler
            {
                CookieContainer = cookies,
                AutomaticDecompression = DecompressionMethods.GZip
                                       | DecompressionMethods.Deflate
                                       | DecompressionMethods.Brotli
            };

            var client = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromSeconds(30)
            };

            client.DefaultRequestHeaders.UserAgent.ParseAdd(
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 " +
                "(KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
            client.DefaultRequestHeaders.Accept.ParseAdd("text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");

            return client;
        }

        public async Task<DgiiCompanyDto?> GetByRncAsync(string rnc, CancellationToken ct)
        {
            using var client = CreateClient(out _);

            var getResp = await client.GetAsync(_baseUrl, ct);
            if (!getResp.IsSuccessStatusCode) return null;

            var getHtml = await getResp.Content.ReadAsStringAsync(ct);
            if (string.IsNullOrWhiteSpace(getHtml) || !getHtml.Contains("__VIEWSTATE"))
            {
                var raw = await getResp.Content.ReadAsByteArrayAsync(ct);
                var charset = getResp.Content.Headers.ContentType?.CharSet;
                getHtml = Decode(raw, charset);
            }

            var (viewState, eventValidation, viewStateGen) = ExtractStateFields(getHtml);
            if (string.IsNullOrEmpty(viewState) || string.IsNullOrEmpty(eventValidation))
                return null;

            var form = new List<KeyValuePair<string, string>>
            {
                new("__EVENTTARGET",    "ctl00$cphMain$btnBuscarPorRNC"),
                new("__EVENTARGUMENT",  ""),
                new("__VIEWSTATE",      viewState),
                new("__EVENTVALIDATION", eventValidation),
                new("__VIEWSTATEGENERATOR", viewStateGen ?? string.Empty),
                new("ctl00$cphMain$txtRNCCedula", rnc),
                new("ctl00$cphMain$hidActiveTab", "rnc"),
            };

            using var postContent = new FormUrlEncodedContent(form);
            var postResp = await client.PostAsync(_baseUrl, postContent, ct);
            if (!postResp.IsSuccessStatusCode) return null;

            var postHtml = await postResp.Content.ReadAsStringAsync(ct);

            var doc = new HtmlDocument();
            doc.LoadHtml(postHtml);

            var table = doc.DocumentNode.SelectSingleNode("//*[@id='cphMain_dvDatosContribuyentes']");
            if (table is null) return null;

            var data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var tr in table.SelectNodes(".//tr") ?? Enumerable.Empty<HtmlNode>())
            {
                var tds = tr.SelectNodes("./td");
                if (tds == null || tds.Count < 2) continue;

                var key = Clean(tds[0].InnerText);
                var val = Clean(tds[1].InnerText);
                if (!string.IsNullOrEmpty(key)) data[key] = val;
            }

            string Pick(params string[] keys)
            {
                foreach (var k in keys)
                    if (data.TryGetValue(k, out var v) && !string.IsNullOrWhiteSpace(v))
                        return v.Trim();
                return "N/D";
            }

            var dto = new DgiiCompanyDto(
                Identification: Pick("Cédula/RNC", "Cedula/RNC", "Cédula / RNC"),
                CompanyName: Pick("Nombre/Razón Social", "Nombre / Razón Social", "Nombre/Razon Social"),
                TradeName: Pick("Nombre Comercial", "Nombre  Comercial"),
                Category: NormalizeNullable(Pick("Categoría", "Categoria")),
                PaymentScheme: Pick("Régimen de pagos", "Regimen de pagos", "Régimen de Pagos"),
                Status: Pick("Estado"),
                EconomicActivity: Pick("Actividad Economica", "Actividad Económica"),
                GovernmentBranch: Pick("Administracion Local", "Administración Local")
            );

            if (dto.Identification == "N/D") dto = dto with { Identification = rnc };
            if (dto.TradeName == "N/D" && dto.CompanyName != "N/D") dto = dto with { TradeName = dto.CompanyName };

            return dto;
        }

        private static (string viewState, string eventValidation, string? viewStateGen) ExtractStateFields(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            string GetVal(string id) =>
                HttpUtility.HtmlDecode(
                    doc.DocumentNode.SelectSingleNode($"//input[@id='{id}']")
                       ?.GetAttributeValue("value", string.Empty) ?? string.Empty
                ).Trim();

            var vs = GetVal("__VIEWSTATE");
            var ev = GetVal("__EVENTVALIDATION");
            var vsg = GetVal("__VIEWSTATEGENERATOR");

            return (vs, ev, string.IsNullOrWhiteSpace(vsg) ? null : vsg);
        }

        private static string Clean(string? s)
        {
            if (string.IsNullOrEmpty(s)) return string.Empty;
            var t = HtmlEntity.DeEntitize(s).Trim();

            var sb = new StringBuilder(t.Length);
            bool prevSpace = false;
            foreach (var ch in t)
            {
                if (char.IsWhiteSpace(ch))
                {
                    if (!prevSpace) { sb.Append(' '); prevSpace = true; }
                }
                else { sb.Append(ch); prevSpace = false; }
            }
            return sb.ToString();
        }

        private static string? NormalizeNullable(string val)
            => string.IsNullOrWhiteSpace(val) || val.Equals("N/D", StringComparison.OrdinalIgnoreCase) ? null : val;

        private static string Decode(byte[] bytes, string? charset)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(charset))
                    return Encoding.GetEncoding(charset).GetString(bytes);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return Encoding.UTF8.GetString(bytes);
        }
    }
}
