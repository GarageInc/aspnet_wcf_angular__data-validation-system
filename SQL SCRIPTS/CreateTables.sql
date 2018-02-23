CREATE DATABASE  IF NOT EXISTS `gb_ts_staging` /*!40100 DEFAULT CHARACTER SET cp1251 */;
USE `gb_ts_staging`;
-- MySQL dump 10.13  Distrib 5.7.12, for Win64 (x86_64)
--
-- Host: mysql86.1gb.ru    Database: gb_ts_staging
-- ------------------------------------------------------
-- Server version	5.5.35-rel33.0-log

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `CustomerContacts`
--

DROP TABLE IF EXISTS `CustomerContacts`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `CustomerContacts` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `CustomerId` int(11) NOT NULL,
  `FirstName` varchar(255) NOT NULL,
  `LastName` varchar(255) NOT NULL,
  `EMail` varchar(255) NOT NULL,
  `Skype` varchar(255) NOT NULL,
  `Phones` varchar(500) NOT NULL,
  `Priority` int(11) NOT NULL DEFAULT '1',
  `UserId` varchar(128) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=cp1251;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `Customers`
--

DROP TABLE IF EXISTS `Customers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `Customers` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Company` varchar(255) NOT NULL DEFAULT '',
  `CountryId` int(11) DEFAULT '-1',
  `City` varchar(255) DEFAULT NULL,
  `PostalIndex` varchar(6) DEFAULT NULL,
  `PostalAddress` varchar(500) DEFAULT NULL,
  `WebSite` varchar(255) NOT NULL DEFAULT '',
  `Phones` varchar(255) DEFAULT NULL,
  `CreateDate` datetime DEFAULT NULL,
  `IsActive` bit(1) NOT NULL,
  `StatusId` int(11) NOT NULL,
  `EMail` varchar(255) DEFAULT NULL,
  `ClassificationId` int(11) NOT NULL DEFAULT '1',
  `SubscriptionStartDate` datetime DEFAULT NULL,
  `SubscriptionEndDate` datetime DEFAULT NULL,
  `IsDeposit` bit(1) NOT NULL DEFAULT b'0',
  `Notes` text,
  `LastUpdate` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `PaymentPeriod` int(11) NOT NULL DEFAULT '0',
  `BookieId` int(11) DEFAULT NULL,
  `PaidTillDate` datetime DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=cp1251;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `TicketFields`
--

DROP TABLE IF EXISTS `TicketFields`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `TicketFields` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `TicketId` int(11) DEFAULT NULL,
  `Field` varchar(255) DEFAULT NULL,
  `Value` mediumtext CHARACTER SET utf8,
  `CustomFieldId` int(11) DEFAULT NULL,
  `TextValue` mediumtext,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=301 DEFAULT CHARSET=cp1251;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `Tickets`
--

DROP TABLE IF EXISTS `Tickets`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `Tickets` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `CreationDate` datetime NOT NULL,
  `LastUpdate` datetime NOT NULL,
  `StatusId` int(11) DEFAULT NULL,
  `CustomerId` int(11) NOT NULL,
  `CustomerUserId` int(11) NOT NULL,
  `PriorityId` int(11) NOT NULL,
  `TypeId` int(11) NOT NULL,
  `ProductId` int(11) NOT NULL,
  `IssueTypeId` int(11) NOT NULL,
  `DepartmentId` int(11) DEFAULT NULL,
  `OwnerId` int(11) DEFAULT NULL,
  `ProductCategoryId` int(11) NOT NULL,
  `PackageId` varchar(64) NOT NULL DEFAULT '1',
  `AssignedTo` varchar(128) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_Tickets_TicketTypes_idx` (`TypeId`),
  KEY `FK_Tickets_IssueTypes_idx` (`IssueTypeId`),
  CONSTRAINT `FK_Tickets_IssueTypes` FOREIGN KEY (`IssueTypeId`) REFERENCES `tic_IssueTypes` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `FK_Tickets_TicketTypes` FOREIGN KEY (`TypeId`) REFERENCES `tic_TicketTypes` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=325 DEFAULT CHARSET=cp1251;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `TicketsLogs`
--

DROP TABLE IF EXISTS `TicketsLogs`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `TicketsLogs` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `TicketId` int(11) NOT NULL,
  `ActorId` int(11) DEFAULT NULL,
  `EntryTypeId` int(11) NOT NULL,
  `EntryValue` mediumtext CHARACTER SET utf8,
  `CreationDate` datetime DEFAULT NULL,
  `EntryExtendedValue` mediumtext,
  `ActorUserId` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_TicketsLogs_Tickets_idx` (`TicketId`),
  CONSTRAINT `FK_TicketsLogs_Tickets` FOREIGN KEY (`TicketId`) REFERENCES `Tickets` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=330 DEFAULT CHARSET=cp1251;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `roles`
--

DROP TABLE IF EXISTS `roles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `roles` (
  `Id` varchar(128) NOT NULL,
  `Name` varchar(256) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=cp1251;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tic_CustomFieldTypes`
--

DROP TABLE IF EXISTS `tic_CustomFieldTypes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tic_CustomFieldTypes` (
  `Id` int(11) NOT NULL,
  `Name` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=cp1251;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tic_CustomFields`
--

DROP TABLE IF EXISTS `tic_CustomFields`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tic_CustomFields` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) NOT NULL,
  `Title` varchar(200) NOT NULL,
  `Identifier` varchar(200) DEFAULT NULL COMMENT 'Identifier is used for Email Templates',
  `PlaceholderText` varchar(200) DEFAULT NULL,
  `CustomeFieldTypeId` int(11) NOT NULL,
  `DbTableName` varchar(200) DEFAULT NULL COMMENT 'Used in case of dropdowns. It stores name of db table where we should take dropdown values from',
  `DbTableIdFieldName` varchar(200) DEFAULT NULL COMMENT 'Used in case of dropdowns. It stores Field Name of Id Field',
  `DbTableTextFieldName` varchar(200) DEFAULT NULL COMMENT 'Used in case of dropdowns. It stores name of Text field Field Name',
  `DbFilterFieldName` varchar(200) DEFAULT NULL COMMENT 'It''s used in case of related dropdowns to filter records on the next steps',
  `StepNumber` int(11) DEFAULT NULL COMMENT 'It''s used in case of related dropdowns',
  `RootCustomFieldId` int(11) DEFAULT NULL COMMENT 'It''s used in case of related dropdowns to identify the root custom field. It''s NULL in case of not related dropdowns',
  `IsActive` tinyint(1) NOT NULL DEFAULT '1',
  `CreationDate` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `CreatedBy` varchar(200) NOT NULL,
  `LastUpdate` timestamp NULL DEFAULT NULL,
  `UpdatedBy` varchar(200) DEFAULT NULL,
  `DropdownCustomFieldId` int(11) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_CustomeFields_CustomFieldTypes_idx` (`CustomeFieldTypeId`),
  CONSTRAINT `FK_CustomeFields_CustomFieldTypes` FOREIGN KEY (`CustomeFieldTypeId`) REFERENCES `tic_CustomFieldTypes` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=139 DEFAULT CHARSET=cp1251;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tic_DepartmentRoles`
--

DROP TABLE IF EXISTS `tic_DepartmentRoles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tic_DepartmentRoles` (
  `Id` varchar(5) NOT NULL,
  `RoleName` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=cp1251 COMMENT='It sores available deprtment roles such as Department Manager and Staff';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tic_Departments`
--

DROP TABLE IF EXISTS `tic_Departments`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tic_Departments` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) NOT NULL,
  `CanSeeCustomerDetails` tinyint(1) NOT NULL DEFAULT '0' COMMENT 'The field that identifies if staff/department manager can see customer details',
  `IsActive` tinyint(1) NOT NULL DEFAULT '1',
  `CreationDate` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `CreatedBy` varchar(200) NOT NULL,
  `LastUpdate` timestamp NULL DEFAULT NULL,
  `UpdatedBy` varchar(200) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=32 DEFAULT CHARSET=cp1251 COMMENT='The database table to store information about departments';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tic_EmailActions`
--

DROP TABLE IF EXISTS `tic_EmailActions`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tic_EmailActions` (
  `Key` varchar(5) NOT NULL,
  `Name` varchar(100) NOT NULL,
  PRIMARY KEY (`Key`)
) ENGINE=InnoDB DEFAULT CHARSET=cp1251 COMMENT='It contains different email actions/events such as Ticket created, Reply added, etc';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tic_EmailSendTo`
--

DROP TABLE IF EXISTS `tic_EmailSendTo`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tic_EmailSendTo` (
  `Id` int(11) NOT NULL,
  `SendTo` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=cp1251 COMMENT='It stores categories of users which will be notified by email such as Staff, Customers, etc';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tic_EmailTemplate`
--

DROP TABLE IF EXISTS `tic_EmailTemplate`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tic_EmailTemplate` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) NOT NULL,
  `EmailTemplate` text,
  `EmailActionKey` varchar(5) DEFAULT NULL,
  `EmailSubject` varchar(450) DEFAULT NULL,
  `IsActive` tinyint(1) NOT NULL DEFAULT '1',
  `CreationDate` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `CreatedBy` varchar(200) NOT NULL,
  `LastUpdate` timestamp NULL DEFAULT NULL,
  `UpdatedBy` varchar(200) DEFAULT NULL,
  `EmailTemplatePreview` longtext,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=14 DEFAULT CHARSET=cp1251 COMMENT='Table that contains email templates';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tic_EmailTemplateSendTo`
--

DROP TABLE IF EXISTS `tic_EmailTemplateSendTo`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tic_EmailTemplateSendTo` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `EmailSendToId` int(11) NOT NULL,
  `EmailTemplateId` int(11) NOT NULL,
  `IsActive` tinyint(1) NOT NULL DEFAULT '1',
  `CreationDate` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `CreatedBy` varchar(200) NOT NULL,
  `LastUpdate` timestamp NULL DEFAULT NULL,
  `UpdatedBy` varchar(200) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_EmailTemplateSendTo_EmailTemplate_idx` (`EmailTemplateId`),
  KEY `FK_EmailTemplatesSendTo_EmailSendTo_idx` (`EmailSendToId`),
  CONSTRAINT `FK_EmailTemplateSendTo_EmailTemplate` FOREIGN KEY (`EmailTemplateId`) REFERENCES `tic_EmailTemplate` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `FK_EmailTemplatesSendTo_EmailSendTo` FOREIGN KEY (`EmailSendToId`) REFERENCES `tic_EmailSendTo` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=29 DEFAULT CHARSET=cp1251;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tic_EntryTypes`
--

DROP TABLE IF EXISTS `tic_EntryTypes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tic_EntryTypes` (
  `Id` int(11) NOT NULL,
  `Name` varchar(45) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=cp1251;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tic_Files`
--

DROP TABLE IF EXISTS `tic_Files`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tic_Files` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) NOT NULL,
  `Path` varchar(450) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=113 DEFAULT CHARSET=cp1251;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tic_FormTemplateCustomFields`
--

DROP TABLE IF EXISTS `tic_FormTemplateCustomFields`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tic_FormTemplateCustomFields` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `CustomFieldId` int(11) NOT NULL,
  `SortOrder` int(11) NOT NULL,
  `FormTemplateId` int(11) NOT NULL,
  `IsActive` tinyint(1) NOT NULL DEFAULT '1',
  `CreationDate` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `CreatedBy` varchar(200) NOT NULL,
  `LastUpdate` timestamp NULL DEFAULT NULL,
  `UpdatedBy` varchar(200) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_FormTemplateCustomFields_CustomFields_idx` (`CustomFieldId`),
  KEY `FK_FormTemplateCustomeFields_FormTemplates_idx` (`FormTemplateId`),
  CONSTRAINT `FK_FormTemplateCustomeFields_FormTemplates` FOREIGN KEY (`FormTemplateId`) REFERENCES `tic_FormTemplates` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `FK_FormTemplateCustomFields_CustomFields` FOREIGN KEY (`CustomFieldId`) REFERENCES `tic_CustomFields` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=41 DEFAULT CHARSET=cp1251;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tic_FormTemplates`
--

DROP TABLE IF EXISTS `tic_FormTemplates`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tic_FormTemplates` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) NOT NULL,
  `TicketTypeId` int(11) DEFAULT NULL,
  `ProductId` int(11) DEFAULT NULL,
  `ProductCategoryId` int(11) DEFAULT NULL,
  `IssueTypeId` int(11) DEFAULT NULL,
  `IsActive` tinyint(1) NOT NULL DEFAULT '1',
  `CreationDate` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `CreatedBy` varchar(200) NOT NULL,
  `LastUpdate` timestamp NULL DEFAULT NULL,
  `UpdatedBy` varchar(200) DEFAULT NULL,
  `SortOrder` int(11) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_FormTemplates_IssueTypes_idx` (`IssueTypeId`),
  KEY `FK_FormTemplates_TicketTypes_idx` (`TicketTypeId`),
  KEY `FK_FormTemplates_Products_idx` (`ProductId`),
  KEY `FK_FormTemplates_ProductCategories_idx` (`ProductCategoryId`),
  CONSTRAINT `FK_FormTemplates_IssueTypes` FOREIGN KEY (`IssueTypeId`) REFERENCES `tic_IssueTypes` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `FK_FormTemplates_ProductCategories` FOREIGN KEY (`ProductCategoryId`) REFERENCES `tic_ProductCategories` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `FK_FormTemplates_Products` FOREIGN KEY (`ProductId`) REFERENCES `tic_Products` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `FK_FormTemplates_TicketTypes` FOREIGN KEY (`TicketTypeId`) REFERENCES `tic_TicketTypes` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=26 DEFAULT CHARSET=cp1251;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tic_Icons`
--

DROP TABLE IF EXISTS `tic_Icons`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tic_Icons` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Icon` varchar(100) NOT NULL,
  `Name` varchar(100) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=cp1251;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tic_IssueTypes`
--

DROP TABLE IF EXISTS `tic_IssueTypes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tic_IssueTypes` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) NOT NULL,
  `IconId` int(11) NOT NULL,
  `IsActive` tinyint(1) NOT NULL DEFAULT '1',
  `CreationDate` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `CreatedBy` varchar(200) NOT NULL,
  `LastUpdate` timestamp NULL DEFAULT NULL,
  `UpdatedBy` varchar(200) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_IssueTypes_Icons_idx` (`IconId`),
  CONSTRAINT `FK_IssueTypes_Icons` FOREIGN KEY (`IconId`) REFERENCES `tic_Icons` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=79 DEFAULT CHARSET=cp1251;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tic_ProductCategories`
--

DROP TABLE IF EXISTS `tic_ProductCategories`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tic_ProductCategories` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) NOT NULL,
  `IconId` int(11) NOT NULL,
  `IsActive` tinyint(1) NOT NULL DEFAULT '1',
  `CreationDate` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `CreatedBy` varchar(200) NOT NULL,
  `LastUpdate` timestamp NULL DEFAULT NULL,
  `UpdatedBy` varchar(200) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_ProductCategories_Icons_idx` (`IconId`),
  CONSTRAINT `FK_ProductCategories_Icons` FOREIGN KEY (`IconId`) REFERENCES `tic_Icons` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=70 DEFAULT CHARSET=cp1251;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tic_Products`
--

DROP TABLE IF EXISTS `tic_Products`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tic_Products` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) NOT NULL,
  `IconId` int(11) NOT NULL,
  `IsActive` tinyint(1) NOT NULL DEFAULT '1',
  `CreationDate` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `CreatedBy` varchar(200) NOT NULL,
  `LastUpdate` timestamp NULL DEFAULT NULL,
  `UpdatedBy` varchar(200) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_Products_Icons_idx` (`IconId`),
  CONSTRAINT `FK_Products_Icons` FOREIGN KEY (`IconId`) REFERENCES `tic_Icons` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=70 DEFAULT CHARSET=cp1251;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tic_TicketDefault`
--

DROP TABLE IF EXISTS `tic_TicketDefault`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tic_TicketDefault` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `PriorityId` int(11) NOT NULL,
  `TypeId` int(11) NOT NULL,
  `ProductId` int(11) NOT NULL,
  `IssueTypeId` int(11) NOT NULL,
  `ProductCategoryId` int(11) NOT NULL,
  `PackageId` varchar(64) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=cp1251 COMMENT='This table contains default values for the ticket created by email';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tic_TicketStatuses`
--

DROP TABLE IF EXISTS `tic_TicketStatuses`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tic_TicketStatuses` (
  `Id` int(11) NOT NULL,
  `Category` varchar(45) DEFAULT NULL,
  `Name` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=cp1251;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tic_TicketTypes`
--

DROP TABLE IF EXISTS `tic_TicketTypes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tic_TicketTypes` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) NOT NULL,
  `IconId` int(11) NOT NULL,
  `IsActive` tinyint(1) NOT NULL DEFAULT '1',
  `CreationDate` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `CreatedBy` varchar(200) NOT NULL,
  `LastUpdate` timestamp NULL DEFAULT NULL,
  `UpdatedBy` varchar(200) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_TicketTypes_Icons_idx` (`IconId`),
  CONSTRAINT `FK_TicketTypes_Icons` FOREIGN KEY (`IconId`) REFERENCES `tic_Icons` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=20 DEFAULT CHARSET=cp1251;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tic_TicketWorkflow`
--

DROP TABLE IF EXISTS `tic_TicketWorkflow`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tic_TicketWorkflow` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `CustomerPriorityId` int(11) DEFAULT NULL,
  `TicketTypeId` int(11) DEFAULT NULL,
  `IssueTypeId` int(11) DEFAULT NULL,
  `ProductId` int(11) DEFAULT NULL,
  `ProductCategoryId` int(11) DEFAULT NULL,
  `DepartmentId` int(11) NOT NULL,
  `SortOrder` int(11) DEFAULT NULL,
  `IsActive` tinyint(1) NOT NULL DEFAULT '1',
  `CreationDate` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `CreatedBy` varchar(200) NOT NULL,
  `LastUpdate` timestamp NULL DEFAULT NULL,
  `UpdatedBy` varchar(200) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_TicketWorkflow_Products_idx` (`ProductId`),
  KEY `FK_TicketWorkflow_TicketTypes_idx` (`TicketTypeId`),
  KEY `FK_TicketWorkflow_IssueTypes_idx` (`IssueTypeId`),
  KEY `FK_TicketWorkflow_ProductCategories_idx` (`ProductCategoryId`),
  KEY `FK_TicketWorkflow_Departments_idx` (`DepartmentId`),
  CONSTRAINT `FK_TicketWorkflow_Departments` FOREIGN KEY (`DepartmentId`) REFERENCES `tic_Departments` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `FK_TicketWorkflow_IssueTypes` FOREIGN KEY (`IssueTypeId`) REFERENCES `tic_IssueTypes` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `FK_TicketWorkflow_ProductCategories` FOREIGN KEY (`ProductCategoryId`) REFERENCES `tic_ProductCategories` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `FK_TicketWorkflow_Products` FOREIGN KEY (`ProductId`) REFERENCES `tic_Products` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `FK_TicketWorkflow_TicketTypes` FOREIGN KEY (`TicketTypeId`) REFERENCES `tic_TicketTypes` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=18 DEFAULT CHARSET=cp1251;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tic_UserDepartments`
--

DROP TABLE IF EXISTS `tic_UserDepartments`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tic_UserDepartments` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `DepartmentId` int(11) DEFAULT NULL,
  `UserId` varchar(128) DEFAULT NULL,
  `DepartmentRoleId` varchar(5) DEFAULT NULL,
  `IsActive` tinyint(1) NOT NULL DEFAULT '1',
  `CreationDate` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `CreatedBy` varchar(200) NOT NULL,
  `LastUpdate` timestamp NULL DEFAULT NULL,
  `UpdatedBy` varchar(200) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_UserDepartments_DepartmentRoles_idx` (`DepartmentRoleId`),
  KEY `FK_UserDepartments_Departments_idx` (`DepartmentId`),
  KEY `FK_UserDepartments_Users_idx` (`UserId`),
  CONSTRAINT `FK_UserDepartments_DepartmentRoles` FOREIGN KEY (`DepartmentRoleId`) REFERENCES `tic_DepartmentRoles` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `FK_UserDepartments_Departments` FOREIGN KEY (`DepartmentId`) REFERENCES `tic_Departments` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `FK_UserDepartments_Users` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=115 DEFAULT CHARSET=cp1251 COMMENT='It stores information about User departments assignment and role of the user which can be department manager or staff';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `userclaims`
--

DROP TABLE IF EXISTS `userclaims`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `userclaims` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `UserId` varchar(128) NOT NULL,
  `ClaimType` longtext,
  `ClaimValue` longtext,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `Id` (`Id`),
  KEY `UserId` (`UserId`),
  CONSTRAINT `ApplicationUser_Claims` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`) ON DELETE CASCADE ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=cp1251;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `userlogins`
--

DROP TABLE IF EXISTS `userlogins`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `userlogins` (
  `LoginProvider` varchar(128) NOT NULL,
  `ProviderKey` varchar(128) NOT NULL,
  `UserId` varchar(128) NOT NULL,
  PRIMARY KEY (`LoginProvider`,`ProviderKey`,`UserId`),
  KEY `ApplicationUser_Logins` (`UserId`),
  CONSTRAINT `ApplicationUser_Logins` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`) ON DELETE CASCADE ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=cp1251;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `userroles`
--

DROP TABLE IF EXISTS `userroles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `userroles` (
  `UserId` varchar(128) NOT NULL,
  `RoleId` varchar(128) NOT NULL,
  PRIMARY KEY (`UserId`,`RoleId`),
  KEY `IdentityRole_Users` (`RoleId`),
  CONSTRAINT `ApplicationUser_Roles` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`) ON DELETE CASCADE ON UPDATE NO ACTION,
  CONSTRAINT `IdentityRole_Users` FOREIGN KEY (`RoleId`) REFERENCES `roles` (`Id`) ON DELETE CASCADE ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=cp1251;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `users`
--

DROP TABLE IF EXISTS `users`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `users` (
  `Id` varchar(128) NOT NULL,
  `Email` varchar(256) DEFAULT NULL,
  `EmailConfirmed` tinyint(1) NOT NULL,
  `PasswordHash` longtext,
  `SecurityStamp` longtext,
  `PhoneNumber` longtext,
  `PhoneNumberConfirmed` tinyint(1) NOT NULL,
  `TwoFactorEnabled` tinyint(1) NOT NULL,
  `LockoutEndDateUtc` datetime DEFAULT NULL,
  `LockoutEnabled` tinyint(1) NOT NULL,
  `AccessFailedCount` int(11) NOT NULL,
  `UserName` varchar(256) NOT NULL,
  `FirstName` varchar(200) DEFAULT NULL,
  `LastName` varchar(200) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=cp1251;
/*!40101 SET character_set_client = @saved_cs_client */;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2016-09-17 15:12:10
