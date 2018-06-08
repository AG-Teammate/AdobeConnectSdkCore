using AdobeConnectSDK.Common;
using AdobeConnectSDK.Model;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

namespace AdobeConnectSDK.Extensions
{
    public static class ContentManagement
    {
        public static async Task<ApiStatusWithSco> Download(this AdobeConnectXmlAPI adobeConnectXmlApi, string scoId)
        {
            var scoInfo = await GetScoById(adobeConnectXmlApi, scoId);
            var url = scoInfo.Sco.UrlPath;
            var filename = scoInfo.Sco.Name;
            var filenameExtensionIndex = filename.LastIndexOf(".", StringComparison.Ordinal);
            if (filenameExtensionIndex > 0)
            {
                var extension = filename.Substring(filenameExtensionIndex + 1);

                var objectId = url.Replace("/", "");
                var fullUrl = string.Format("/{0}/output/{0}.{1}?download={1}", objectId, extension);
                try
                {
                    var data = await adobeConnectXmlApi.CommunicationProvider.DownloadContent(fullUrl, adobeConnectXmlApi.SessionInfo);
                    scoInfo.Sco.Data = data;
                }
                catch (Exception e)
                {
                    scoInfo.Code = StatusCodes.NoData;
                    scoInfo.InnerException = e;
                }
            }
            else
            {
                scoInfo.Code = StatusCodes.Invalid;
            }
            return scoInfo;
        }

        public static async Task<ApiStatusWithSco> CreateFile(this AdobeConnectXmlAPI adobeConnectXmlApi, string folderId, string fileName, byte[] Data)
        {
            var sco = new Sco()
            {
                FolderId = folderId,
                Name = fileName,
                ItemType = SCOtype.Content,
                Data = Data
            };
            var scoInfo = await ScoUpdate(adobeConnectXmlApi, sco);

            if (scoInfo.Code == StatusCodes.OK && !string.IsNullOrEmpty(scoInfo.Sco.ScoId))
            {
                sco.ScoId = scoInfo.Sco.ScoId;
                try
                {
                    await Upload(adobeConnectXmlApi, sco);
                }
                catch (Exception e)
                {
                    //rollback if upload fails
                    await DeleteByScoId(adobeConnectXmlApi, sco.ScoId);
                    scoInfo.Code = StatusCodes.InternalError;
                    scoInfo.InnerException = e;
                }
            }

            return scoInfo;
        }

        public static async Task<ApiStatus> DeleteByScoId(this AdobeConnectXmlAPI adobeConnectXmlApi, string scoId)
        {
            return await ScoGenericOperation(adobeConnectXmlApi, new Sco() { ScoId = scoId }, "sco-delete");
        }

        public static async Task<ApiStatusWithSco> CreateFolder(this AdobeConnectXmlAPI adobeConnectXmlApi, string parentScoId, string name)
        {
            var sco = new Sco()
            {
                Name = name,
                FolderId = parentScoId,
                ItemType = SCOtype.Folder
            };

            return await ScoUpdate(adobeConnectXmlApi, sco);
        }

        public static async Task<ApiStatus> RenameSco(this AdobeConnectXmlAPI adobeConnectXmlApi, string scoId, string newName)
        {
            var sco = new Sco()
            {
                ScoId = scoId,
                Name = newName
            };
            return await ScoUpdate(adobeConnectXmlApi, sco);
        }

        public static async Task<ApiStatusWithSco> ScoUpdate(AdobeConnectXmlAPI adobeConnectXmlApi, Sco sco)
        {
            return await ScoGenericOperation(adobeConnectXmlApi, sco, "sco-update");
        }

        public static async Task<ApiStatusWithSco> GetScoById(AdobeConnectXmlAPI adobeConnectXmlApi, string scoId)
        {
            return await ScoGenericOperation(adobeConnectXmlApi, new Sco() { ScoId = scoId }, "sco-info");
        }

        public static async Task<ApiStatusWithSco> ScoGenericOperation(AdobeConnectXmlAPI adobeConnectXmlApi, Sco sco, string operation)
        {
            var response = ApiStatusWithSco.FromApiStatus(await adobeConnectXmlApi.ProcessApiRequest(operation, Helpers.StructToQueryString(sco, true)));

            if (response.Code != StatusCodes.OK || response.ResultDocument == null)
            {
                return response;
            }

            XElement scoNode = response.ResultDocument.XPathSelectElement("//sco");
            if (scoNode == null)
            {
                return response;
            }

            try
            {
                response.Sco = XmlSerializerHelpersGeneric.FromXML<Sco>(scoNode.CreateReader());
                response.Sco.FullUrl = adobeConnectXmlApi.ResolveFullUrl(response.Sco.UrlPath);
            }
            catch (Exception ex)
            {
                response.Code = StatusCodes.Invalid;
                response.SubCode = StatusSubCodes.Format;
                response.InnerException = ex;

                throw ex.InnerException;
            }

            return response;
        }

        public static async Task Upload(this AdobeConnectXmlAPI adobeConnectXmlApi, Sco content)
        {
            using (var httpClient = new HttpClient())
            {
                if (adobeConnectXmlApi.Settings.HttpTimeoutSeconds.HasValue)
                    httpClient.Timeout = TimeSpan.FromSeconds(adobeConnectXmlApi.Settings.HttpTimeoutSeconds.Value);
                var form = new MultipartFormDataContent();
                form.Add(new ByteArrayContent(content.Data), "file", content.Name);
                var url = string.Format("{0}/api/xml?action=sco-upload&sco-id={1}&session={2}",
                    adobeConnectXmlApi.Settings.ServiceURL, content.ScoId, adobeConnectXmlApi.SessionInfo);
                await httpClient.PostAsync(url, form);
            }
        }

        public static async Task<EnumerableResultStatus<Sco>> GetFolderContents(this AdobeConnectXmlAPI adobeConnectXmlApi, string folderId)
        {
            var s = await adobeConnectXmlApi.ProcessApiRequest("sco-contents", String.Format("sco-id={0}", folderId));
            var result = Helpers.WrapBaseStatusInfo<EnumerableResultStatus<Sco>>(s);

            if (s.Code != StatusCodes.OK || s.ResultDocument == null)
            {
                return result;
            }

            var scoList = s.ResultDocument.XPathSelectElements("//sco");
            if (scoList == null) return result;

            result.Result = scoList.Select(scoNode =>
            {
                var sco = XmlSerializerHelpersGeneric.FromXML<Sco>(scoNode.CreateReader());
                sco.FullUrl = adobeConnectXmlApi.ResolveFullUrl(sco.UrlPath);
                return sco;
            }).ToList();

            return result;
        }
    }
}
