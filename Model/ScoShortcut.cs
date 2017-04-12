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
    /// ScoShortcut structure
    /// </summary>

    [XmlRoot("sco")]
  public class ScoShortcut
  {
    [XmlAttribute("tree-id")]
    public int TreeId;

    [XmlAttribute("sco-id")]
    public string ScoId;

    [XmlAttribute("type")]
    public string Type;

    [XmlElement("domain-name")]
    public string DomainName;

  }
}
