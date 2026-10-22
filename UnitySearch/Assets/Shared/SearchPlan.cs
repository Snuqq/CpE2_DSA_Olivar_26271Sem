using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace CpE2_DSA_Olivar_26271Sem
{
    // Forms compute the comparisons; Unity only plays this recorded plan.
    public sealed class SearchPlan
    {
        const int MaximumXmlBytes = 64000;
        static readonly XmlSerializer Serializer = new XmlSerializer(typeof(SearchPlan));
        public int[] Values;
        public int Target;
        public string Algorithm;
        public List<SearchStep> Steps;

        public void Validate()
        {
            if (Values == null || Values.Length < 1 || Values.Length > 40)
                throw new ArgumentException("Use 1–40 values.");
            for (int i = 1; i < Values.Length; i++)
                if (Values[i] < Values[i - 1]) throw new ArgumentException("Values must be sorted.");
            if (Algorithm != "Jump Search" && Algorithm != "Exponential Search" && Algorithm != "Fibonacci Search")
                throw new ArgumentException("Select a search algorithm.");
            if (Steps == null || Steps.Count < 1 || Steps.Count > Values.Length + 1)
                throw new ArgumentException("Invalid search step count.");
            for (int i = 0; i < Steps.Count; i++)
            {
                SearchStep step = Steps[i];
                bool terminal = i == Steps.Count - 1;
                if (step == null || step.Terminal != terminal || step.Comparisons != (terminal ? i : i + 1)
                    || step.Index < (terminal ? -1 : 0) || step.Index >= Values.Length
                    || step.Left < 0 || step.Right >= Values.Length || step.Right < -1
                    || step.NextLeft < step.Left || step.NextLeft > Values.Length
                    || step.NextRight > step.Right || step.NextRight < -1
                    || string.IsNullOrEmpty(step.Phase) || string.IsNullOrEmpty(step.Reason)
                    || string.IsNullOrEmpty(step.Comparison) || string.IsNullOrEmpty(step.Message))
                    throw new ArgumentException("Invalid search step.");
                if (!terminal && (step.Index < step.Left || step.Index > step.Right || step.Found))
                    throw new ArgumentException("Invalid search probe.");
                if (terminal && (step.Found != (step.Index >= 0) || (step.Found && Values[step.Index] != Target)))
                    throw new ArgumentException("Invalid search result.");
            }
        }

        public string Serialize()
        {
            Validate();
            using (var text = new StringWriter(CultureInfo.InvariantCulture))
            {
                using (var writer = XmlWriter.Create(text, new XmlWriterSettings { OmitXmlDeclaration = true }))
                {
                    var namespaces = new XmlSerializerNamespaces();
                    namespaces.Add("", "");
                    Serializer.Serialize(writer, this, namespaces);
                }
                string xml = text.ToString();
                if (Encoding.UTF8.GetByteCount(xml) > MaximumXmlBytes) throw new InvalidDataException("Search plan is too large.");
                return xml;
            }
        }

        public static SearchPlan Deserialize(string xml)
        {
            if (xml == null || Encoding.UTF8.GetByteCount(xml) > MaximumXmlBytes)
                throw new InvalidDataException("Invalid search plan size.");
            var settings = new XmlReaderSettings {
                DtdProcessing = DtdProcessing.Prohibit, XmlResolver = null,
                MaxCharactersInDocument = MaximumXmlBytes
            };
            using (var text = new StringReader(xml))
            using (var reader = XmlReader.Create(text, settings))
            {
                var plan = (SearchPlan)Serializer.Deserialize(reader);
                if (plan == null) throw new InvalidDataException("Missing search plan.");
                plan.Validate();
                return plan;
            }
        }
    }
}
