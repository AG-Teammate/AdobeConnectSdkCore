/*
Copyright 2007-2014 Dmitry Stroganov (dmitrystroganov.dk)
Redistributions of any form must retain the above copyright notice.
 
Use of any commands included in this SDK is at your own risk. 
Dmitry Stroganov cannot be held liable for any damage through the use of these commands.
*/

namespace AdobeConnectSDK.Interfaces
{
    using AdobeConnectSDK.Model;
    using System.Threading.Tasks;

    /// <summary>
    /// Communication provider.
    /// </summary>
    public interface ICommunicationProvider
    {
        Task<ApiStatus> ProcessRequest(string pAction, string qParams);
        Task<byte[]> DownloadContent(string url, string sessionInfo = null);
    }
}
