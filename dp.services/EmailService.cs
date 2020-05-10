
using dp.business.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace dp.services
{
    public class EmailService
    {
        private string _baseWebsiteUrl;
        private string _appName;
        private string _senderEmail;
        private string _adminEmail;
        private string _template;
        private EmailConfig _emailConfig;
        private SendGridClient _sgclient;

        public EmailService(EmailConfig emailConfig)
        {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
            _emailConfig = emailConfig;
            _adminEmail = emailConfig.EmailAddresses.AdminEmail;
            _sgclient = new SendGridClient(emailConfig.SendGridKey);
            // If you don't want to use an email template
            _template = @"
                <body bgcolor=""#FFFFFF"" style=""width:100%"">
                </body>";

        }

        // TODO: Convert these into template email
        public async Task InviteNewMemmberToTeamEmail(string toEmail, string emailKey, string teamName, string ownTemplate)
        {
            //create email:
            //Click this link to reset your password
            // return await emailController.SendEmail(request);

            string emailLink = _baseWebsiteUrl + "/#/invite-new-team?ek=" + emailKey;

            string contentText = "You can verify NID in real time using our automated API or using our manual portal.";
            string welcomeText = $"You have been invited to join the team: {teamName}.";
            string linkText = $"Please click here to accept the invite";
            string nameText = "";


            ownTemplate = ownTemplate.Replace("{{click-link}}", emailLink);
            ownTemplate = ownTemplate.Replace("{{click-text}}", linkText);
            ownTemplate = ownTemplate.Replace("{{hello-text}}", nameText);
            ownTemplate = ownTemplate.Replace("{{welcome-text}}", welcomeText);
            ownTemplate = ownTemplate.Replace("{{content-text}}", contentText);
            EmailForSendGrid esg = new EmailForSendGrid()
            {
                HtmlText = _template,
                PlainText = @"Welcome",
                Subject = "Invite to " + _appName,
                EmailTo = toEmail,
                DisplayNameFrom = _appName + " Support"
            };
            await SendEmail(esg);
        }

        // All the following task is using email template
        public async Task SendTemplateSendGridEmail(string templateId, TeamInfo teamInfo, UserProfile userProfile, string emailKey = null)
        {
          //  if (teamInfo != null) SetEmailSetting(teamInfo);
            string emailLink = "";
            if (emailKey != null) emailLink = _baseWebsiteUrl + "/#/set-password?ek=" + emailKey;
            string loginUrl = _baseWebsiteUrl + "/#/login";
            var from = new EmailAddress(_senderEmail, _appName);
            var to = new EmailAddress(userProfile.Email);
            SendGridTemplateData templateData = new SendGridTemplateData()
            {
                AppName = _appName,
                TeamInfo = teamInfo,
                UserProfile = userProfile,
                LoginUrl = loginUrl,
                Link = emailLink
            };
            var msg = MailHelper.CreateSingleTemplateEmail(from, to, templateId, templateData);
            var response = await _sgclient.SendEmailAsync(msg);
        }
        public async Task SendEmail(EmailForSendGrid request)
        {
            string replyto = _emailConfig.EmailAddresses.SupportEmail;

            string emailFrom = _emailConfig.EmailAddresses.SupportEmail;
            request.EmailFrom = request.EmailFrom ?? emailFrom;
            var from = new EmailAddress(request.EmailFrom, request.DisplayNameFrom);
            var subject = request.Subject;
            var to = new EmailAddress(request.EmailTo);     
            var plainTextContent = request.PlainText;
            var htmlContent = request.HtmlText;
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            msg.SetReplyTo(new EmailAddress(replyto, request.DisplayNameFrom));
            if (request.EmailBccs != null)
            {
                foreach (string email in request.EmailBccs)
                {
                    msg.AddBcc(email);
                }
            }
            var response = await _sgclient.SendEmailAsync(msg);
        }
    }
    public class EmailForSendGrid
    {
        public String EmailFrom { get; set; }
        public String DisplayNameFrom { get; set; }
        public String Subject { get; set; }
        public String EmailTo { get; set; }
        public String DisplayNameTo { get; set; }
        public String PlainText { get; set; }
        public String HtmlText { get; set; }
        public List<string> EmailBccs { get; set; }
    }

    public class SendGridTemplateData
    {
        public string Subject { get; set; }
        public TeamInfo TeamInfo { get; set; }
        public UserProfile UserProfile { get; set; }
        public string AppName { get; set; }
        public string LoginUrl { get; set; }
        public string Link { get; set; }
        public string Note { get; set; }
    }
}
