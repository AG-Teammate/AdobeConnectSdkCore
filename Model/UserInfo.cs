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
    /// UserInfo structure
    /// </summary>

    [XmlRoot("user")]
  public class UserInfo
  {
    [XmlAttribute("user-id")]
    public string UserId;

    [XmlElement("name")]
    public string Name;

    [XmlElement("login")]
    public string Login;
  }
}
