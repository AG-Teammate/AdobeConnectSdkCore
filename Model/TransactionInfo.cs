/*
Copyright 2007-2014 Dmitry Stroganov (dmitrystroganov.dk)
Redistributions of any form must retain the above copyright notice.
 
Use of any commands included in this SDK is at your own risk. 
Dmitry Stroganov cannot be held liable for any damage through the use of these commands.
*/

using System.Xml.Serialization;
using AdobeConnectSDK.Common;

namespace AdobeConnectSDK.Model
{
    /// <summary>
    /// TransactionInfo structure
    /// </summary>

    [XmlRoot("row")]
  public class TransactionInfo : XmlDateTimeBase
  {
    [XmlAttribute("transaction-id")]
    public string TransactionId;

    [XmlAttribute("sco-id")]
    public string ScoId;

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

    [XmlAttribute("principal-id")]
    public string PrincipalId;

    [XmlAttribute("score")]
    public string Score;

    [XmlElement("name")]
    public string Name;

    [XmlElement("url")]
    public string Url;

    [XmlElement("login")]
    public string Login;

    [XmlElement("user-name")]
    public string UserName;

    [XmlElement("status")]
    public string Status;
  }
}
