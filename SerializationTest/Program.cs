using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using LSports.Framework.Models.CustomClasses;
using Newtonsoft.Json;
using System.Data;
using System.Globalization;
using System.Net;
using System.Xml.Linq;
using LSports.Framework.Models.Enums;
using System.Security.Cryptography;
using System.Threading;
using System.Xml.XPath;
using LSports.Scheduler;
using LSports.Scheduler.Jobs;
using LSports.Scheduler.Jobs.@base;
using LSports.Scheduler.Services;
using Slack.Webhooks;

namespace SerializationTest
{
    class Program
    {

        static void Main(string[] args)
        {;
			/*
            var c = 1;
            
            var a = XDocument.Load("input_A.xml");
            var b = XDocument.Load("input_B.xml");

            var merger = new XmlMerger();

            merger.MergeWithOldChanges(a.Elements(), b.Elements(), a.Root, b.Root, "");

            a.Save("output_merged_A_with_B.xml");
            /*
            */
            
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-us");

            XmlValidator validator = new XmlValidator();

            XmlCommonWorker worker = new XmlCommonWorker();

            var _parser = new XmlParser();

            var messageEvents = new List<Event>();
            // 6361461573057051433e9b0467-3e82-4078-a079-9c4101ec9e83
            using (XmlTextReader myTextReader = new XmlTextReader("test.xml"))//"TestCases\\ValidationSetting #" + id+"\\Positive#1.xml"))
            {
                myTextReader.WhitespaceHandling = WhitespaceHandling.None;

                // XmlDocument rewardXmlDoc = new XmlDocument();

                var headerXml = "";

                while (myTextReader.EOF == false)
                {
                    if (myTextReader.NodeType == XmlNodeType.Element)
                    {
                        if (myTextReader.LocalName == "Event")
                        {
                            var xmlText = myTextReader.ReadOuterXml();
                            
                            var newEvent = _parser.LoadEvent(true, 1, headerXml, xmlText,
                                new List<string>(), new List<string>(), new List<string>(), new List<string>(),
                                new List<string>(),
                                new List<string>(), new List<string>(), new List<string>());
                              messageEvents.Add(newEvent);
                        }
                        else if (myTextReader.LocalName == "Header")
                        {
                            headerXml = myTextReader.ReadOuterXml();

                            // myTextReader.Skip();
                        }
                        else
                        {
                            myTextReader.Read();
                        }
                    }
                    else
                    {
                        myTextReader.Read();
                    }
                }
            }
            
            var id = 198;

            var validationSettings = worker.GetValidationSettingsForProducts(new List<int> { 1, 2, 3, 4, 5 }).ToList();
            validationSettings = validationSettings.Where(x => x.Id == id).ToList();

            var validationRules = worker.GetValidationRules();

            worker.SetRulesForSettings(validationSettings, validationRules);

            messageEvents = messageEvents.Take(1000).ToList();
            var results =  validator.ValidateEventsCommon(messageEvents, validationSettings.ToList());

            var first = results[0];
        }
        
    }
    
}
