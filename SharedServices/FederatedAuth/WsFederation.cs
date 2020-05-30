using SharedServices.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SharedServices.FederatedAuth
{
    public class WsFederation
    {
        public static string wa = "wsignin1.0";
        public string wtrealm { get; internal set; }
        public int? wfresh { get; internal set; }
        public string wauth { get; internal set; }
        public string wreq { get; internal set; }
        public string wreply { get; internal set; }
        public string wctx { get; internal set; }
        public string whr { get; internal set; }
        public WsFederation() { }


        public WsFederation(Uri uri) {
            UriBuilder uriBuilder = new UriBuilder(uri);
            NameValueCollection query = HttpUtility.ParseQueryString(uriBuilder.Query);
            wtrealm = HttpUtility.UrlDecode(query.GetValue("wtrealm"));
            wfresh = query.GetValueInt("wfresh");
            wauth = query.GetValue("wauth");
            wreq = query.GetValue("wreq");
            wreply = HttpUtility.UrlDecode(query.GetValue("wreply"));
            wctx = query.GetValue("wctx");
            whr = query.GetValue("whr");
        }
        public string BuildWsFederationUrl(Uri baseUri, Uri returnUri= null)
        {
            UriBuilder uriBuilder = new UriBuilder(baseUri);
            NameValueCollection query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query.SetValue("wa", wa);
            query.SetValue("wtrealm", HttpUtility.UrlEncode(wtrealm));
            query.SetValue("wfresh", wfresh);
            query.SetValue("wauth", wauth);
            query.SetValue("wreq", wreq);
            query.SetValue("wreply", returnUri != null ? HttpUtility.UrlEncode(returnUri.ToString()) : HttpUtility.UrlEncode(wreply));
            query.SetValue("wctx", wctx);
            query.SetValue("whr", whr);
           
            uriBuilder.Query = query.ToString();
            return uriBuilder.ToString();
        }
        public Uri GetResponseUri(Uri returnUri = null) => new Uri(GetResponseUrl(returnUri));
        public string GetResponseUrl(Uri returnUri = null)
        {
            UriBuilder uriBuilder = new UriBuilder(new Uri(wreply));
            NameValueCollection query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query.SetValue("wa", wa);
            query.SetValue("wtrealm", HttpUtility.UrlEncode(wtrealm));
            query.SetValue("wfresh", wfresh);
            query.SetValue("wauth", wauth);
            query.SetValue("wreq", wreq);
            query.SetValue("wreply", returnUri != null ? HttpUtility.UrlEncode(returnUri.ToString()): null);
            query.SetValue("wctx", wctx);
            query.SetValue("whr", whr);

            uriBuilder.Query = query.ToString();
            return uriBuilder.ToString();
        }


    }
}
