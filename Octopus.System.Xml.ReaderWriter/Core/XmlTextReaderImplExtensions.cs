using System.IO;
using System.Xml;

namespace Octopus.System.Xml
{
    internal partial class XmlTextReaderImpl
    { 
        public XmlTextReaderImpl(TextReader textReader, bool normalizeLineEndings)
            : this(string.Empty, textReader, normalizeLineEndings)
        {
        }

        public XmlTextReaderImpl(string filename, TextReader textReader, bool normalizeLineEndings)
            : this(textReader, new XmlReaderSettings(), filename, null)
        {
            _normalize = normalizeLineEndings;
            _ps.eolNormalized = !_normalize;
        }
    }
}
