using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using LSports.Framework.Models.CustomClasses;
using LSports.Scheduler.Jobs.@base;
using LSports.Scheduler.Services;
using NUnit.Framework;

namespace LSports.Tests.Validation
{
    [TestFixture]
    public class XmlValidatorTests
    {
        private IList<ValidationSetting> _validationSettings;
        private IList<ValidationRule> _validationRules;
        private XmlValidator _validator;
        private XmlParser _parser;

        [SetUp]
        public void SetUp()
        {
            _validator = new XmlValidator();
            _parser = new XmlParser();

            var worker = new XmlCommonWorker();

            _validationSettings = worker.GetValidationSettingsForProducts(new List<int> {1,2,3,4,5}).ToList();
            _validationRules = worker.GetValidationRules();

            worker.SetRulesForSettings(_validationSettings,_validationRules);
        }

        private void InitializeEvents(int settingId, string testCaseName, ref List<Event> messageEvents)
        {
            var fileName = "..\\..\\TestCases\\ValidationSetting #" + settingId + "\\" + testCaseName + ".xml";
            using (XmlTextReader myTextReader = new XmlTextReader(fileName))
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

                            var newEvent = _parser.LoadEvent(true,1, headerXml, xmlText,
                                new List<string>(), new List<string>(), new List<string>(), new List<string>(), new List<string>(),
                                new List<string>(), new List<string>(), new List<string>());

                            messageEvents.Add(newEvent);
                        }
                        else if (myTextReader.LocalName == "Header")
                        {
                            headerXml = (myTextReader.ReadOuterXml());
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
        }
        
        [Test]
        [TestCase(29)]
        public void ValidationSetting29_Negative1_ReturnsEmptyList(int settingId)
        {
            //Arrange
            var messageEvents = new List<Event>();
            InitializeEvents(settingId, "Negative#1", ref messageEvents);

            //Act
            var result = _validator.ValidateEventsCommon(messageEvents, _validationSettings.Where(x => x.Id == settingId).ToList());

            //Assert
            Assert.AreEqual(0, result.Count);
        }
        [Test]
        [TestCase(29)]
        public void ValidationSetting29_Positive1_ReturnsCorrectList(int settingId)
        {
            //Arrange
            var messageEvents = new List<Event>();
            InitializeEvents(settingId, "Positive#1", ref messageEvents);

            //Act
            var result = _validator.ValidateEventsCommon(messageEvents, _validationSettings.Where(x => x.ProductId == 1).Where(x => x.Id == settingId).ToList());

            //Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Football", result[0].Result.SportName);
            Assert.AreEqual("Scotland", result[0].Result.LocationName);
            Assert.AreEqual("Premiership League", result[0].Result.LeagueName);
            Assert.AreEqual("St. Johnstone W W vs Ross CountyWW", result[0].Result.EventName);
            Assert.AreEqual("", result[0].Result.Market);
            Assert.AreEqual("", result[0].Result.Provider);
            Assert.AreEqual("HomeTeam@Name=\"St. Johnstone W W\"@Name=\"St. Johnstone W W\"", result[0].Result.PointsToHighline);
        }

        [Test]
        [TestCase(29)]
        public void ValidationSetting29_Positive2_ReturnsCorrectList(int settingId)
        {
            //Arrange
            var messageEvents = new List<Event>();
            InitializeEvents(settingId, "Positive#2", ref messageEvents);

            //Act
            var result = _validator.ValidateEventsCommon(messageEvents, _validationSettings.Where(x => x.ProductId == 1).Where(x => x.Id == settingId).ToList());

            //Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Football", result[0].Result.SportName);
            Assert.AreEqual("Scotland", result[0].Result.LocationName);
            Assert.AreEqual("Premiership League", result[0].Result.LeagueName);
            Assert.AreEqual("St. Johnstone W W vs Ross County W W", result[0].Result.EventName);
            Assert.AreEqual("", result[0].Result.Market);
            Assert.AreEqual("", result[0].Result.Provider);
            Assert.AreEqual("HomeTeam@Name=\"St. Johnstone W W\"@Name=\"St. Johnstone W W\"|AwayTeam@Name=\"Ross County W W\"@Name=\"Ross County W W\"", result[0].Result.PointsToHighline);
        }



        #region ValidationSetting #30
        [Test]
        [TestCase(30)]
        public void ValidationSetting30_Negative1_ReturnsEmptyList(int settingId)
        {
            //Arrange
            var messageEvents = new List<Event>();
            InitializeEvents(settingId, "Negative#1", ref messageEvents);

            //Act
            var result = _validator.ValidateEventsCommon(messageEvents, _validationSettings.Where(x => x.Id == settingId).ToList());

            //Assert
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        [TestCase(30)]
        public void ValidationSetting30_Negative2_ReturnsEmptyList(int settingId)
        {
            //Arrange
            var messageEvents = new List<Event>();
            InitializeEvents(settingId, "Negative#2", ref messageEvents);

            //Act
            var result = _validator.ValidateEventsCommon(messageEvents, _validationSettings.Where(x => x.Id == settingId).ToList());

            //Assert
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        [TestCase(30)]
        public void ValidationSetting30_Negative3_ReturnsEmptyList(int settingId)
        {
            //Arrange
            var messageEvents = new List<Event>();
            InitializeEvents(settingId, "Negative#3", ref messageEvents);

            //Act
            var result = _validator.ValidateEventsCommon(messageEvents, _validationSettings.Where(x => x.Id == settingId).ToList());

            //Assert
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        [TestCase(30)]
        public void ValidationSetting30_Positive1_ReturnsCorrectList(int settingId)
        {
            //Arrange
            var messageEvents = new List<Event>();
            InitializeEvents(settingId, "Positive#1", ref messageEvents);

            //Act
            var result = _validator.ValidateEventsCommon(messageEvents, _validationSettings.Where(x => x.Id == settingId).ToList());

            //Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Football", result[0].Result.SportName);
            Assert.AreEqual("Scotland", result[0].Result.LocationName);
            Assert.AreEqual("Premiership League", result[0].Result.LeagueName);
            Assert.AreEqual("St. Johnstone vs Ross County", result[0].Result.EventName);
            Assert.AreEqual("Asian Handicap", result[0].Result.Market);
            Assert.AreEqual("Ladbrokes", result[0].Result.Provider);
            Assert.AreEqual("Odds@id=\"4382102812242903\"@BaseLine=\"\"|Outcome@id=\"3\"@id=\"3\"|Bookmaker@id=\"81\"@id=\"81\"", result[0].Result.PointsToHighline);
        }

        [Test]
        [TestCase(30)]
        public void ValidationSetting30_Positive2_ReturnsCorrectList(int settingId)
        {
            //Arrange
            var messageEvents = new List<Event>();
            InitializeEvents(settingId, "Positive#2", ref messageEvents);

            //Act
            var result = _validator.ValidateEventsCommon(messageEvents, _validationSettings.Where(x => x.Id == settingId).ToList());

            //Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Football", result[0].Result.SportName);
            Assert.AreEqual("Scotland", result[0].Result.LocationName);
            Assert.AreEqual("Premiership League", result[0].Result.LeagueName);
            Assert.AreEqual("St. Johnstone vs Ross County", result[0].Result.EventName);
            Assert.AreEqual("Asian Handicap", result[0].Result.Market);
            Assert.AreEqual("Ladbrokes", result[0].Result.Provider);
            Assert.AreEqual("Odds@id=\"4382102812242903\"@BaseLine=\"\"|Odds@id=\"15500655032242903\"@BaseLine=\"\"|Outcome@id=\"3\"@id=\"3\"|Bookmaker@id=\"81\"@id=\"81\"", result[0].Result.PointsToHighline);
        }

        #endregion

        
        [Test]
        [TestCase(42)]
        public void ValidationSetting42_Negative1_ReturnsEmptyList(int settingId)
        {
            //Arrange
            var messageEvents = new List<Event>();
            InitializeEvents(settingId, "Negative#1", ref messageEvents);

            //Act
            var result = _validator.ValidateEventsCommon(messageEvents, _validationSettings.Where(x => x.Id == settingId).ToList());

            //Assert
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        [TestCase(42)]
        public void ValidationSetting42_Negative2_ReturnsEmptyList(int settingId)
        {
            //Arrange
            var messageEvents = new List<Event>();
            InitializeEvents(settingId, "Negative#2", ref messageEvents);

            //Act
            var result = _validator.ValidateEventsCommon(messageEvents, _validationSettings.Where(x => x.Id == settingId).ToList());

            //Assert
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        [TestCase(42)]
        public void ValidationSetting42_Negative3_ReturnsEmptyList(int settingId)
        {
            //Arrange
            var messageEvents = new List<Event>();
            InitializeEvents(settingId, "Negative#3", ref messageEvents);

            //Act
            var result = _validator.ValidateEventsCommon(messageEvents, _validationSettings.Where(x => x.Id == settingId).ToList());

            //Assert
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        [TestCase(42)]
        public void ValidationSetting42_Positive1_ReturnsCorrectList(int settingId)
        {
            //Arrange
            var messageEvents = new List<Event>();
            InitializeEvents(settingId, "Positive#1", ref messageEvents);

            //Act
            var result = _validator.ValidateEventsCommon(messageEvents, _validationSettings.Where(x => x.Id == settingId).ToList());

            //Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Football", result[0].Result.SportName);
            Assert.AreEqual("Scotland", result[0].Result.LocationName);
            Assert.AreEqual("Premiership League", result[0].Result.LeagueName);
            Assert.AreEqual("St. Johnstone vs Ross County", result[0].Result.EventName);
            Assert.AreEqual("Asian Handicap", result[0].Result.Market);
            Assert.AreEqual("Ladbrokes", result[0].Result.Provider);
            Assert.AreEqual("Odds@id=\"4382102812242903\"@id=\"4382102812242903\"|Odds@id=\"15500655032242903\"@id=\"15500655032242903\"|Status@@Finished", result[0].Result.PointsToHighline);
        }

        [Test]
        [TestCase(42)]
        public void ValidationSetting42_Positive2_ReturnsCorrectList(int settingId)
        {
            //Arrange
            var messageEvents = new List<Event>();
            InitializeEvents(settingId, "Positive#2", ref messageEvents);

            //Act
            var result = _validator.ValidateEventsCommon(messageEvents, _validationSettings.Where(x => x.Id == settingId).ToList());

            //Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Football", result[0].Result.SportName);
            Assert.AreEqual("Scotland", result[0].Result.LocationName);
            Assert.AreEqual("Premiership League", result[0].Result.LeagueName);
            Assert.AreEqual("St. Johnstone vs Ross County", result[0].Result.EventName);
            Assert.AreEqual("Asian Handicap", result[0].Result.Market);
            Assert.AreEqual("Ladbrokes", result[0].Result.Provider);
            Assert.AreEqual("Odds@id=\"4382102812242903\"@id=\"4382102812242903\"|Odds@id=\"155006550322429031\"@id=\"155006550322429031\"|Status@@Finished", result[0].Result.PointsToHighline);
        }
        


        
        [Test]
        [TestCase(47)]
        public void ValidationSetting47_Negative1_ReturnsEmptyList(int settingId)
        {
            //Arrange
            var messageEvents = new List<Event>();
            InitializeEvents(settingId, "Negative#1", ref messageEvents);

            //Act
            var result = _validator.ValidateEventsCommon(messageEvents, _validationSettings.Where(x => x.Id == settingId).ToList());

            //Assert
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        [TestCase(47)]
        public void ValidationSetting47_Negative2_ReturnsEmptyList(int settingId)
        {
            //Arrange
            var messageEvents = new List<Event>();
            InitializeEvents(settingId, "Negative#2", ref messageEvents);

            //Act
            var result = _validator.ValidateEventsCommon(messageEvents, _validationSettings.Where(x => x.Id == settingId).ToList());

            //Assert
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        [TestCase(47)]
        public void ValidationSetting47_Negative3_ReturnsEmptyList(int settingId)
        {
            //Arrange
            var messageEvents = new List<Event>();
            InitializeEvents(settingId, "Negative#3", ref messageEvents);

            //Act
            var result = _validator.ValidateEventsCommon(messageEvents, _validationSettings.Where(x => x.Id == settingId).ToList());

            //Assert
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        [TestCase(47)]
        public void ValidationSetting47_Positive1_ReturnsCorrectList(int settingId)
        {
            //Arrange
            var messageEvents = new List<Event>();
            InitializeEvents(settingId, "Positive#1", ref messageEvents);

            //Act
            var result = _validator.ValidateEventsCommon(messageEvents, _validationSettings.Where(x => x.Id == settingId).ToList());

            //Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Football", result[0].Result.SportName);
            Assert.AreEqual("Scotland", result[0].Result.LocationName);
            Assert.AreEqual("Premiership League", result[0].Result.LeagueName);
            Assert.AreEqual("St. Johnstone vs Ross County", result[0].Result.EventName);
            Assert.AreEqual("Asian Handicap", result[0].Result.Market);
            Assert.AreEqual("Ladbrokes", result[0].Result.Provider);
            Assert.AreEqual("Odds@id=\"4382102812242903\"@Status=\"Open\"|Odds@id=\"15500655032242903\"@Status=\"Open\"|Status@@Finished", result[0].Result.PointsToHighline);
        }

        [Test]
        [TestCase(47)]
        public void ValidationSetting47_Positive2_ReturnsCorrectList(int settingId)
        {
            //Arrange
            var messageEvents = new List<Event>();
            InitializeEvents(settingId, "Positive#2", ref messageEvents);

            //Act
            var result = _validator.ValidateEventsCommon(messageEvents, _validationSettings.Where(x => x.Id == settingId).ToList());

            //Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Football", result[0].Result.SportName);
            Assert.AreEqual("Scotland", result[0].Result.LocationName);
            Assert.AreEqual("Premiership League", result[0].Result.LeagueName);
            Assert.AreEqual("St. Johnstone vs Ross County", result[0].Result.EventName);
            Assert.AreEqual("Asian Handicap", result[0].Result.Market);
            Assert.AreEqual("Ladbrokes", result[0].Result.Provider);
            Assert.AreEqual("Odds@id=\"4382102812242903\"@Status=\"Open\"|Odds@id=\"15500655032242903\"@Status=\"Open\"|Status@@inprogress", result[0].Result.PointsToHighline);
        }
        



        #region ValidationSetting #49
        [Test]
        [TestCase(49)]
        public void ValidationSetting49_Negative1_ReturnsEmptyList(int settingId)
        {
            //Arrange
            var messageEvents = new List<Event>();
            InitializeEvents(settingId, "Negative#1", ref messageEvents);

            //Act
            var result = _validator.ValidateEventsCommon(messageEvents, _validationSettings.Where(x => x.Id == settingId).ToList());

            //Assert
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        [TestCase(49)]
        public void ValidationSetting49_Negative2_ReturnsEmptyList(int settingId)
        {
            //Arrange
            var messageEvents = new List<Event>();
            InitializeEvents(settingId, "Negative#2", ref messageEvents);

            //Act
            var result = _validator.ValidateEventsCommon(messageEvents, _validationSettings.Where(x => x.Id == settingId).ToList());

            //Assert
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        [TestCase(49)]
        public void ValidationSetting49_Negative3_ReturnsEmptyList(int settingId)
        {
            //Arrange
            var messageEvents = new List<Event>();
            InitializeEvents(settingId, "Negative#3", ref messageEvents);

            //Act
            var result = _validator.ValidateEventsCommon(messageEvents, _validationSettings.Where(x => x.Id == settingId).ToList());

            //Assert
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        [TestCase(49)]
        public void ValidationSetting49_Positive1_ReturnsCorrectList(int settingId)
        {
            //Arrange
            var messageEvents = new List<Event>();
            InitializeEvents(settingId, "Positive#1", ref messageEvents);

            //Act
            var result = _validator.ValidateEventsCommon(messageEvents, _validationSettings.Where(x => x.Id == settingId).ToList());

            //Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Football", result[0].Result.SportName);
            Assert.AreEqual("Scotland", result[0].Result.LocationName);
            Assert.AreEqual("Premiership League", result[0].Result.LeagueName);
            Assert.AreEqual("St. Johnstone vs Ross County", result[0].Result.EventName);
            Assert.AreEqual("Asian Handicap", result[0].Result.Market);
            Assert.AreEqual("Ladbrokes", result[0].Result.Provider);
            Assert.AreEqual("Odds@id=\"4382102812242903\"@line=\"-0.5\"|Odds@id=\"4382102812242903\"@Status=\"Open\"|Odds@id=\"4382102812242904\"@line=\"-0.5\"|Odds@id=\"4382102812242904\"@Status=\"Open\"|Outcome@id=\"3\"@id=\"3\"", result[0].Result.PointsToHighline);
        }

        #endregion



        #region ValidationSetting #50
        [Test]
        [TestCase(50)]
        public void ValidationSetting50_Negative1_ReturnsEmptyList(int settingId)
        {
            //Arrange
            var messageEvents = new List<Event>();
            InitializeEvents(settingId, "Negative#1", ref messageEvents);

            //Act
            var result = _validator.ValidateEventsCommon(messageEvents, _validationSettings.Where(x => x.Id == settingId).ToList());

            //Assert
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        [TestCase(50)]
        public void ValidationSetting50_Negative2_ReturnsEmptyList(int settingId)
        {
            //Arrange
            var messageEvents = new List<Event>();
            InitializeEvents(settingId, "Negative#2", ref messageEvents);

            //Act
            var result = _validator.ValidateEventsCommon(messageEvents, _validationSettings.Where(x => x.Id == settingId).ToList());

            //Assert
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        [TestCase(50)]
        public void ValidationSetting50_Negative3_ReturnsEmptyList(int settingId)
        {
            //Arrange
            var messageEvents = new List<Event>();
            InitializeEvents(settingId, "Negative#3", ref messageEvents);

            //Act
            var result = _validator.ValidateEventsCommon(messageEvents, _validationSettings.Where(x => x.Id == settingId).ToList());

            //Assert
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        [TestCase(50)]
        public void ValidationSetting50_Positive1_ReturnsCorrectList(int settingId)
        {
            //Arrange
            var messageEvents = new List<Event>();
            InitializeEvents(settingId, "Positive#1", ref messageEvents);

            //Act
            var result = _validator.ValidateEventsCommon(messageEvents, _validationSettings.Where(x => x.Id == settingId).ToList());

            //Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Football", result[0].Result.SportName);
            Assert.AreEqual("Scotland", result[0].Result.LocationName);
            Assert.AreEqual("Premiership League", result[0].Result.LeagueName);
            Assert.AreEqual("St. Johnstone vs Ross County", result[0].Result.EventName);
            Assert.AreEqual("Under/Over", result[0].Result.Market);
            Assert.AreEqual("Ladbrokes", result[0].Result.Provider);
            Assert.AreEqual("Odds@id=\"4382102812242903\"@line=\"s-0.5\"|Odds@id=\"4382102812242903\"@Status=\"Open\"|Odds@id=\"15500655032242903\"@line=\"s0.5\"|Odds@id=\"15500655032242903\"@Status=\"Open\"|Odds@id=\"43821028122429089\"@line=\"s-0.5\"|Odds@id=\"43821028122429089\"@Status=\"Open\"|Outcome@id=\"3\"@name=\"Under/Over\"", result[0].Result.PointsToHighline);
        }

        #endregion


        
        [Test]
        [TestCase(55)]
        public void ValidationSetting55_Negative1_ReturnsEmptyList(int settingId)
        {
            //Arrange
            var messageEvents = new List<Event>();
            InitializeEvents(settingId, "Negative#1", ref messageEvents);

            //Act
            var result = _validator.ValidateEventsCommon(messageEvents, _validationSettings.Where(x => x.Id == settingId).ToList());

            //Assert
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        [TestCase(55)]
        public void ValidationSetting55_Positive1_ReturnsCorrectList(int settingId)
        {
            //Arrange
            var messageEvents = new List<Event>();
            InitializeEvents(settingId, "Positive#1", ref messageEvents);

            //Act
            var result = _validator.ValidateEventsCommon(messageEvents, _validationSettings.Where(x => x.Id == settingId).ToList());

            //Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Football", result[0].Result.SportName);
            Assert.AreEqual("Scotland", result[0].Result.LocationName);
            Assert.AreEqual("Premiership League", result[0].Result.LeagueName);
            Assert.AreEqual("12345 vs Ross CountyWW", result[0].Result.EventName);
            Assert.AreEqual("HomeTeam@Name=\"12345\"@Name=\"12345\"", result[0].Result.PointsToHighline);
        }

        [Test]
        [TestCase(55)]
        public void ValidationSetting55_Positive2_ReturnsCorrectList(int settingId)
        {
            //Arrange
            var messageEvents = new List<Event>();
            InitializeEvents(settingId, "Positive#2", ref messageEvents);

            //Act
            var result = _validator.ValidateEventsCommon(messageEvents, _validationSettings.Where(x => x.Id == settingId).ToList());

            //Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Football", result[0].Result.SportName);
            Assert.AreEqual("Scotland", result[0].Result.LocationName);
            Assert.AreEqual("Premiership League", result[0].Result.LeagueName);
            Assert.AreEqual("St. Johnstone vs 54323", result[0].Result.EventName);
            Assert.AreEqual("AwayTeam@Name=\"54323\"@Name=\"54323\"", result[0].Result.PointsToHighline);
        }

        [Test]
        [TestCase(55)]
        public void ValidationSetting55_Positive3_ReturnsCorrectList(int settingId)
        {
            //Arrange
            var messageEvents = new List<Event>();
            InitializeEvents(settingId, "Positive#3", ref messageEvents);

            //Act
            var result = _validator.ValidateEventsCommon(messageEvents, _validationSettings.Where(x => x.Id == settingId).ToList());

            //Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Football", result[0].Result.SportName);
            Assert.AreEqual("Scotland", result[0].Result.LocationName);
            Assert.AreEqual("Premiership League", result[0].Result.LeagueName);
            Assert.AreEqual("7623 vs 54323", result[0].Result.EventName);
            Assert.AreEqual("HomeTeam@Name=\"7623\"@Name=\"7623\"|AwayTeam@Name=\"54323\"@Name=\"54323\"", result[0].Result.PointsToHighline);
        }
        



        #region ValidationSetting #57
        [Test]
        [TestCase(57)]
        public void ValidationSetting57_Negative1_ReturnsEmptyList(int settingId)
        {
            //Arrange
            var messageEvents = new List<Event>();
            InitializeEvents(settingId, "Negative#1", ref messageEvents);

            //Act
            var result = _validator.ValidateEventsCommon(messageEvents, _validationSettings.Where(x => x.Id == settingId).ToList());

            //Assert
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        [TestCase(57)]
        public void ValidationSetting57_Negative2_ReturnsEmptyList(int settingId)
        {
            //Arrange
            var messageEvents = new List<Event>();
            InitializeEvents(settingId, "Negative#2", ref messageEvents);

            //Act
            var result = _validator.ValidateEventsCommon(messageEvents, _validationSettings.Where(x => x.Id == settingId).ToList());

            //Assert
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        [TestCase(57)]
        public void ValidationSetting57_Negative3_ReturnsEmptyList(int settingId)
        {
            //Arrange
            var messageEvents = new List<Event>();
            InitializeEvents(settingId, "Negative#3", ref messageEvents);

            //Act
            var result = _validator.ValidateEventsCommon(messageEvents, _validationSettings.Where(x => x.Id == settingId).ToList());

            //Assert
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        [TestCase(57)]
        public void ValidationSetting57_Negative4_ReturnsEmptyList(int settingId)
        {
            //Arrange
            var messageEvents = new List<Event>();
            InitializeEvents(settingId, "Negative#4", ref messageEvents);

            //Act
            var result = _validator.ValidateEventsCommon(messageEvents, _validationSettings.Where(x => x.Id == settingId).ToList());

            //Assert
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        [TestCase(57)]
        public void ValidationSetting57_Positive1_ReturnsCorrectList(int settingId)
        {
            //Arrange
            var messageEvents = new List<Event>();
            InitializeEvents(settingId, "Positive#1", ref messageEvents);

            //Act
            var result = _validator.ValidateEventsCommon(messageEvents, _validationSettings.Where(x => x.Id == settingId).ToList());

            //Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Football", result[0].Result.SportName);
            Assert.AreEqual("Scotland", result[0].Result.LocationName);
            Assert.AreEqual("Premiership League", result[0].Result.LeagueName);
            Assert.AreEqual("12345 vs Ross County", result[0].Result.EventName);
            Assert.AreEqual("Under/Over - Home Team", result[0].Result.Market);
            Assert.AreEqual("Bet365", result[0].Result.Provider);
            Assert.AreEqual("Odds@id=\"17430310252370256\"@line=\"1.5\"|Odds@id=\"17430310252370256\"@Status=\"Open\"|Outcome@id=\"101\"@id=\"101\"", result[0].Result.PointsToHighline);
        }

        [Test]
        [TestCase(57)]
        public void ValidationSetting57_Positive2_ReturnsCorrectList(int settingId)
        {
            //Arrange
            var messageEvents = new List<Event>();
            InitializeEvents(settingId, "Positive#2", ref messageEvents);

            //Act
            var result = _validator.ValidateEventsCommon(messageEvents, _validationSettings.Where(x => x.Id == settingId).ToList());

            //Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Football", result[0].Result.SportName);
            Assert.AreEqual("Scotland", result[0].Result.LocationName);
            Assert.AreEqual("Premiership League", result[0].Result.LeagueName);
            Assert.AreEqual("12345 vs Ross County", result[0].Result.EventName);
            Assert.AreEqual("Under/Over - Home Team", result[0].Result.Market);
            Assert.AreEqual("Bet365", result[0].Result.Provider);
            Assert.AreEqual("Odds@id=\"17430310252370256\"@line=\"1.5\"|Odds@id=\"17430310252370256\"@Status=\"Open\"|Odds@id=\"17430309302370256\"@line=\"1.9\"|Odds@id=\"17430309302370256\"@Status=\"Open\"|Outcome@id=\"101\"@id=\"101\"", result[0].Result.PointsToHighline);
        }

        #endregion



        #region ValidationSetting #59
        [Test]
        [TestCase(59)]
        public void ValidationSetting59_Negative1_ReturnsEmptyList(int settingId)
        {
            //Arrange
            var messageEvents = new List<Event>();
            InitializeEvents(settingId, "Negative#1", ref messageEvents);

            //Act
            var result = _validator.ValidateEventsCommon(messageEvents, _validationSettings.Where(x => x.Id == settingId).ToList());

            //Assert
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        [TestCase(59)]
        public void ValidationSetting59_Negative2_ReturnsEmptyList(int settingId)
        {
            //Arrange
            var messageEvents = new List<Event>();
            InitializeEvents(settingId, "Negative#2", ref messageEvents);

            //Act
            var result = _validator.ValidateEventsCommon(messageEvents, _validationSettings.Where(x => x.Id == settingId).ToList());

            //Assert
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        [TestCase(59)]
        public void ValidationSetting59_Positive1_ReturnsCorrectList(int settingId)
        {
            //Arrange
            var messageEvents = new List<Event>();
            InitializeEvents(settingId, "Positive#1", ref messageEvents);

            //Act
            var result = _validator.ValidateEventsCommon(messageEvents, _validationSettings.Where(x => x.Id == settingId).ToList());

            //Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Football", result[0].Result.SportName);
            Assert.AreEqual("Scotland", result[0].Result.LocationName);
            Assert.AreEqual("Premiership League", result[0].Result.LeagueName);
            Assert.AreEqual("St. Johnstone vs 54323", result[0].Result.EventName);
            Assert.AreEqual("", result[0].Result.Market);
            Assert.AreEqual("", result[0].Result.Provider);
            Assert.AreEqual("Status@@\n        Finished\n        ", result[0].Result.PointsToHighline);
        }

        [Test]
        [TestCase(59)]
        public void ValidationSetting59_Positive2_ReturnsCorrectList(int settingId)
        {
            //Arrange
            var messageEvents = new List<Event>();
            InitializeEvents(settingId, "Positive#2", ref messageEvents);

            //Act
            var result = _validator.ValidateEventsCommon(messageEvents, _validationSettings.Where(x => x.Id == settingId).ToList());

            //Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Football", result[0].Result.SportName);
            Assert.AreEqual("Scotland", result[0].Result.LocationName);
            Assert.AreEqual("Premiership League", result[0].Result.LeagueName);
            Assert.AreEqual("7623 vs 54323", result[0].Result.EventName);
            Assert.AreEqual("", result[0].Result.Market);
            Assert.AreEqual("", result[0].Result.Provider);
            // Assert.AreEqual("Odds@id=\"17430310252370256\"@line=\"1.5\"|Odds@id=\"17430310252370256\"@line=\"1.5\"|Odds@id=\"17430310252370256\"@Status=\"Open\"|Odds@id=\"17430309302370256\"@line=\"1.9\"|Odds@id=\"17430309302370256\"@line=\"1.9\"|Odds@id=\"17430309302370256\"@Status=\"Open\"|Outcome@id=\"101\"@id=\"101\"", result[0].Result.PointsToHighline);
        }

        #endregion



        #region ValidationSetting #65
        [Test]
        [TestCase(65)]
        public void ValidationSetting65_Negative1_ReturnsEmptyList(int settingId)
        {
            //Arrange
            var messageEvents = new List<Event>();
            InitializeEvents(settingId, "Negative#1", ref messageEvents);

            //Act
            var result = _validator.ValidateEventsCommon(messageEvents, _validationSettings.Where(x => x.Id == settingId).ToList());

            //Assert
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        [TestCase(65)]
        public void ValidationSetting65_Positive1_ReturnsCorrectList(int settingId)
        {
            //Arrange
            var messageEvents = new List<Event>();
            InitializeEvents(settingId, "Positive#1", ref messageEvents);

            //Act
            var result = _validator.ValidateEventsCommon(messageEvents, _validationSettings.Where(x => x.Id == settingId).ToList());

            //Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(null, result[0].Result.SportName);
            Assert.AreEqual(null, result[0].Result.LocationName);
            Assert.AreEqual(null, result[0].Result.LeagueName);
            Assert.AreEqual("Germany U20 W W vs France U20 W", result[0].Result.EventName);
            Assert.AreEqual("", result[0].Result.Market);
            Assert.AreEqual("", result[0].Result.Provider);
            Assert.AreEqual("HomeTeam@Id=\"228888\"@Germany U20 W W", result[0].Result.PointsToHighline);
        }

        [Test]
        [TestCase(65)]
        public void ValidationSetting65_Positive2_ReturnsCorrectList(int settingId)
        {
            //Arrange
            var messageEvents = new List<Event>();
            InitializeEvents(settingId, "Positive#2", ref messageEvents);

            //Act
            var result = _validator.ValidateEventsCommon(messageEvents, _validationSettings.Where(x => x.Id == settingId).ToList());

            //Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(null, result[0].Result.SportName);
            Assert.AreEqual(null, result[0].Result.LocationName);
            Assert.AreEqual(null, result[0].Result.LeagueName);
            Assert.AreEqual("Germany U20 W W vs France U20 W W", result[0].Result.EventName);
            Assert.AreEqual("", result[0].Result.Market);
            Assert.AreEqual("", result[0].Result.Provider);
            Assert.AreEqual("HomeTeam@Id=\"228888\"@Germany U20 W W|AwayTeam@Id=\"228891\"@France U20 W W", result[0].Result.PointsToHighline);
        }

        #endregion

        [Test]
        [TestCase(88)]
        public void ValidationSetting88_Negative1_ReturnsEmptyList(int settingId)
        {
            //Arrange
            var messageEvents = new List<Event>();
            InitializeEvents(settingId, "Negative#1", ref messageEvents);

            //Act
            var result = _validator.ValidateEventsCommon(messageEvents, _validationSettings.Where(x => x.Id == settingId).ToList());

            //Assert
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        [TestCase(88)]
        public void ValidationSetting88_Negative2_ReturnsEmptyList(int settingId)
        {
            //Arrange
            var messageEvents = new List<Event>();
            InitializeEvents(settingId, "Negative#2", ref messageEvents);

            //Act
            var result = _validator.ValidateEventsCommon(messageEvents, _validationSettings.Where(x => x.Id == settingId).ToList());

            //Assert
            Assert.AreEqual(0, result.Count);
        }


        [Test]
        [TestCase(88)]
        public void ValidationSetting88_Negative3_ReturnsEmptyList(int settingId)
        {
            //Arrange
            var messageEvents = new List<Event>();
            InitializeEvents(settingId, "Negative#3", ref messageEvents);

            //Act
            var result = _validator.ValidateEventsCommon(messageEvents, _validationSettings.Where(x => x.Id == settingId).ToList());

            //Assert
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        [TestCase(88)]
        public void ValidationSetting88_Positive1_ReturnsCorrectList(int settingId)
        {
            //Arrange
            var messageEvents = new List<Event>();
            InitializeEvents(settingId, "Positive#1", ref messageEvents);

            //Act
            var result = _validator.ValidateEventsCommon(messageEvents, _validationSettings.Where(x => x.Id == settingId).ToList());

            //Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Horse Racing", result[0].Result.SportName);
            Assert.AreEqual("Chile", result[0].Result.LocationName);
            Assert.AreEqual("Chile", result[0].Result.LeagueName);
            Assert.AreEqual("Chile", result[0].Result.EventName);
            Assert.AreEqual("", result[0].Result.Market);
            Assert.AreEqual("", result[0].Result.Provider);
            // Assert.AreEqual("Position@@\"\"", result[0].Result.PointsToHighline);
        }

        [Test]
        [TestCase(88)]
        public void ValidationSetting88_Positive2_ReturnsCorrectList(int settingId)
        {
            //Arrange
            var messageEvents = new List<Event>();
            InitializeEvents(settingId, "Positive#2", ref messageEvents);

            //Act
            var result = _validator.ValidateEventsCommon(messageEvents, _validationSettings.Where(x => x.Id == settingId).ToList());

            //Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Horse Racing", result[0].Result.SportName);
            Assert.AreEqual("Chile", result[0].Result.LocationName);
            Assert.AreEqual("Chile", result[0].Result.LeagueName);
            Assert.AreEqual("Chile", result[0].Result.EventName);
            Assert.AreEqual("", result[0].Result.Market);
            Assert.AreEqual("", result[0].Result.Provider);
            Assert.AreEqual("Status@@Finished|SportID@Name=\"Horse Racing\"@687888|Participant@Number=\"4\"@Position=\"a\"|Participant@Number=\"3\"@Position=\"b\"|Participant@Number=\"6\"@Position=\"c\"|Participant@Number=\"1\"@Position=\"d\"|Participant@Number=\"2\"@Position=\"e\"", result[0].Result.PointsToHighline);
        }

        [Test]
        [TestCase(88)]
        public void ValidationSetting88_Positive3_ReturnsCorrectList(int settingId)
        {
            //Arrange
            var messageEvents = new List<Event>();
            InitializeEvents(settingId, "Positive#3", ref messageEvents);

            //Act
            var result = _validator.ValidateEventsCommon(messageEvents, _validationSettings.Where(x => x.Id == settingId).ToList());

            //Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Horse Racing", result[0].Result.SportName);
            Assert.AreEqual("Chile", result[0].Result.LocationName);
            Assert.AreEqual("Chile", result[0].Result.LeagueName);
            Assert.AreEqual("Chile", result[0].Result.EventName);
            Assert.AreEqual("", result[0].Result.Market);
            Assert.AreEqual("", result[0].Result.Provider);
            Assert.AreEqual("Status@@Finished|SportID@Name=\"Horse Racing\"@687888|Participant@Number=\"4\"@Number=\"4\"|Participant@Number=\"3\"@Number=\"3\"|Participant@Number=\"6\"@Number=\"6\"|Participant@Number=\"1\"@Number=\"1\"|Participant@Number=\"2\"@Number=\"2\"", result[0].Result.PointsToHighline);
        }

        [Test]
        [TestCase(88)]
        public void ValidationSetting88_Positive4_ReturnsCorrectList(int settingId)
        {
            //Arrange
            var messageEvents = new List<Event>();
            InitializeEvents(settingId, "Positive#4", ref messageEvents);

            //Act
            var result = _validator.ValidateEventsCommon(messageEvents, _validationSettings.Where(x => x.Id == settingId).ToList());

            //Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Horse Racing", result[0].Result.SportName);
            Assert.AreEqual("Chile", result[0].Result.LocationName);
            Assert.AreEqual("Chile", result[0].Result.LeagueName);
            Assert.AreEqual("Chile", result[0].Result.EventName);
            Assert.AreEqual("", result[0].Result.Market);
            Assert.AreEqual("", result[0].Result.Provider);
            Assert.AreEqual("Status@@Finished|SportID@Name=\"Horse Racing\"@687888|Participant@Number=\"4\"@Position=\"a\"|Participant@Number=\"3\"@Position=\"b\"|Participant@Number=\"6\"@Position=\"c\"|Participant@Number=\"1\"@Position=\"d\"|Participant@Number=\"2\"@Position=\"a\"", result[0].Result.PointsToHighline);
        }


        [Test]
        [TestCase(88)]
        public void ValidationSetting88_Positive5_ReturnsCorrectList(int settingId)
        {
            //Arrange
            var messageEvents = new List<Event>();
            InitializeEvents(settingId, "Positive#5", ref messageEvents);

            //Act
            var result = _validator.ValidateEventsCommon(messageEvents, _validationSettings.Where(x => x.Id == settingId).ToList());

            //Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Horse Racing", result[0].Result.SportName);
            Assert.AreEqual("United States", result[0].Result.LocationName);
            Assert.AreEqual("Dover Downs", result[0].Result.LeagueName);
            Assert.AreEqual("Dover Downs", result[0].Result.EventName);
            Assert.AreEqual("", result[0].Result.Market);
            Assert.AreEqual("", result[0].Result.Provider);
            Assert.AreEqual("Status@@Finished|SportID@Name=\"Horse Racing\"@687888|@@", result[0].Result.PointsToHighline);
        }

        [Test]
        [TestCase(133)]
        public void ValidationSetting133_Positive1_ReturnsCorrectList(int settingId)
        {
            //Arrange
            var messageEvents = new List<Event>();
            InitializeEvents(settingId, "Positive#1", ref messageEvents);

            //Act
            var result = _validator.ValidateEventsCommon(messageEvents, _validationSettings.Where(x => x.Id == settingId).ToList());

            //Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Horse Racing", result[0].Result.SportName);
            Assert.AreEqual("Great Britain", result[0].Result.LocationName);
            Assert.AreEqual("County Hurdle", result[0].Result.LeagueName);
            Assert.AreEqual("Cheltenham", result[0].Result.EventName);
            Assert.AreEqual("Race Half", result[0].Result.Market);
            Assert.AreEqual("BetCRIS", result[0].Result.Provider);
            Assert.AreEqual("Odds@id=\"2921668462428290\"@Status=\"Open\"|Outcome@id=\"1602\"@name=\"Race Half\"|SportID@Name=\"Horse Racing\"@35232", result[0].Result.PointsToHighline);
        }
        
        [Test]
        [TestCase(165)]
        public void ValidationSetting165_Positive1_ReturnsCorrectList(int settingId)
        {
            //Arrange
            var messageEvents = new List<Event>();
            InitializeEvents(settingId, "Positive#1", ref messageEvents);

            //Act
            var result = _validator.ValidateEventsCommon(messageEvents, _validationSettings.Where(x => x.Id == settingId).ToList());

            //Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Horse Racing", result[0].Result.SportName);
            Assert.AreEqual("Great Britain", result[0].Result.LocationName);
            Assert.AreEqual("County Hurdle", result[0].Result.LeagueName);
            Assert.AreEqual("Cheltenham", result[0].Result.EventName);
            Assert.AreEqual("", result[0].Result.Market);
            Assert.AreEqual("", result[0].Result.Provider);
            Assert.AreEqual("StartDate@@2017-01-17T09:00:00.000", result[0].Result.PointsToHighline);
        }


        [Test]
        [TestCase(165)]
        public void ValidationSetting165_Negative1_ReturnsCorrectList(int settingId)
        {
            //Arrange
            var messageEvents = new List<Event>();
            InitializeEvents(settingId, "Negative#1", ref messageEvents);

            //Act
            var result = _validator.ValidateEventsCommon(messageEvents, _validationSettings.Where(x => x.Id == settingId).ToList());

            //Assert
            //Assert
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        [TestCase(166)]
        public void ValidationSetting166_Positive1_ReturnsCorrectList(int settingId)
        {
            //Arrange
            var messageEvents = new List<Event>();
            InitializeEvents(settingId, "Positive#1", ref messageEvents);

            //Act
            var result = _validator.ValidateEventsCommon(messageEvents, _validationSettings.Where(x => x.Id == settingId).ToList());

            //Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("HorceRacing", result[0].Result.SportName);
            Assert.AreEqual("Great Britain", result[0].Result.LocationName);
            Assert.AreEqual("Euroleague - Regular Season", result[0].Result.LeagueName);
            Assert.AreEqual("Brose Baskets vs Maccabi Tel Aviv", result[0].Result.EventName);
            Assert.AreEqual("", result[0].Result.Market);
            Assert.AreEqual("", result[0].Result.Provider);
            Assert.AreEqual("Status@@Cancelled|LocationID@Name=\"Great Britain\"@2|SportID@Name=\"HorceRacing\"@687888", result[0].Result.PointsToHighline);
        }

        [Test]
        [TestCase(167)]
        public void ValidationSetting167_Positive1_ReturnsCorrectList(int settingId)
        {
            //Arrange
            var messageEvents = new List<Event>();
            InitializeEvents(settingId, "Positive#1", ref messageEvents);

            //Act
            var result = _validator.ValidateEventsCommon(messageEvents, _validationSettings.Where(x => x.Id == settingId).ToList());

            //Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Ice Hockey", result[0].Result.SportName);
            Assert.AreEqual("Sweden", result[0].Result.LocationName);
            Assert.AreEqual("SHL", result[0].Result.LeagueName);
            Assert.AreEqual("Leksands IF vs Linköpings HC", result[0].Result.EventName);
            Assert.AreEqual("Asian Handicap Including Overtime", result[0].Result.Market);
            Assert.AreEqual("138", result[0].Result.Provider);
            Assert.AreEqual("Odds@id=\"9970959362201139\"@currentPrice=\"2.28\"|Odds@id=\"2164473912201139\"@currentPrice=\"3.76\"|Odds@id=\"9970621772201139\"@currentPrice=\"1.55\"|Odds@id=\"9982120312201139\"@currentPrice=\"1.62\"|Odds@id=\"7361054072201139\"@currentPrice=\"1.04\"|Odds@id=\"13606529912201139\"@currentPrice=\"2.42\"", result[0].Result.PointsToHighline);
        }

        [Test]
        [TestCase(167)]
        public void ValidationSetting167_Positive2_ReturnsCorrectList(int settingId)
        {
            //Arrange
            var messageEvents = new List<Event>();
            InitializeEvents(settingId, "Positive#2", ref messageEvents);

            //Act
            var result = _validator.ValidateEventsCommon(messageEvents, _validationSettings.Where(x => x.Id == settingId).ToList());

            //Assert
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("Ice Hockey", result[0].Result.SportName);
            Assert.AreEqual("Sweden", result[0].Result.LocationName);
            Assert.AreEqual("SHL", result[0].Result.LeagueName);
            Assert.AreEqual("Leksands IF vs Linköpings HC", result[0].Result.EventName);
            Assert.AreEqual("Asian Handicap Including Overtime", result[0].Result.Market);
            Assert.AreEqual("138", result[0].Result.Provider);
            Assert.AreEqual("Odds@id=\"9970959362201139\"@currentPrice=\"2.28\"|Odds@id=\"2164473912201139\"@currentPrice=\"3.76\"|Odds@id=\"9970621772201139\"@currentPrice=\"1.55\"|Odds@id=\"9982120312201139\"@currentPrice=\"1.62\"|Odds@id=\"7361054072201139\"@currentPrice=\"1.04\"|Odds@id=\"13606529912201139\"@currentPrice=\"2.42\"", result[0].Result.PointsToHighline);
        }

        [Test]
        [TestCase(167)]
        public void ValidationSetting167_Negative1_ReturnsCorrectList(int settingId)
        {
            //Arrange
            var messageEvents = new List<Event>();
            InitializeEvents(settingId, "Negative#1", ref messageEvents);

            //Act
            var result = _validator.ValidateEventsCommon(messageEvents, _validationSettings.Where(x => x.Id == settingId).ToList());

            //Assert
            //Assert
            Assert.AreEqual(0, result.Count);
        }
        [Test]
        [TestCase(168)]
        public void ValidationSetting168_Negative1_ReturnsCorrectList(int settingId)
        {
            //Arrange
            var messageEvents = new List<Event>();
            InitializeEvents(settingId, "Negative#1", ref messageEvents);

            //Act
            var result = _validator.ValidateEventsCommon(messageEvents, _validationSettings.Where(x => x.Id == settingId).ToList());

            //Assert
            //Assert
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        [TestCase(168)]
        public void ValidationSetting168_Positive1_ReturnsCorrectList(int settingId)
        {
            //Arrange
            var messageEvents = new List<Event>();
            InitializeEvents(settingId, "Positive#1", ref messageEvents);

            //Act
            var result = _validator.ValidateEventsCommon(messageEvents, _validationSettings.Where(x => x.Id == settingId).ToList());

            //Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Ice Hockey", result[0].Result.SportName);
            Assert.AreEqual("Sweden", result[0].Result.LocationName);
            Assert.AreEqual("SHL", result[0].Result.LeagueName);
            Assert.AreEqual("Leksands IF vs Linköpings HC", result[0].Result.EventName);
            Assert.AreEqual("European Handicap Including Overtime", result[0].Result.Market);
            Assert.AreEqual("138", result[0].Result.Provider);
            Assert.AreEqual("Odds@id=\"9970959362201139\"@line=\"0.0 (0-0)\"|Odds@id=\"2164473912201139\"@line=\"0.5 (0-0)\"|Odds@id=\"9970621772201139\"@line=\"1.0 (0-0)\"|Odds@id=\"9982120312201139\"@line=\"0.0 (0-0)\"|Odds@id=\"7361054072201139\"@line=\"-0.5 (0-0)\"|Odds@id=\"13606529912201139\"@line=\"-1.0 (0-0)\"|Outcome@id=\"342\"@name=\"European Handicap Including Overtime\"", result[0].Result.PointsToHighline);
        }

        [Test]
        [TestCase(168)]
        public void ValidationSetting168_Positive2_ReturnsCorrectList(int settingId)
        {
            //Arrange
            var messageEvents = new List<Event>();
            InitializeEvents(settingId, "Positive#2", ref messageEvents);

            //Act
            var result = _validator.ValidateEventsCommon(messageEvents, _validationSettings.Where(x => x.Id == settingId).ToList());

            //Assert
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("Ice Hockey", result[0].Result.SportName);
            Assert.AreEqual("Sweden", result[0].Result.LocationName);
            Assert.AreEqual("SHL", result[0].Result.LeagueName);
            Assert.AreEqual("Leksands IF vs Linköpings HC", result[0].Result.EventName);
            Assert.AreEqual("European Handicap Including Overtime", result[0].Result.Market);
            Assert.AreEqual("138", result[0].Result.Provider);
            Assert.AreEqual("Odds@id=\"9970959362201139\"@line=\"0.0 (0-0)\"|Odds@id=\"2164473912201139\"@line=\"0.5 (0-0)\"|Odds@id=\"9970621772201139\"@line=\"1.0 (0-0)\"|Odds@id=\"9982120312201139\"@line=\"0.0 (0-0)\"|Odds@id=\"7361054072201139\"@line=\"-0.5 (0-0)\"|Odds@id=\"13606529912201139\"@line=\"-1.0 (0-0)\"|Outcome@id=\"342\"@name=\"European Handicap Including Overtime\"", result[0].Result.PointsToHighline);
        }

        [Test]
        [TestCase(132)]
        public void ValidationSetting132_Positive1_ReturnsCorrectList(int settingId)
        {
            //Arrange
            var messageEvents = new List<Event>();
            InitializeEvents(settingId, "Positive#1", ref messageEvents);

            //Act
            var result = _validator.ValidateEventsCommon(messageEvents, _validationSettings.Where(x => x.Id == settingId).ToList());

            //Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Horse Racing", result[0].Result.SportName);
            Assert.AreEqual("Chile", result[0].Result.LocationName);
            Assert.AreEqual("Chile", result[0].Result.LeagueName);
            Assert.AreEqual("Chile", result[0].Result.EventName);
            Assert.AreEqual("", result[0].Result.Market);
            Assert.AreEqual("", result[0].Result.Provider);
            Assert.AreEqual("Status@@Finished", result[0].Result.PointsToHighline);
        }


        [Test]
        [TestCase(150)]
        public void ValidationSetting150_Positive1_ReturnsCorrectList(int settingId)
        {
            //Arrange
            var messageEvents = new List<Event>();
            InitializeEvents(settingId, "Positive#1", ref messageEvents);

            //Act
            var result = _validator.ValidateEventsCommon(messageEvents, _validationSettings.Where(x => x.Id == settingId).ToList());

            //Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Football", result[0].Result.SportName);
            Assert.AreEqual("Czech Republic", result[0].Result.LocationName);
            Assert.AreEqual("Juniorska liga", result[0].Result.LeagueName);
            Assert.AreEqual("Fk Pribram U21 vs Fc Viktoria Plzen U21", result[0].Result.EventName);
            Assert.AreEqual("Under/Over", result[0].Result.Market);
            Assert.AreEqual("CashPoint", result[0].Result.Provider);
            Assert.AreEqual("Odds@id=\"17865308931398603\"@bet=\"Under\"|Outcome@id=\"2\"@id=\"2\"|Score@period=\"CFS\"@totalScore=\"3\"|Score@period=\"CFS\"@period=\"CFS\"", result[0].Result.PointsToHighline);
        }

        [Test]
        [TestCase(173)]
        public void ValidationSetting173_Positive1_ReturnsCorrectList(int settingId)
        {
            //Arrange
            var messageEvents = new List<Event>();
            InitializeEvents(settingId, "Positive#1", ref messageEvents);

            //Act
            var result = _validator.ValidateEventsCommon(messageEvents, _validationSettings.Where(x => x.Id == settingId).ToList());

            //Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Rugby Union", result[0].Result.SportName);
            Assert.AreEqual("International", result[0].Result.LocationName);
            Assert.AreEqual("Super Rugby", result[0].Result.LeagueName);
            Assert.AreEqual("Sunwolves vs Hurricanes", result[0].Result.EventName);
            Assert.AreEqual("Asian Handicap", result[0].Result.Market);
            Assert.AreEqual("10Bet", result[0].Result.Provider);
            Assert.AreEqual("Odds@id=\"20139519492401067\"@currentPriceProbability=\"20\"|Odds@id=\"20139519492401067\"@currentPrice=\"1.01\"|Outcome@id=\"1\"@name=\"Asian Handicap\"", result[0].Result.PointsToHighline);
            
        }


        [Test]
        [TestCase(180)]
        public void ValidationSetting180_Positive1_ReturnsCorrectList(int settingId)
        {
            //Arrange
            var messageEvents = new List<Event>();
            InitializeEvents(settingId, "Positive#1", ref messageEvents);

            //Act
            var result = _validator.ValidateEventsCommon(messageEvents, _validationSettings.Where(x => x.Id == settingId).ToList());

            //Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Basketball", result[0].Result.SportName);
            Assert.AreEqual("Russia", result[0].Result.LocationName);
            Assert.AreEqual("VTB United League", result[0].Result.LeagueName);
            Assert.AreEqual("BC Tsmoki-Minsk vs Unics", result[0].Result.EventName);
            Assert.AreEqual("Asian Handicap Halftime", result[0].Result.Market);
            Assert.AreEqual("MarathonBet", result[0].Result.Provider);
            Assert.AreEqual("Odds@id=\"21450753242316696\"@currentPrice=\"1.92\"|Odds@id=\"17843630892316696\"@currentPrice=\"1.92\"|Bookmaker@id=\"74\"@name=\"MarathonBet\"|StartDate@@2017-02-05T11:00:00.000", result[0].Result.PointsToHighline);

        }

        [Test]
        [TestCase(180)]
        public void ValidationSetting180_Negative1_ReturnsCorrectList(int settingId)
        {
            //Arrange
            var messageEvents = new List<Event>();
            InitializeEvents(settingId, "Negative#1", ref messageEvents);

            //Act
            var result = _validator.ValidateEventsCommon(messageEvents, _validationSettings.Where(x => x.Id == settingId).ToList());

            //Assert
            //Assert
            Assert.AreEqual(0, result.Count);
        }



        [Test]
        [TestCase(181)]
        public void ValidationSetting181_Positive1_ReturnsCorrectList(int settingId)
        {
            //Arrange
            var messageEvents = new List<Event>();
            InitializeEvents(settingId, "Positive#1", ref messageEvents);

            //Act
            var result = _validator.ValidateEventsCommon(messageEvents, _validationSettings.Where(x => x.Id == settingId).ToList());

            //Assert
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("Rugby Union", result[0].Result.SportName);
            Assert.AreEqual("International", result[0].Result.LocationName);
            Assert.AreEqual("Super Rugby", result[0].Result.LeagueName);
            Assert.AreEqual("Sunwolves vs Hurricanes", result[0].Result.EventName);
            Assert.AreEqual("Asian Handicap", result[0].Result.Market);
            Assert.AreEqual("10Bet", result[0].Result.Provider);
            Assert.AreEqual("Odds@id=\"4478680082401067\"@currentPrice=\"11\"|Odds@id=\"20139519492401067\"@currentPrice=\"1.01\"|Odds@id=\"7417825972401067\"@currentPrice=\"44.75\"", result[0].Result.PointsToHighline);

        }


        [Test]
        [TestCase(182)]
        public void ValidationSetting182_Positive1_ReturnsCorrectList(int settingId)
        {
            //Arrange
            var messageEvents = new List<Event>();
            InitializeEvents(settingId, "Positive#1", ref messageEvents);

            //Act
            var result = _validator.ValidateEventsCommon(messageEvents, _validationSettings.Where(x => x.Id == settingId).ToList());

            //Assert
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual("Ice Hockey", result[0].Result.SportName);
            Assert.AreEqual("United States", result[0].Result.LocationName);
            Assert.AreEqual("NHL", result[0].Result.LeagueName);
            Assert.AreEqual("Null vs Null", result[0].Result.EventName);
            Assert.AreEqual("Outright Winner", result[0].Result.Market);
            Assert.AreEqual("Bet365", result[0].Result.Provider);
            Assert.AreEqual("Bookmaker@id=\"8\"@name=\"Bet365\"|Odds@id=\"15196785512239081\"@currentPrice=\"21\"|Odds@id=\"1582397982239081\"@currentPrice=\"2501\"|Odds@id=\"5557060662239081\"@currentPrice=\"41\"|Odds@id=\"16776139852239081\"@currentPrice=\"76\"|Odds@id=\"20087219612239081\"@currentPrice=\"36\"|Odds@id=\"9977214392239081\"@currentPrice=\"76\"|Odds@id=\"13643160092239081\"@currentPrice=\"10.5\"|Odds@id=\"21191847802239081\"@currentPrice=\"6001\"|Odds@id=\"5268145052239081\"@currentPrice=\"8\"|Odds@id=\"15714598882239081\"@currentPrice=\"34\"|Odds@id=\"636129762239081\"@currentPrice=\"126\"|Odds@id=\"12676319862239081\"@currentPrice=\"21\"|Odds@id=\"15216844522239081\"@currentPrice=\"46\"|Odds@id=\"15130755032239081\"@currentPrice=\"29\"|Odds@id=\"21188434142239081\"@currentPrice=\"7\"|Odds@id=\"15245559822239081\"@currentPrice=\"8.5\"|Odds@id=\"2209372822239081\"@currentPrice=\"26\"|Odds@id=\"15418332072239081\"@currentPrice=\"201\"|Odds@id=\"16358310362239081\"@currentPrice=\"51\"|Odds@id=\"63722032239081\"@currentPrice=\"15\"|Odds@id=\"12439385392239081\"@currentPrice=\"26\"|Odds@id=\"12746472182239081\"@currentPrice=\"51\"|Odds@id=\"12700659412239081\"@currentPrice=\"8.5\"|Odds@id=\"11652901012239081\"@currentPrice=\"18\"|Odds@id=\"10252937382239081\"@currentPrice=\"31\"|Odds@id=\"4457921212239081\"@currentPrice=\"51\"|Odds@id=\"19139775692239081\"@currentPrice=\"31\"|Odds@id=\"3574962732239081\"@currentPrice=\"41\"|Odds@id=\"20297923752239081\"@currentPrice=\"7\"|Odds@id=\"8791917732239081\"@currentPrice=\"36\"|Bookmaker@id=\"8\"@name=\"Bet365\"", result[0].Result.PointsToHighline);

        }


        [Test]
        [TestCase(187)]
        public void ValidationSetting187_Negative1_ReturnsCorrectList(int settingId)
        {
            //Arrange
            var messageEvents = new List<Event>();
            InitializeEvents(settingId, "Negative#1", ref messageEvents);

            //Act
            var result = _validator.ValidateEventsCommon(messageEvents, _validationSettings.Where(x => x.Id == settingId).ToList());

            //Assert
            //Assert
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        [TestCase(188)]
        public void ValidationSetting188_Positive1_ReturnsCorrectList(int settingId)
        {
            //Arrange
            var messageEvents = new List<Event>();
            InitializeEvents(settingId, "Positive#1", ref messageEvents);

            //Act
            var result = _validator.ValidateEventsCommon(messageEvents, _validationSettings.Where(x => x.Id == settingId).ToList());

            //Assert
            Assert.AreEqual(4, result.Count);
            Assert.AreEqual("Ice Hockey", result[0].Result.SportName);
            Assert.AreEqual("Sweden", result[0].Result.LocationName);
            Assert.AreEqual("SHL", result[0].Result.LeagueName);
            Assert.AreEqual("HV 71 vs Rögle BK", result[0].Result.EventName);
            Assert.AreEqual("Under/Over 3rd Period - Home Team", result[0].Result.Market);
            Assert.AreEqual("Paf", result[0].Result.Provider);
            Assert.AreEqual("Odds@id=\"17370711502201148\"@currentPrice=\"1.39\"|Odds@id=\"9317396082201148\"@currentPrice=\"1.93\"|Odds@id=\"17370711172201148\"@currentPrice=\"3\"|Odds@id=\"6401746282201148\"@currentPrice=\"1.8\"|Odds@id=\"3961279012201148\"@currentPrice=\"1.34\"|Status@@Finished\n            ", result[0].Result.PointsToHighline);

        }

        [Test]
        [TestCase(189)]
        public void ValidationSetting189_Positive1_ReturnsCorrectList(int settingId)
        {
            //Arrange
            var messageEvents = new List<Event>();
            InitializeEvents(settingId, "Positive#1", ref messageEvents);

            //Act
            var result = _validator.ValidateEventsCommon(messageEvents, _validationSettings.Where(x => x.Id == settingId).ToList());

            //Assert
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("Ice Hockey", result[0].Result.SportName);
            Assert.AreEqual("Sweden", result[0].Result.LocationName);
            Assert.AreEqual("SHL", result[0].Result.LeagueName);
            Assert.AreEqual("HV 71 vs Rögle BK", result[0].Result.EventName);
            Assert.AreEqual("Under/Over 3rd Period - Home Team", result[0].Result.Market);
            Assert.AreEqual("Paf", result[0].Result.Provider);
            Assert.AreEqual("Odds@id=\"17370711502201148\"@startPrice=\"1.36\"|Odds@id=\"17370711172201148\"@startPrice=\"2.9\"|Odds@id=\"19420088662201148\"@startPrice=\"2.95\"|Odds@id=\"6401746282201148\"@startPrice=\"1.87\"|Odds@id=\"3961279012201148\"@startPrice=\"1.37\"|Bookmaker@id=\"46\"@name=\"Paf\"|Odds@id=\"17370711502201148\"@startPrice=\"1.36\"|Odds@id=\"9317396082201148\"@startPrice=\"3\"|Odds@id=\"17370711172201148\"@startPrice=\"2.9\"|Odds@id=\"19420088662201148\"@startPrice=\"2.95\"|Odds@id=\"6401746282201148\"@startPrice=\"1.87\"|Odds@id=\"3961279012201148\"@startPrice=\"1.37\"", result[0].Result.PointsToHighline);

        }


        [Test]
        [TestCase(195)]
        public void ValidationSetting195_Positive1_ReturnsCorrectList(int settingId)
        {
            //Arrange
            var messageEvents = new List<Event>();
            InitializeEvents(settingId, "Positive#1", ref messageEvents);

            //Act
            var result = _validator.ValidateEventsCommon(messageEvents, _validationSettings.Where(x => x.Id == settingId).ToList());

            //Assert
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("Greyhounds", result[0].Result.SportName);
            Assert.AreEqual("Australia", result[0].Result.LocationName);
            Assert.AreEqual("Warragul", result[0].Result.LeagueName);
            Assert.AreEqual("Warragul", result[0].Result.EventName);
            Assert.AreEqual("Race Winner", result[0].Result.Market);
            Assert.AreEqual("Bet365", result[0].Result.Provider);
            Assert.AreEqual("|Odds@id=\"202619622362351749\"@currentPriceProbability=\"20\"|Odds@id=\"68281856262351749\"@currentPriceProbability=\"5.263\"|Odds@id=\"57226434862351749\"@currentPriceProbability=\"7.692\"|Odds@id=\"70382836562351749\"@currentPriceProbability=\"14.286\"|Odds@id=\"180046630562351749\"@currentPriceProbability=\"14.286\"|Odds@id=\"27600503462351749\"@currentPriceProbability=\"12.5\"|Odds@id=\"110641838262351749\"@currentPriceProbability=\"27.778\"|Odds@id=\"127826613162351749\"@currentPriceProbability=\"18.182\"|Scores@@|Score@period=\"1P\"@totalScore=\"0\"|Score@period=\"2P\"@totalScore=\"2\"|Score@period=\"3P\"@totalScore=\"2\"|Score@period=\"FT\"@totalScore=\"4\"|Score@period=\"OT\"@totalScore=\"0\"|Score@period=\"P\"@totalScore=\"1\"|Participant@Number=\"6\"@Number=\"6\"|Participant@Number=\"4\"@Number=\"4\"|Participant@Number=\"1\"@Number=\"1\"|Participant@Number=\"8\"@Number=\"8\"|Participant@Number=\"2\"@Number=\"2\"|Participant@Number=\"3\"@Number=\"3\"|Participant@Number=\"5\"@Number=\"5\"|Participant@Number=\"7\"@Number=\"7\"|Participant@Number=\"9\"@Number=\"9\"|Participant@Number=\"10\"@Number=\"10\"", result[0].Result.PointsToHighline);

        }

        [Test]
        [TestCase(195)]
        public void ValidationSetting195_Negative1_ReturnsCorrectList(int settingId)
        {
            //Arrange
            var messageEvents = new List<Event>();
            InitializeEvents(settingId, "Negative#1", ref messageEvents);

            //Act
            var result = _validator.ValidateEventsCommon(messageEvents, _validationSettings.Where(x => x.Id == settingId).ToList());

            //Assert
            //Assert
            Assert.AreEqual(0, result.Count);
        }
        
        [Test]
        [TestCase(195)]
        public void ValidationSetting195_Negative2_ReturnsCorrectList(int settingId)
        {
            //Arrange
            var messageEvents = new List<Event>();
            InitializeEvents(settingId, "Negative#2", ref messageEvents);

            //Act
            var result = _validator.ValidateEventsCommon(messageEvents, _validationSettings.Where(x => x.Id == settingId).ToList());

            //Assert
            //Assert
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        [TestCase(195)]
        public void ValidationSetting195_Negative3_ReturnsCorrectList(int settingId)
        {
            //Arrange
            var messageEvents = new List<Event>();
            InitializeEvents(settingId, "Negative#3", ref messageEvents);

            //Act
            var result = _validator.ValidateEventsCommon(messageEvents, _validationSettings.Where(x => x.Id == settingId).ToList());

            //Assert
            //Assert
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        [TestCase(195)]
        public void ValidationSetting195_Negative4_ReturnsCorrectList(int settingId)
        {
            //Arrange
            var messageEvents = new List<Event>();
            InitializeEvents(settingId, "Negative#4", ref messageEvents);

            //Act
            var result = _validator.ValidateEventsCommon(messageEvents, _validationSettings.Where(x => x.Id == settingId).ToList());

            //Assert
            //Assert
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        [TestCase(170)]
        public void ValidationSetting170_Negative4_ReturnsCorrectList(int settingId)
        {
            //Arrange
            var messageEvents = new List<Event>();
            InitializeEvents(settingId, "Negative#1", ref messageEvents);

            //Act
            var result = _validator.ValidateEventsCommon(messageEvents, _validationSettings.Where(x => x.Id == settingId).ToList());

            //Assert
            //Assert
            Assert.AreEqual(0, result.Count);
        }
    }
}
