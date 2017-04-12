/*
Copyright 2007-2014 Dmitry Stroganov (dmitrystroganov.dk)
Redistributions of any form must retain the above copyright notice.
 
Use of any commands included in this SDK is at your own risk. 
Dmitry Stroganov cannot be held liable for any damage through the use of these commands.
*/

using System.Collections.Generic;
using System.Globalization;
using System.Xml.Serialization;

namespace AdobeConnectSDK.Model
{
    /// <summary>
    /// Quota information.
    /// </summary>

    [XmlRoot("report-quotas")]
  public class QuotaInfo
  {
    [XmlElement("quota")]
    public List<Quota> QuotaList { get; set; }
  }


  public class Quota : XmlDateTimeBase
  {
    [XmlAttribute("acl-id")]
    public long AclId { get; set; }

    [XmlAttribute("quota-id")]
    public string QuotaId { get; set; }

    [XmlAttribute("used")]
    public long Used { get; set; }

    [XmlIgnore]
    public long Limit { get; set; }

    [XmlAttribute("limit")]
    internal string LimitRaw
    {
      get { return this.Limit.ToString(CultureInfo.InvariantCulture); }
      set { this.Limit = (value == "unlimited") ? 0 : long.Parse(value, CultureInfo.InvariantCulture); }
    }

    [XmlAttribute("soft-limit")]
    public long SoftLimit { get; set; }
  }
}
