/// <summary>
/// Author: Cornelius J. van Dyk blog.cjvandyk.com @cjvandyk
/// This code is provided under GNU GPL 3.0 and is a copyrighted work of the
/// author and contributors.  Please see:
/// https://github.com/cjvandyk/Extensions/blob/main/LICENSE
/// </summary>

using Microsoft.Graph.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static Extensions.Core;

namespace Extensions
{
    /// <summary>
    /// A class container for custom Azure email methods.
    /// </summary>
    [Serializable]
    public static partial class CMail
    {
        public static bool Send(
            string from,
            string subject,
            string body,
            string to,
            string cc = null,
            string bcc = null)
        {
            Message msg = new Message();
            Dictionary<string, string> dic = new Dictionary<string, string>();
            List<FileAttachment> attachments = new List<FileAttachment>();
            foreach (var blob in Azure.Blob.GetBlobs())
            {
                var client = Azure.Blob.GetBlobClient(blob.Name);
                using (var memoryStream = new MemoryStream())
                {
                    client.DownloadTo(memoryStream);
                    if (memoryStream.Length > 0)
                    {
                        memoryStream.Position = 0;
                        using (var streamReader = 
                            new StreamReader(memoryStream))
                        {
                            if (blob.Name.ToLower() == "template.html")
                            {
                                dic.Add(blob.Name.ToLower(), 
                                        streamReader.ReadToEnd()
                                        .Replace("{Body}", body));
                            }
                            else
                            {
                                dic.Add(blob.Name, 
                                        streamReader.ReadToEnd());
                                string contentId = "";
                                if (blob.Name.ToLower().Contains("header"))
                                {
                                    contentId = "header";
                                }
                                else if (blob.Name.ToLower().Contains("footer"))
                                {
                                    contentId = "footer";
                                }
                                memoryStream.Position = 0;
                                attachments.Add(GetGraphFileAttachment(
                                    memoryStream,
                                    blob.Name,
                                    contentId));
                            }
                        }
                    }
                }
            }
            ItemBody itemBody = new ItemBody
            {
                ContentType = BodyType.Html,
                Content = dic["template.html"]
            };
            if (attachments.Count > 0)
            {
                List<Attachment> allAttachments = new List<Attachment>();
                foreach (var attachment in attachments)
                {
                    allAttachments.Add(attachment);
                }
                msg.Subject = subject;
                msg.ToRecipients = GetRecipients(to);
                if (cc != null)
                {
                    msg.CcRecipients = GetRecipients(cc);
                }
                if (bcc != null)
                {
                    msg.BccRecipients = GetRecipients(bcc);
                }
                msg.Body = itemBody;
                if (attachments.Count > 0)
                {
                    msg.HasAttachments = true;
                    msg.Attachments = allAttachments;
                }
            }
            var sendMailPostRequestBody = new Microsoft.Graph.Users.Item.SendMail.SendMailPostRequestBody
            {
                Message = msg
            };
            GetAuth(Identity.ScopeType.Graph, true);
            ActiveAuth.GraphClient.Users[from]
                .SendMail
                .PostAsync(sendMailPostRequestBody)
                .GetAwaiter().GetResult();
            return true;
        }

        public static List<Recipient> GetRecipients(string to)
        {
            List<Recipient> recipients = new List<Recipient>();
            List<string> emails = to.TrimEnd(',').Replace(" ", "").Split(',').ToList();
            foreach (string email in emails)
            {
                Recipient newRecipient = new Recipient()
                {
                    EmailAddress = new EmailAddress { Address = email }
                };
                recipients.Add(newRecipient);
            }
            return recipients;
        }

        internal static FileAttachment GetGraphFileAttachment(
            MemoryStream memoryStream,
            string fileName,
            string contentId)
        {
            FileAttachment fileAttachment = new FileAttachment();
            if (memoryStream.Length > 0)
            {
                memoryStream.Position = 0;
                byte[] fileContent = memoryStream.ToArray();
                memoryStream.Close();
                fileAttachment.OdataType = "#microsoft.graph.fileAttachment";
                fileAttachment.ContentBytes = fileContent;
                if (fileName.ToLower().EndsWith(".jpg") ||
                    fileName.ToLower().EndsWith(".jpeg"))
                {
                    fileAttachment.ContentType = "image/jpeg";
                }
                else if (fileName.ToLower().EndsWith(".gif"))
                {
                    fileAttachment.ContentType = "image/gif";
                }
                else if (fileName.ToLower().EndsWith(".bmp"))
                {
                    fileAttachment.ContentType = "image/bmp";
                }
                fileAttachment.ContentId = contentId;
                fileAttachment.Name = fileName;
            }
            return fileAttachment;
        }
    }
}
