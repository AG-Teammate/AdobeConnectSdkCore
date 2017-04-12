/*
Copyright 2007-2014 Dmitry Stroganov (dmitrystroganov.dk)
Redistributions of any form must retain the above copyright notice.
 
Use of any commands included in this SDK is at your own risk. 
Dmitry Stroganov cannot be held liable for any damage through the use of these commands.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.XPath;
using AdobeConnectSDK.Common;
using AdobeConnectSDK.Interfaces;
using AdobeConnectSDK.Model;
using System.Threading.Tasks;

namespace AdobeConnectSDK
{
    /// <summary>
    /// AdobeConnectXmlAPI - .Net wrapper for Adobe Connect Professional web services.
    /// Version supported: from 6 and up.
    /// </summary>
    public sealed class AdobeConnectXmlAPI
    {
        string sessionInfo = string.Empty;

        private readonly ICommunicationProvider communicationProvider;

        private readonly ISdkSettings settings;

        public ISdkSettings Settings
        {
            get { return this.settings; }
        }

        public ICommunicationProvider CommunicationProvider
        {
            get { return this.communicationProvider; }
        }

        public string SessionInfo
        {
            get { return this.sessionInfo; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdobeConnectXmlAPI"/> class, using default <see cref="HttpCommunicationProvider"/> and <see cref="SdkSettings"/>.
        /// </summary>
        /// <remarks>
        /// <para>Default constructor expects the following configuration defined:</para>
        /// <para>&lt;add key="ServiceURL" value="https://acrobat.com/api/xml" /&gt;</para>
        /// <para>&lt;add key="NetUser" value="[your AC user]" /&gt;</para>
        /// <para>&lt;add key="NetPassword" value="[your AC password]" /&gt;</para>
        /// <para>Optional proxy settings:</para>
        /// <para>&lt;settings&gt;</para>
        /// <para>&lt;ipv6 enabled="true" /&gt;</para>
        /// <para>&lt;/settings&gt;</para>
        /// <para>&lt;defaultProxy enabled="true" useDefaultCredentials="true"&gt;</para>
        /// <para>    &lt;proxy bypassonlocal="True" proxyaddress="http://..." /&gt;</para>
        /// <para>&lt;/defaultProxy&gt;</para>
        /// </remarks>
        public AdobeConnectXmlAPI()
            : this(new HttpCommunicationProvider(), new SdkSettings()
            {
                //ServiceURL = ConfigurationManager.AppSettings["ServiceURL"],
                //NetUser = ConfigurationManager.AppSettings["NetUser"],
                //NetPassword = ConfigurationManager.AppSettings["NetPassword"],
                //NetDomain = ConfigurationManager.AppSettings["NetDomain"],
                //ProxyUrl = ConfigurationManager.AppSettings["ProxyUrl"],
                //UseSessionParam = ConfigurationManager.AppSettings["UseSessionParam"] == null || bool.Parse(ConfigurationManager.AppSettings["UseSessionParam"])
            })
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdobeConnectXmlAPI" /> class.
        /// </summary>
        /// <param name="communicationProvider">The communicaton provider.</param>
        /// <param name="settings"><see cref="ISdkSettings"/></param>
        /// <exception cref="System.ArgumentNullException">
        /// Argument 'communicationProvider' can not be null.
        /// or
        /// Argument 'settings' can not be null.
        /// or
        /// Configuration parameter 'serviceURL' can not be null.
        /// </exception>
        public AdobeConnectXmlAPI(ICommunicationProvider communicationProvider, ISdkSettings settings)
        {
            if (communicationProvider == null)
                throw new ArgumentNullException("Argument 'communicationProvider' can not be null.");

            if (settings == null)
                throw new ArgumentNullException("Argument 'settings' can not be null.");

            if (string.IsNullOrEmpty(settings.ServiceURL))
                throw new ArgumentNullException("Configuration parameter 'serviceURL' can not be null.");

            this.communicationProvider = communicationProvider;
            this.settings = settings;

            this.settings.ServiceURL = this.settings.ServiceURL.TrimEnd(new char[] { '/', '?' });

            AutoMapper.Mapper.Initialize(cfg => cfg.CreateMissingTypeMaps = true);
        }

        /// <summary>
        /// Performs log-in procedure.
        /// </summary>
        /// <param name="sInfo">after successful Login, <see cref="LoginStatus"/> contains Session ID to be used for single-sign-on.</param>
        /// <returns>An <see cref="LoginStatus"/></returns>
        /// <example>
        /// url: action=Login&Login=bobs@acme.com&password=football&Session=
        /// cookie: BREEZESESSION
        /// </example>
        public async Task<LoginStatus> Login()
        {
            return await this.Login(this.settings.NetUser, this.settings.NetPassword);
        }

        /// <summary>
        /// Performs log-in procedure.
        /// </summary>
        /// <param name="userName">valid Adobe Connect account Name.</param>
        /// <param name="userPassword">valid Adobe Connect account password.</param>
        /// <param name="sInfo">after successful Login, <see cref="LoginStatus"/> contains Session ID to be used for single-sign-on.</param>
        /// <returns>An <see cref="LoginStatus"/></returns>
        /// <example>
        /// url: action=Login&Login=bobs@acme.com&password=football&Session=
        /// cookie: BREEZESESSION
        /// </example>
        public async Task<LoginStatus> Login(string userName, string userPassword)
        {
            ApiStatus s = await this.ProcessApiRequest("login", string.Format("login={0}&password={1}", userName, userPassword));

            var loginStatus = Helpers.WrapBaseStatusInfo<LoginStatus>(s);

            if (s.Code != StatusCodes.OK || string.IsNullOrEmpty(s.SessionInfo))
            {
                return loginStatus;
            }

            this.sessionInfo = s.SessionInfo;

            loginStatus.Result = true;
            return loginStatus;
        }

        /// <summary>
        /// Performs log-out procedure.
        /// </summary>
        /// <returns>A <see cref="bool"/> value, indicating whenever the action was successful.</returns>
        public async Task<bool> Logout()
        {
            ApiStatus s = await this.ProcessApiRequest("logout", null);

            if (s.Code == StatusCodes.OK)
            {
                this.sessionInfo = null;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns information about currently logged in user.
        /// </summary>
        /// <returns>
        ///   <see cref="UserInfo" />
        /// </returns>
        public async Task<UserInfoStatus> GetUserInfo()
        {
            ApiStatus s = await this.ProcessApiRequest("common-info", null);

            var userInfoStatus = Helpers.WrapBaseStatusInfo<UserInfoStatus>(s);

            if (s.Code != StatusCodes.OK || s.ResultDocument == null)
            {
                return null;
            }

            try
            {
                var userInfo = XmlSerializerHelpersGeneric.FromXML<UserInfo>(s.ResultDocument.Descendants("user").FirstOrDefault().CreateReader());
                userInfoStatus.Result = userInfo;
                return userInfoStatus;
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }

        /// <summary>
        /// Creates metadata for a SCO, or updates existing metadata describing a SCO.
        /// Call sco-update to create metadata only for SCOs that represent Content, including
        /// meetings. You also need to upload Content files with either sco-upload or Connect Enterprise Manager.
        /// You must provide a folder-id or a sco-id, but not both. If you pass a folder-id, scoupdate
        /// creates a new SCO and returns a sco-id. If the SCO already exists and you pass a
        /// sco-id, sco-update updates the metadata describing the SCO.
        /// After you create a new SCO with sco-update, call permissions-update to specify which
        /// users and groups can access it.
        /// </summary>
        /// <returns>
        ///   <see cref="ApiStatus" />
        /// </returns>
        internal async Task<ApiStatusWithMeetingDetail> ScoUpdate(MeetingUpdateItem meetingUpdateItem)
        {
            if (meetingUpdateItem == null)
                return null;

            string cmdParams = Helpers.StructToQueryString(meetingUpdateItem, true);

            ApiStatusWithMeetingDetail s = ApiStatusWithMeetingDetail.FromApiStatus(await this.ProcessApiRequest("sco-update", cmdParams));

            if (s.Code != StatusCodes.OK || s.ResultDocument == null)
            {
                return s;
            }

            //notice: no '/sco' will be returned during update
            XElement meetingDetailNode = s.ResultDocument.XPathSelectElement("//sco");
            if (meetingDetailNode == null)
                return s;

            try
            {
                s.MeetingDetail = XmlSerializerHelpersGeneric.FromXML<MeetingDetail>(meetingDetailNode.CreateReader());
                s.MeetingDetail.FullUrl = this.ResolveFullUrl(s.MeetingDetail.UrlPath);
            }
            catch (Exception ex)
            {
                s.Code = StatusCodes.Invalid;
                s.SubCode = StatusSubCodes.Format;
                s.InnerException = ex;

                //rollback: delete the meeting
                //AG: scary stuff
                if (!string.IsNullOrEmpty(s.MeetingDetail.ScoId))
                    await this.ScoDelete(new[]
                    {
                        s.MeetingDetail.ScoId
                    });

                throw ex.InnerException;
            }

            return s;
        }

        /// <summary>
        /// Deletes one or more objects (SCOs).
        /// If the sco-id you specify is for a Folder, all the contents of the specified Folder are deleted. To
        /// delete multiple SCOs, specify multiple sco-id parameters.
        /// You can use a call such as sco-contents to check the ref-count of the SCO, which is the
        /// number of other SCOs that reference this SCO. If the SCO has no references, you can safely
        /// Remove it, and the server reclaims the space.
        /// If the SCO has references, removing it can cause the SCOs that reference it to stop working,
        /// or the server not to reclaim the space, or both. For example, if a Course references a quiz
        /// presentation, removing the presentation might make the Course stop working.
        /// As another example, if a Meeting has used a Content SCO (such as a presentation or video),
        /// there is a reference from the Meeting to the SCO. Deleting the Content SCO does not free
        /// disk space, because the Meeting still references it.
        /// To delete a SCO, you need at least Manage permission (see permission-id for details). Users
        /// who belong to the built-in authors group have Manage permission on their own Content
        /// Folder, so they can delete Content within it.
        /// </summary>
        /// <param name="scoId">The sco identifier.</param>
        /// <returns>
        ///   <see cref="ApiStatus" />
        /// </returns>
        internal async Task<ApiStatus> ScoDelete(string[] scoId)
        {
            for (int i = 0; i < scoId.Length; i++)
            {
                scoId[i] = "sco-id=" + scoId[i];
            }

            ApiStatus s = await this.ProcessApiRequest("sco-delete", string.Join("&", scoId));

            return s;
        }

        /// <summary>
        /// Prepares the API request, by combining action, parameters, and session information.
        /// </summary>
        /// <param name="apiAction">The API action.</param>
        /// <param name="apiParams">The API parameters.</param>
        /// <returns>
        ///   <see cref="ApiStatus" />
        /// </returns>
        public async Task<ApiStatus> ProcessApiRequest(string apiAction, string apiParams)
        {
            if (!string.IsNullOrEmpty(this.sessionInfo) && this.settings.UseSessionParam)
            {
                if (String.IsNullOrEmpty(apiParams))
                {
                    apiParams = "session=" + this.sessionInfo;
                }
                else
                {
                    apiParams = String.Concat("session=", this.sessionInfo, @"&", apiParams);
                }
            }

            return await this.communicationProvider.ProcessRequest(apiAction, apiParams);
        }

        #region internal routines

        internal IEnumerable<MeetingItem> PreProcessMeetingItems(IEnumerable<XElement> list, XmlRootAttribute xmlRootAttribute)
        {
            IEnumerable<MeetingItem> meetingItems = list.Select(meetingInfo => XmlSerializerHelpersGeneric.FromXML<MeetingItem>(meetingInfo.CreateReader(), xmlRootAttribute));

            foreach (var meetingItem in meetingItems)
            {
                //NOTE: if Folder =>  date-begin is null
                meetingItem.Duration = meetingItem.DateEnd.Subtract(meetingItem.DateBegin);

                if (!string.IsNullOrEmpty(meetingItem.UrlPath))
                {
                    Uri uri = new Uri(this.settings.ServiceURL);
                    meetingItem.FullUrl = uri.GetComponents(UriComponents.SchemeAndServer, UriFormat.SafeUnescaped) + meetingItem.UrlPath;
                }

                yield return meetingItem;
            }
        }

        internal string ResolveFullUrl(string urlPath)
        {
            if (string.IsNullOrEmpty(urlPath))
            {
                return string.Empty;
            }

            var u = new Uri(this.settings.ServiceURL);
            return u.GetComponents(UriComponents.SchemeAndServer, UriFormat.SafeUnescaped) + urlPath;
        }

        #endregion

    }
}