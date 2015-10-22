using System;
using System.Xml;
using System.Xml.Schema;

namespace Vega.DbUpgrade.Utilities
{
    /// <summary>
    /// Contains methods for XML validation
    /// </summary>
    public class XmlValidator
    {
        #region [Members]

        /// <summary>
        /// Singleton instance.
        /// </summary>
        private static readonly XmlValidator Instance = new XmlValidator();

        /// <summary>
        /// Current XML file that validation is performed on.
        /// </summary>
        private string _xmlFileName;

        #endregion
        
        #region [Public Methods]

        /// <summary>
        /// Singleton instance.
        /// </summary>
        /// <returns><see cref="Vega.DbUpgrade.Utilities.XmlValidator"/> instance.</returns>
        public static XmlValidator GetInstance()
        {
            return Instance;
        }

        /// <summary>
        /// Validates XML definition against given XSD schema.
        /// </summary>
        /// <param name="xmlFileName">Full path to the XML file on file system.</param>
        /// <param name="schemaContent">XSD schema.</param>
        /// <param name="schemaName">XSD schema name.</param>
        public void Validate(string xmlFileName, string schemaContent, string schemaName)
        {
            _xmlFileName = xmlFileName;

            var xmlDoc = GetUpdatedXml(xmlFileName, schemaName);
            var xmlSchemaSet = GetXmlSchema(schemaContent, schemaName);            

            var nt = new NameTable();
            var nsmgr = new XmlNamespaceManager(nt);
            var context = new XmlParserContext(null, nsmgr, null, XmlSpace.None);

            var settings = new XmlReaderSettings();
            settings.ValidationType = ValidationType.Schema;
            settings.Schemas.Add(xmlSchemaSet);
            settings.ValidationEventHandler += ValidationCallbackOne;

            using (var xmlr = new XmlTextReader(xmlDoc.OuterXml, XmlNodeType.Document, context))
            {
                using (var reader = XmlReader.Create(xmlr, settings))
                {
                    while (reader.Read())
                    {
                        var val = reader.Value;
                    }
                }
            }
        }

        #endregion
        
        #region [Private Methods]
        /// <summary>
        /// Updates XML file with XML schema namespaces.
        /// </summary>
        /// <param name="xmlFileName">Full path to the XML file on file system.</param>
        /// <param name="schemaName">Schema name.</param>
        /// <returns>Updated XML document.</returns>
        private static XmlDocument GetUpdatedXml(string xmlFileName, string schemaName)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlFileName);

            var xmlns = xmlDoc.CreateAttribute("xmlns");
            var xmlnsXsi = xmlDoc.CreateAttribute("xmlns:xsi");

            xmlns.Value = schemaName;
            xmlnsXsi.Value = "http://www.w3.org/2001/XMLSchema-instance";

            if (xmlDoc.DocumentElement != null)
            {
                xmlDoc.DocumentElement.Attributes.Append(xmlns);
                xmlDoc.DocumentElement.Attributes.Append(xmlnsXsi);
            }

            return xmlDoc;
        }

        /// <summary>
        /// Gets XML schema based on schema content and schema name.
        /// </summary>
        /// <param name="schemaContent">Content of XML schema.</param>
        /// <param name="schemaName">Schema name.</param>
        /// <returns>
        ///   <see cref="System.Xml.Schema.XmlSchemaCollection" /> instance.
        /// </returns>
        private static XmlSchemaSet GetXmlSchema(string schemaContent, string schemaName)
        {
            var schemas = new XmlSchemaSet();
            var nt = new NameTable();
            var nsmgr = new XmlNamespaceManager(nt);
            var context = new XmlParserContext(null, nsmgr, null, XmlSpace.None);

            using (var tr = new XmlTextReader(schemaContent, XmlNodeType.Document, context))
            {
                schemas.Add(schemaName, tr);
            }

            return schemas;
        }

        #endregion

        #region [Event Handlers]

        /// <summary>
        /// Event handler of XML validator.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args"><see cref="System.Xml.Schema.ValidationEventArgs" /> instance.</param>
        /// <exception cref="System.Xml.Schema.XmlSchemaValidationException"></exception>
        private void ValidationCallbackOne(object sender, ValidationEventArgs args)
        {
            var errorMessage = String.Format(Constants.Messages.IncorrectFormatOfXmlFile, _xmlFileName, args.Message);
            throw new XmlSchemaValidationException(errorMessage);
        }

        #endregion
    }
}