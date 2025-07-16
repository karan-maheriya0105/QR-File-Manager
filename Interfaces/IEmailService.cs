namespace BrochureAPI.Interfaces
{
    public interface IEmailService
    {
        /// <summary>
        /// Sends an email with the specified parameters
        /// </summary>
        /// <param name="to">Recipient email address</param>
        /// <param name="subject">Email subject</param>
        /// <param name="body">Email body content (can be HTML)</param>
        /// <param name="isHtml">Whether the body contains HTML</param>
        /// <returns>A task representing the asynchronous operation</returns>
        Task SendEmailAsync(string to, string subject, string body, bool isHtml = true);

        /// <summary>
        /// Sends an email with attachment
        /// </summary>
        /// <param name="to">Recipient email address</param>
        /// <param name="subject">Email subject</param>
        /// <param name="body">Email body content</param>
        /// <param name="attachmentPath">Path to the attachment file</param>
        /// <param name="isHtml">Whether the body contains HTML</param>
        /// <returns>A task representing the asynchronous operation</returns>
        Task SendEmailWithAttachmentAsync(string to, string subject, string body, string attachmentPath, bool isHtml = true);

        /// <summary>
        /// Sends an email with attachment using a specific attachment name
        /// </summary>
        /// <param name="to">Recipient email address</param>
        /// <param name="subject">Email subject</param>
        /// <param name="body">Email body content</param>
        /// <param name="attachmentPath">Path to the attachment file</param>
        /// <param name="attachmentName">Name to use for the attachment</param>
        /// <param name="isHtml">Whether the body contains HTML</param>
        /// <returns>A task representing the asynchronous operation</returns>
        Task SendEmailWithAttachmentAsync(string to, string subject, string body, string attachmentPath, string attachmentName, bool isHtml = true);
    }
}