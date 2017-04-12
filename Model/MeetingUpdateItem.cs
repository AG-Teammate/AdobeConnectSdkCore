/*
Copyright 2007-2014 Dmitry Stroganov (dmitrystroganov.dk)
Redistributions of any form must retain the above copyright notice.
 
Use of any commands included in this SDK is at your own risk. 
Dmitry Stroganov cannot be held liable for any damage through the use of these commands.
*/

using System.Xml.Serialization;

namespace AdobeConnectSDK.Model
{
    /// <summary>
    /// MeetingUpdateItem structure
    /// </summary>

    [XmlRoot("MeetingUpdateItem")]
    public class MeetingUpdateItem : XmlDateTimeBase
    {
        [XmlAttribute("sco-id")]
        public string ScoId;

        [XmlAttribute("folder-id")]
        public string FolderId;

        [XmlElement("name")]
        public string Name;

        [XmlElement("description")]
        public string Description;

        [XmlElement("lang")]
        public string Language;

        [XmlElement("sco-tag")]
        public string ScoTag;

        [XmlElement]
        public string Email;

        [XmlElement("first-name")]
        public string FirstName;

        [XmlElement("last-name")]
        public string LastName;

        [XmlElement("url-path")]
        public string UrlPath;

        [XmlElement("type")]
        public SCOtype MeetingItemType = SCOtype.NotSet;

        [XmlElement("source-sco-id")]
        public string SourceScoId;
    }
}
