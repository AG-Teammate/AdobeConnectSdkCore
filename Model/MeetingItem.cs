/*
Copyright 2007-2014 Dmitry Stroganov (dmitrystroganov.dk)
Redistributions of any form must retain the above copyright notice.
 
Use of any commands included in this SDK is at your own risk. 
Dmitry Stroganov cannot be held liable for any damage through the use of these commands.
*/

using System;
using System.Xml.Serialization;
using AdobeConnectSDK.Common;

namespace AdobeConnectSDK.Model {
    /// <summary>
    /// Meeting information.
    /// </summary>
  
    [XmlRoot("meeting")]
    public class MeetingItem : XmlDateTimeBase {
        [XmlAttribute("sco-id")]
        public string ScoId;

        [XmlAttribute("folder-id")]
        public string FolderId;

        [XmlAttribute("active-participants")]
        public int ActiveParticipants;

        [XmlIgnore]
        public PermissionId PermissionId;

        [XmlAttribute("permission-id")]
        internal string PermissionIdRaw {
            get { return Helpers.EnumToString(this.PermissionId); }
            set {
                this.PermissionId = Helpers.ReflectEnum<PermissionId>(value);
            }
        }

        [XmlIgnore]
        public SCOtype ItemType;

        [XmlAttribute("type")]
        internal string ItemTypeRaw {
            get {
                return Helpers.EnumToString(this.ItemType);
            }
            set {
                this.ItemType = Helpers.ReflectEnum<SCOtype>(value);
            }
        }

        [XmlAttribute("icon")]
        public string Icon;

        [XmlElement("name")]
        public string MeetingName;

        [XmlElement("description")]
        public string MeetingDescription;

        [XmlElement("lang")]
        public string Language;

        [XmlElement("sco-tag")]
        public string ScoTag;

        [XmlElement("domain-name")]
        public string DomainName;

        [XmlElement("url-path")]
        public string UrlPath;

 //       [NonSerialized]
        [XmlIgnore]
        public string FullUrl;

        [XmlElement("is-folder")]
        public bool IsFolder;

        [XmlElement("expired")]
        public bool Expired;

        [XmlElement("duration")]
        public TimeSpan Duration;

        [XmlElement("byte-count")]
        public long ByteCount;
    }

    public enum SCOtype {
        NotSet,
        Content,
        Course,
        Curriculum,
        //event,
        Folder,
        Link,
        Meeting,
        Session,
        Tree
    }
}
