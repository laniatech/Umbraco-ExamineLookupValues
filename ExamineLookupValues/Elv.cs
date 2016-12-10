using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using umbraco;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using System.Web.Hosting;
using System.IO;

namespace DotSee.ExamineLookupValues
{
    /// <summary>
    /// Creates new nodes under a newly created node, according to a set of rules
    /// </summary>
    public sealed class Elv
    {

        #region Private Members
        /// <summary>
        /// Lazy singleton instance member
        /// </summary>
        private static readonly Lazy<Elv> _instance = new Lazy<Elv>(() => new Elv());

        /// <summary>
        /// The list of rule objects
        /// </summary>
        private List<ElvRule> _rules;


        #endregion

        #region Public Properties

        /// <summary>
        /// The name of the indexer to use
        /// </summary>
        public string Indexer { get; set; }
        
        #endregion

        #region Constructors

        /// <summary>
        /// Returns a (singleton) AutoNode instance
        /// </summary>
        public static Elv Instance { get { return _instance.Value; } }


        /// <summary>
        /// Private constructor for Singleton
        /// </summary>
        private Elv()
        {
            _rules = new List<ElvRule>();

            ///Get rules from the config file. Any rules programmatically declared later on will be added too.
            GetRulesFromConfigFile();
        }

        #endregion

        #region Public Methods
        public void RefreshRules()
        {
            GetRulesFromConfigFile();
        }

        /// <summary>
        /// Registers a new rule object 
        /// </summary>
        /// <param name="rule">The rule object</param>
        public void RegisterRule(ElvRule rule)
        {
            _rules.Add(rule);
        }

        public List<ElvRule> GetRules()
        {
            return _rules;
        }

        public string[] GetDistinctDocTypes()
        {
            return (_rules.Select(x => x.DocTypeAlias).Distinct().ToArray());
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets rules from /config/autoNode.config file (if it exists)
        /// </summary>
        private void GetRulesFromConfigFile()
        {
            XmlDocument xmlConfig = new XmlDocument();

            try
            {
                xmlConfig.Load(HostingEnvironment.MapPath(GlobalSettings.Path + "/../config/DotSee.Elv.config"));
            }
            catch (FileNotFoundException ex)
            {
                Umbraco.Core.Logging.LogHelper.Error(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "Elv: Configuration file was not found.", ex);
                return;
            }
            catch (Exception ex)
            {
                Umbraco.Core.Logging.LogHelper.Error(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "Elv: There was a problem loading Elv configuration from the config file", ex);
                return;
            }
            Umbraco.Core.Logging.LogHelper.Info(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "Elv: Loading configuration...");

            Indexer = xmlConfig.SelectSingleNode("/elv").Attributes["indexer"].Value;

            foreach (XmlNode xmlConfigEntry in xmlConfig.SelectNodes("/elv/rule"))
            {
                if (xmlConfigEntry.NodeType == XmlNodeType.Element)
                {
                    string docTypeAlias = xmlConfigEntry.Attributes["docTypeAlias"].Value;
                    string propertyAlias = xmlConfigEntry.Attributes["propertyAlias"].Value;
                    var rule = new ElvRule(docTypeAlias, propertyAlias);

                    _rules.Add(rule);
                }
            }
            Umbraco.Core.Logging.LogHelper.Info(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "Elv: Loading configuration complete");
        }

        #endregion
    }
}