using Examine;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using Umbraco.Core;
using Umbraco.Core.Configuration;
using Umbraco.Core.Models;
using Umbraco.Web;
using Umbraco.Web.Routing;
using Umbraco.Web.Security;

namespace DotSee.ExamineLookupValues
{
    public class ElvBootstrapper : IApplicationEventHandler
    {

        public void OnApplicationInitialized(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
        }

        public void OnApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            var indexer = ExamineManager.Instance.IndexProviderCollection[Elv.Instance.Indexer];
            indexer.GatheringNodeData += GatheringNodeDataHandler;
        }

        public void OnApplicationStarting(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
        }

        void GatheringNodeDataHandler(object sender, IndexingNodeDataEventArgs e)
        {
            var umbHelper = GetHelper();

            //Get names for lookups and add them to the index instead of ids
            foreach (ElvRule r in Elv.Instance.GetRules())
            {
                if (e.Fields["NodeTypeAlias"].Equals(r.DocTypeAlias) && e.Fields.ContainsKey(r.PropertyAlias))
                {
                    try
                    {
                        e.Fields[r.PropertyAlias] = GetPickerValue(e.Fields, r.PropertyAlias, umbHelper).IfNull(x => e.Fields[r.PropertyAlias]);
                    }
                    //We don't want an exception to stop other things from being indexed, so swallow it.
                    //An exception will usually be thrown when the property alias is not an MNTP any more.
                    catch { }
                }
            }
        }

        /// <summary>
        /// Search for the node(s) selected on an MNTP and get the associated node's name(s).
        /// If there are more than one nodes, then a space-delimited string containing all names 
        /// will be returned.
        /// </summary>
        /// <param name="dic">A dictionary containing Examine fields</param>
        /// <param name="fieldName">The property alias to search for</param>
        /// <param name="umbHelper"></param>
        /// <returns></returns>
        private string GetPickerValue(Dictionary<string, string> dic, string fieldName, UmbracoHelper umbHelper)
        {
            if (!dic.ContainsKey(fieldName)) return null;

            string mntpValue = (dic[fieldName] == null) ? "" : dic[fieldName];

            string id = dic["id"];

            System.Text.StringBuilder retVal = new System.Text.StringBuilder(string.Empty);
            foreach (string item in mntpValue.Split(','))
            {
                IPublishedContent c = umbHelper.TypedContent(item);
                retVal.Append(c.Name);
                retVal.Append(" ");
            }
            return (retVal.ToString());
        }

        private UmbracoHelper GetHelper() {
            
            //Get a dummy cached HTTP context
            var dummyHttpContext = (HttpContextWrapper)ApplicationContext.Current.ApplicationCache.RequestCache
                .GetCacheItem("dummyHttpContext",
                () => new HttpContextWrapper(new HttpContext(new SimpleWorkerRequest("dummy.aspx", "", new StringWriter())))
                );

            //Get a cached umbraco helper
            var umbHelper = (UmbracoHelper)ApplicationContext.Current.ApplicationCache.RequestCache
                .GetCacheItem("umbHelperForSearch", () =>
                    new UmbracoHelper(UmbracoContext.EnsureContext(
                    dummyHttpContext,
                    ApplicationContext.Current,
                    new WebSecurity(dummyHttpContext, ApplicationContext.Current),
                    UmbracoConfig.For.UmbracoSettings(),
                    UrlProviderResolver.Current.Providers,
                    false)
            ));

            return (umbHelper);
        }
    }
}