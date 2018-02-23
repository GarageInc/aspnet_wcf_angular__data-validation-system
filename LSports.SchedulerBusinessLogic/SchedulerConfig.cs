using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LSports.Scheduler
{
	/// <summary>
	/// Static and dinamic config for services. Now all configs moved in web.config
	/// </summary>
    public class SchedulerConfig
    {
        public static TimeSpan PullCommonWorkerTimeoutInMs = new TimeSpan(0, 0, 0, 1);
        public static TimeSpan PushWorkerTimeoutInMs = new TimeSpan(0, 0, 0, 0, 500);
		
        public static TimeSpan DownloadingTimeoutInMs = new TimeSpan(0, 0, 0, 10);
        public static int MaxBytesCount = 4096;

		// Configs in web.config
        public static string ResetDownloaderIntervalAlias = "ResetDownloaderInterval";
        public static string ResetDownloaderTimeStampAlias = "ResetDownloaderTimeStamp";//new TimeSpan(0, -1, 30, 0, 0);

        public static string PortAliasInConfig = "Port";

        public static string BasePathAliasInConfig = "BasePath";

        public static string SlackWebhookAlias = "SlackWebhook";
        public static string DvsHostAlias = "DvsHost";

        public static string SlackDefaultChannelAlias = "SlackDefaultChannel";

        public static string IsPullEnabledAlias = "IsPullEnabled";
        public static string IsPushEnabledAlias = "IsPushEnabled";

        public static string EmailSmtpPortAlias = "EmailSmtpPort";
        public static string EmailSmtpHostAlias = "EmailSmtpHost";
        public static string EmailSenderAddressAlias = "EmailSenderAddress";
        public static string EmailSenderPasswordAlias = "EmailSenderPassword";
        public static string EmailRecipientAlias = "EmailRecipient";

        public static string EventsServiceWebApiUrlAlias = "EventsServiceWebApiUrl";

        public static string EventsServiceWebApiRequestsIntervalAlias = "EventsServiceWebApiRequestsInterval";

        public static string EventsServicePortAlias = "EventsServicePort";

        public static string NumberOfSimultaneouslyProcessedMessagesAlias = "NumberOfSimultaneouslyProcessedMessages";
    }
}