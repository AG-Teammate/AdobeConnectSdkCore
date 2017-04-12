using AdobeConnectSDK.Common;
using System.Xml.Serialization;

namespace AdobeConnectSDK.Model
{
    [XmlRoot("sco")]
    public class Sco : XmlDateTimeBase
    {
        [XmlAttribute("sco-id")]
        public string ScoId;

        [XmlAttribute("folder-id")]
        public string FolderId;

        [XmlIgnore]
        public SCOtype ItemType;

        [XmlAttribute("type")]
        internal string ItemTypeRaw
        {
            get
            {
                return Helpers.EnumToString(this.ItemType);
            }
            set
            {
                this.ItemType = Helpers.ReflectEnum<SCOtype>(value);
            }
        }

        [XmlAttribute("icon")]
        public string Icon;

        [XmlElement("name")]
        public string Name;

        [XmlElement("lang")]
        public string Language;

        [XmlElement("url-path")]
        public string UrlPath;

        [XmlElement("byte-count")]
        public long ByteCount;

        [XmlIgnore]
        public string FullUrl;

        [XmlIgnore]
        public byte[] Data;
    }
}
