/*
Copyright 2007-2014 Dmitry Stroganov (dmitrystroganov.dk)
Redistributions of any form must retain the above copyright notice.
 
Use of any commands included in this SDK is at your own risk. 
Dmitry Stroganov cannot be held liable for any damage through the use of these commands.
*/

using System;
using System.Globalization;
using System.Xml.Serialization;
using AdobeConnectSDK.Common;

namespace AdobeConnectSDK.Model
{
  /// <summary>
  /// Shared serialization properties.
  /// </summary>

  public class XmlDateTimeBase
  {
    [XmlIgnore]
    public DateTime DateBegin { get; set; }

    [XmlElement(ElementName = "date-begin")]
    public string DateBeginRaw
    {
      get { return this.DateBegin.ToString(Constants.DateFormatString, CultureInfo.InvariantCulture); }
      set { this.DateBegin = DateTime.ParseExact(value, Constants.DateFormatString, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal); }
    }

    [XmlIgnore]
    public DateTime DateEnd { get; set; }

    [XmlElement(ElementName = "date-end")]
    public string DateEndRaw
    {
      get { return this.DateEnd.ToString(Constants.DateFormatString, CultureInfo.InvariantCulture); }
      set { this.DateEnd = DateTime.ParseExact(value, Constants.DateFormatString, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal); }
    }

    [XmlIgnore]
    public DateTime DateModified { get; set; }

    [XmlElement(ElementName = "date-modified")]
    public string DateModifiedRaw
    {
      get { return this.DateModified.ToString(Constants.DateFormatString, CultureInfo.InvariantCulture); }
      set { this.DateModified = DateTime.ParseExact(value, Constants.DateFormatString, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal); }
    }

    [XmlIgnore]
    public DateTime DateCreated { get; set; }

    [XmlElement(ElementName = "date-created")]
    public string DateCreatedRaw
    {
      get { return this.DateCreated.ToString(Constants.DateFormatString, CultureInfo.InvariantCulture); }
      set { this.DateCreated = DateTime.ParseExact(value, Constants.DateFormatString, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal); }
    }

    [XmlIgnore]
    public DateTime DateClosed { get; set; }

    [XmlElement(ElementName = "date-closed")]
    public string DateClosedRaw
    {
      get { return this.DateClosed.ToString(Constants.DateFormatString, CultureInfo.InvariantCulture); }
      set { this.DateClosed = DateTime.ParseExact(value, Constants.DateFormatString, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal); }
    }
  }
}
