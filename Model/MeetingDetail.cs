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
    /// Meeting structure
    /// </summary>

    [XmlRoot("sco")]
  public class MeetingDetail : XmlDateTimeBase
  {
    [XmlAttribute("sco-id")]
    public string ScoId;

    [XmlAttribute("account-id")]
    public string AccountId;

    [XmlAttribute("folder-id")]
    public string FolderId;

    [XmlAttribute("lang")]
    public string Language;

    [XmlElement("name")]
    public string Name;

    [XmlElement("description")]
    public string Description;

    [XmlElement("url-path")]
    public string UrlPath;

//    [NonSerialized]
    public string FullUrl;

    [XmlElement("passing-score")]
    public int PassingScore;

    /// <summary>
    /// The length of time needed to View or play the SCO, in milliseconds.
    /// </summary>
    [XmlElement("duration")]
    public int Duration;

    [XmlElement("section-count")]
    public int SectionCount;
  }
}
