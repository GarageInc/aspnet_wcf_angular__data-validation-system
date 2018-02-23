using log4net;
using LSports.DVS.Framework.DataAccess.Repositories;
using LSports.DVS.Framework.DataAccess.Repositories.Interfaces;
using LSports.Framework.Models.CustomClasses;
using LSports.Framework.Models.Enums;
using LSports.Scheduler.Services;
using Quartz;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Web;
using System.Xml;
using LSports.Scheduler.Jobs.@base;

namespace LSports.Scheduler.Jobs
{
	// This sender work each N hours(the hours are taken from the config)
	[DisallowConcurrentExecution]
    public class DailyReporter : XmlWorker
    {
        public IValidationSettingRepository validationSettingRepository = new ValidationSettingRepository();
        public IValidationResultRepository validationResultRepository = new ValidationResultRepository();
        public IProductRepository productRepository = new ProductRepository();

        public static string DATE_SHORT_FORMAT = "yyyy-MM-dd";
        public static string DATE_FORMAT = "yyyy-MM-ddTHH:mm:ss";

        override public void Run()
        {
			// For last day
            var borderDate = DateTime.UtcNow.AddHours(-24);

			// All products, settings and val.results
            var products = productRepository.GetList();
            var settings = validationSettingRepository.GetList();
            var validationResults = validationResultRepository.GetValidationResultsByBorderDate(borderDate);

			// Email's sender
            var senderMail = ConfigurationManager.AppSettings[SchedulerConfig.EmailSenderAddressAlias];
            var senderPass = ConfigurationManager.AppSettings[SchedulerConfig.EmailSenderPasswordAlias];

			// Emails host-server info
            var smtpHost = ConfigurationManager.AppSettings[SchedulerConfig.EmailSmtpHostAlias];
            var smtpPort = int.Parse(ConfigurationManager.AppSettings[SchedulerConfig.EmailSmtpPortAlias]);

			// Receiver
            var recipientMail = ConfigurationManager.AppSettings[SchedulerConfig.EmailRecipientAlias];

			// Creating empty client
            SmtpClient smtpClient = new SmtpClient(smtpHost);

            smtpClient.Credentials = new System.Net.NetworkCredential(senderMail, senderPass);
            //smtpClient.UseDefaultCredentials = true;
            //smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.EnableSsl = true;
            smtpClient.Port = smtpPort;

			// Bulding body of email with custom info
            var body = new StringBuilder();

            body.Append($"<b><h2>Daily report for : {borderDate.ToString(DATE_SHORT_FORMAT)}</h3></b>");

			// Min/max dates from val.results
            var firstDate = validationResults.Min(x => x.CreatedOn);
            var lastDate = validationResults.Max(x => x.CreatedOn);

            body.Append($"First validation result created at: {firstDate.ToString(DATE_FORMAT)};");
            body.Append($"</br>");
            body.Append($"Last validation result created at: {lastDate.ToString(DATE_FORMAT)};");

			// Grep all info for products
            foreach (var product in products)
            {
                body.Append($"<b><h3>Product: {product.Name}</h3></b>");

                var groupByProduct = validationResults.Where(x => x.ProductId == product.Id);

                if (groupByProduct.Any())
                {
                    var groupBySettings = groupByProduct.GroupBy(x => x.ValidationSettingId);

                    foreach (var groupBySetting in groupBySettings)
                    {
                        var setting = settings.First(x => x.Id == groupBySetting.Key);
                        
                        body.Append($"<b><h4>#{setting.Id} '{setting.Name}'</h3></b>");
                        body.Append(
                            $"<div>Total count: {groupBySetting.Count()}; active: {groupBySetting.Count(x => x.IsActive)}</div>");
                    }
                }
                else
                {
                    body.Append($"<div>No new validation results</div>");
                }
            }

			// Then we send email
            MailMessage mail = new MailMessage
            {
                From = new MailAddress(senderMail, "DVS - daily report for " + borderDate.ToString(DATE_SHORT_FORMAT)),
                IsBodyHtml = true,
                Body = body.ToString()
            };

            mail.To.Add(new MailAddress(recipientMail));

            smtpClient.Send(mail);
        }
    }
}