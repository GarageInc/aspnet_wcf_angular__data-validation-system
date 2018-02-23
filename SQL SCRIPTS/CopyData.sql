LOCK TABLES `Customers` WRITE;
/*!40000 ALTER TABLE `Customers` DISABLE KEYS */;
INSERT INTO `Customers` VALUES (-1,'Unknown',1,'',NULL,NULL,'',NULL,NULL,'',0,NULL,1,NULL,NULL,'\0',NULL,'2016-08-25 08:01:34',0,NULL,NULL),(1,'Gazprom',1,'Spb',NULL,NULL,'gazprom.ru',NULL,NULL,'',0,NULL,1,NULL,NULL,'',NULL,'2016-09-14 07:12:03',0,NULL,NULL);
/*!40000 ALTER TABLE `Customers` ENABLE KEYS */;
UNLOCK TABLES;


--
-- Dumping data for table `tic_CustomFieldTypes`
--

LOCK TABLES `tic_CustomFieldTypes` WRITE;
/*!40000 ALTER TABLE `tic_CustomFieldTypes` DISABLE KEYS */;
INSERT INTO `tic_CustomFieldTypes` VALUES (1,'Text'),(2,'Long Text'),(3,'Dropdown'),(4,'Related Dropdowns'),(5,'Checkbox'),(6,'Date'),(7,'Attachments'),(8,'Numeric'),(9,'Email'),(10,'Multiple Selection');
/*!40000 ALTER TABLE `tic_CustomFieldTypes` ENABLE KEYS */;
UNLOCK TABLES;


--
-- Dumping data for table `tic_CustomFields`
--

LOCK TABLES `tic_CustomFields` WRITE;
/*!40000 ALTER TABLE `tic_CustomFields` DISABLE KEYS */;
INSERT INTO `tic_CustomFields` VALUES (-2,'Attachments From Email','Attachments','AttchamentsFromEmail',NULL,7,NULL,NULL,NULL,NULL,NULL,NULL,1,'2016-09-07 11:04:25','Admin',NULL,NULL,NULL),(-1,'Description From Email','Description','DescritionFromEmail',NULL,2,NULL,NULL,NULL,NULL,NULL,NULL,1,'2016-09-07 11:04:25','Admin','2016-09-14 15:58:12','Admin',NULL);
/*!40000 ALTER TABLE `tic_CustomFields` ENABLE KEYS */;
UNLOCK TABLES;


--
-- Dumping data for table `tic_DepartmentRoles`
--

LOCK TABLES `tic_DepartmentRoles` WRITE;
/*!40000 ALTER TABLE `tic_DepartmentRoles` DISABLE KEYS */;
INSERT INTO `tic_DepartmentRoles` VALUES ('DM','Department Manager'),('S','Staff');
/*!40000 ALTER TABLE `tic_DepartmentRoles` ENABLE KEYS */;
UNLOCK TABLES;


--
-- Dumping data for table `tic_EmailActions`
--

LOCK TABLES `tic_EmailActions` WRITE;
/*!40000 ALTER TABLE `tic_EmailActions` DISABLE KEYS */;
INSERT INTO `tic_EmailActions` VALUES ('AC','Assignee Changed'),('CA','Comment Added'),('RA','Reply Added'),('SC','Status Changed'),('TC','Ticket Created'),('TE','Ticket Edited');
/*!40000 ALTER TABLE `tic_EmailActions` ENABLE KEYS */;
UNLOCK TABLES;


--
-- Dumping data for table `tic_EmailSendTo`
--

LOCK TABLES `tic_EmailSendTo` WRITE;
/*!40000 ALTER TABLE `tic_EmailSendTo` DISABLE KEYS */;
INSERT INTO `tic_EmailSendTo` VALUES (1,'Customer'),(2,'Staff'),(3,'Department Manager'),(4,'Adminstrator'),(5,'All');
/*!40000 ALTER TABLE `tic_EmailSendTo` ENABLE KEYS */;
UNLOCK TABLES;



--
-- Dumping data for table `tic_EntryTypes`
--

LOCK TABLES `tic_EntryTypes` WRITE;
/*!40000 ALTER TABLE `tic_EntryTypes` DISABLE KEYS */;
INSERT INTO `tic_EntryTypes` VALUES (1,'Assignee Changed'),(2,'Comment Added'),(3,'Reply Added By Customer'),(4,'Status Changed'),(5,'Ticket Created'),(6,'Ticket Edited'),(7,'Reply Added By Staff');
/*!40000 ALTER TABLE `tic_EntryTypes` ENABLE KEYS */;
UNLOCK TABLES;




--
-- Dumping data for table `roles`
--

LOCK TABLES `roles` WRITE;
/*!40000 ALTER TABLE `roles` DISABLE KEYS */;
INSERT INTO `roles` VALUES ('17b4aa8a-0fc7-43cb-a230-d123ba1b1162','Admin'),('2849b091-2bb7-4f3c-9426-be92cbb1dbec','CustomerContact'),('bf3b24d0-2997-43ca-85aa-003789e3003a','Staff');
/*!40000 ALTER TABLE `roles` ENABLE KEYS */;
UNLOCK TABLES;




--
-- Dumping data for table `users`
--

LOCK TABLES `users` WRITE;
/*!40000 ALTER TABLE `users` DISABLE KEYS */;
INSERT INTO `users` VALUES ('ea5ad17f-4262-4de6-b1ba-6f09faa54b6d','admin@ambaha.com',0,'AD5GUT55uJCfii+HAA0ZNvyxsZE72Rp0YWyjoM4HGDKkspeQrGT09SeI5Nl+NbUV3A==','9387905e-5f3f-4c58-bd1b-f3fa92a28315',NULL,0,0,'2016-08-17 11:30:32',0,0,'admin@ambaha.com','Tic','Admin');
/*!40000 ALTER TABLE `users` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;



--
-- Dumping data for table `userroles`
--

LOCK TABLES `userroles` WRITE;
/*!40000 ALTER TABLE `userroles` DISABLE KEYS */;
INSERT INTO `userroles` VALUES ('ea5ad17f-4262-4de6-b1ba-6f09faa54b6d','17b4aa8a-0fc7-43cb-a230-d123ba1b1162');
/*!40000 ALTER TABLE `userroles` ENABLE KEYS */;
UNLOCK TABLES
