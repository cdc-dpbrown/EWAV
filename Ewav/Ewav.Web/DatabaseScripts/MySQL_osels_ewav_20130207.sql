CREATE DATABASE  IF NOT EXISTS `osels_ewav` /*!40100 DEFAULT CHARACTER SET utf8 */;
USE `osels_ewav`;
-- MySQL dump 10.13  Distrib 5.5.16, for Win32 (x86)
--
-- Host: localhost    Database: osels_ewav
-- ------------------------------------------------------
-- Server version	5.5.27

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
-- Table structure for table `securitylevel`
--

DROP TABLE IF EXISTS `securitylevel`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `securitylevel` (
  `SecurityLevelID` int(11) NOT NULL AUTO_INCREMENT,
  `SecurityLevelValue` int(11) NOT NULL,
  `SecurityLevelDescription` varchar(50) NOT NULL,
  PRIMARY KEY (`SecurityLevelID`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `canvas`
--

DROP TABLE IF EXISTS `canvas`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `canvas` (
  `CanvasID` int(11) NOT NULL AUTO_INCREMENT,
  `CanvasName` varchar(50) NOT NULL,
  `UserID` int(11) NOT NULL,
  `CanvasDescription` longtext,
  `CreatedDate` datetime NOT NULL,
  `ModifiedDate` datetime NOT NULL,
  `DatasourceID` int(64) NOT NULL,
  `CanvasContent` longtext NOT NULL,
  PRIMARY KEY (`CanvasID`,`UserID`,`DatasourceID`),
  KEY `fk_canvas_user1` (`UserID`),
  KEY `fk_canvas_datasource1` (`DatasourceID`),
  CONSTRAINT `fk_canvas_user1` FOREIGN KEY (`UserID`) REFERENCES `user` (`UserID`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Temporary table structure for view `vwcanvasshare`
--

DROP TABLE IF EXISTS `vwcanvasshare`;
/*!50001 DROP VIEW IF EXISTS `vwcanvasshare`*/;
SET @saved_cs_client     = @@character_set_client;
SET character_set_client = utf8;
/*!50001 CREATE TABLE `vwcanvasshare` (
  `UserName` varchar(50),
  `DatasourceName` varchar(50),
  `CanvasID` int(11),
  `CanvasName` varchar(50),
  `CanvasDescription` longtext,
  `CreatedDate` datetime,
  `ModifiedDate` datetime,
  `CanvasContent` longtext,
  `UserID` int(11),
  `OrganizationID` int(11),
  `FirstName` varchar(50),
  `LastName` varchar(50),
  `DatasourceID` int(11)
) ENGINE=MyISAM */;
SET character_set_client = @saved_cs_client;

--
-- Temporary table structure for view `vwcanvasuser`
--

DROP TABLE IF EXISTS `vwcanvasuser`;
/*!50001 DROP VIEW IF EXISTS `vwcanvasuser`*/;
SET @saved_cs_client     = @@character_set_client;
SET character_set_client = utf8;
/*!50001 CREATE TABLE `vwcanvasuser` (
  `CanvasID` int(11),
  `CanvasName` varchar(50),
  `CanvasDescription` longtext,
  `CreatedDate` datetime,
  `ModifiedDate` datetime,
  `CanvasContent` longtext,
  `UserID` int(11),
  `UserName` varchar(50),
  `FirstName` varchar(50),
  `LastName` varchar(50),
  `OrganizationID` int(11),
  `DatasourceID` int(11),
  `DatasourceName` varchar(50)
) ENGINE=MyISAM */;
SET character_set_client = @saved_cs_client;

--
-- Table structure for table `datasourceuser`
--

DROP TABLE IF EXISTS `datasourceuser`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `datasourceuser` (
  `DatasourceID` int(11) NOT NULL,
  `UserID` int(11) NOT NULL,
  PRIMARY KEY (`DatasourceID`,`UserID`),
  KEY `fk_datasourceuser_user1` (`UserID`),
  KEY `fk_datasourceuser_datasource1` (`DatasourceID`),
  CONSTRAINT `fk_datasourceuser_user1` FOREIGN KEY (`UserID`) REFERENCES `user` (`UserID`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_datasourceuser_datasource1` FOREIGN KEY (`DatasourceID`) REFERENCES `datasource` (`DatasourceID`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `datasource`
--

DROP TABLE IF EXISTS `datasource`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `datasource` (
  `DatasourceID` int(11) NOT NULL AUTO_INCREMENT,
  `DatasourceName` varchar(50) NOT NULL,
  `OrganizationID` int(11) NOT NULL,
  `DatasourceServerName` varchar(100) NOT NULL,
  `DatabaseType` varchar(50) NOT NULL,
  `InitialCatalog` varchar(50) DEFAULT NULL,
  `PersistSecurityInfo` varchar(50) DEFAULT NULL,
  `DatabaseUserID` varchar(50) DEFAULT NULL,
  `Password` varchar(50) NOT NULL,
  `TableName` longtext NOT NULL,
  `SQLQuery` bit(1) DEFAULT NULL,
  `SQLText` longtext,
  `active` bit(1) NOT NULL,
  PRIMARY KEY (`DatasourceID`,`OrganizationID`),
  KEY `fk_datasource_organization1` (`OrganizationID`),
  CONSTRAINT `fk_datasource_organization1` FOREIGN KEY (`OrganizationID`) REFERENCES `organization` (`OrganizationID`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `sharedcanvases`
--

DROP TABLE IF EXISTS `sharedcanvases`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `sharedcanvases` (
  `CanvasID` int(11) NOT NULL,
  `UserID` int(11) NOT NULL,
  PRIMARY KEY (`CanvasID`,`UserID`),
  KEY `fk_sharedcanvases_canvas` (`CanvasID`),
  KEY `fk_sharedcanvases_user1` (`UserID`),
  CONSTRAINT `fk_sharedcanvases_canvas` FOREIGN KEY (`CanvasID`) REFERENCES `canvas` (`CanvasID`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_sharedcanvases_user1` FOREIGN KEY (`UserID`) REFERENCES `user` (`UserID`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `organization`
--

DROP TABLE IF EXISTS `organization`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `organization` (
  `OrganizationID` int(11) NOT NULL AUTO_INCREMENT,
  `OrganizationName` varchar(50) NOT NULL,
  `Description` varchar(50) DEFAULT NULL,
  PRIMARY KEY (`OrganizationID`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Temporary table structure for view `vwuserdatasource`
--

DROP TABLE IF EXISTS `vwuserdatasource`;
/*!50001 DROP VIEW IF EXISTS `vwuserdatasource`*/;
SET @saved_cs_client     = @@character_set_client;
SET character_set_client = utf8;
/*!50001 CREATE TABLE `vwuserdatasource` (
  `UserID` int(11),
  `UserName` varchar(50),
  `OrganizationID` int(11),
  `PasswordHash` longtext,
  `SecurityLevelValue` int(11),
  `SecurityLevelID` int(11),
  `SecurityLevelDescription` varchar(50),
  `DatasourceID` int(11),
  `DatasourceName` varchar(50),
  `DatasourceServerName` varchar(100),
  `DatabaseType` varchar(50),
  `InitialCatalog` varchar(50),
  `PersistSecurityInfo` varchar(50),
  `Password` varchar(50),
  `TableName` longtext,
  `SQLQuery` bit(1),
  `SQLText` longtext,
  `active` bit(1),
  `DatabaseUserID` varchar(50)
) ENGINE=MyISAM */;
SET character_set_client = @saved_cs_client;

--
-- Temporary table structure for view `vwcanvasuser1`
--

DROP TABLE IF EXISTS `vwcanvasuser1`;
/*!50001 DROP VIEW IF EXISTS `vwcanvasuser1`*/;
SET @saved_cs_client     = @@character_set_client;
SET character_set_client = utf8;
/*!50001 CREATE TABLE `vwcanvasuser1` (
  `UserName` varchar(50),
  `DatasourceName` varchar(50),
  `CanvasID` int(11),
  `CanvasName` varchar(50),
  `CanvasDescription` longtext,
  `CreatedDate` datetime,
  `ModifiedDate` datetime,
  `CanvasContent` longtext,
  `UserID` int(11),
  `OrganizationID` int(11),
  `FirstName` varchar(50),
  `LastName` varchar(50),
  `DatasourceID` int(11)
) ENGINE=MyISAM */;
SET character_set_client = @saved_cs_client;

--
-- Table structure for table `user`
--

DROP TABLE IF EXISTS `user`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `user` (
  `UserID` int(11) NOT NULL AUTO_INCREMENT,
  `UserName` varchar(50) DEFAULT NULL,
  `FirstName` varchar(50) DEFAULT NULL,
  `LastName` varchar(50) DEFAULT NULL,
  `OrganizationID` int(11) NOT NULL,
  `SecurityLevelID` int(11) NOT NULL,
  `PasswordHash` longtext,
  PRIMARY KEY (`UserID`,`OrganizationID`,`SecurityLevelID`),
  KEY `fk_user_organization1` (`OrganizationID`),
  KEY `fk_user_securitylevel1` (`SecurityLevelID`),
  CONSTRAINT `fk_user_organization1` FOREIGN KEY (`OrganizationID`) REFERENCES `organization` (`OrganizationID`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_user_securitylevel1` FOREIGN KEY (`SecurityLevelID`) REFERENCES `securitylevel` (`SecurityLevelID`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Final view structure for view `vwcanvasshare`
--

/*!50001 DROP TABLE IF EXISTS `vwcanvasshare`*/;
/*!50001 DROP VIEW IF EXISTS `vwcanvasshare`*/;
/*!50001 SET @saved_cs_client          = @@character_set_client */;
/*!50001 SET @saved_cs_results         = @@character_set_results */;
/*!50001 SET @saved_col_connection     = @@collation_connection */;
/*!50001 SET character_set_client      = utf8 */;
/*!50001 SET character_set_results     = utf8 */;
/*!50001 SET collation_connection      = utf8_general_ci */;
/*!50001 CREATE ALGORITHM=UNDEFINED */
/*!50013 DEFINER=`root`@`localhost` SQL SECURITY DEFINER */
/*!50001 VIEW `vwcanvasshare` AS (select `user`.`UserName` AS `UserName`,`datasource`.`DatasourceName` AS `DatasourceName`,`canvas`.`CanvasID` AS `CanvasID`,`canvas`.`CanvasName` AS `CanvasName`,`canvas`.`CanvasDescription` AS `CanvasDescription`,`canvas`.`CreatedDate` AS `CreatedDate`,`canvas`.`ModifiedDate` AS `ModifiedDate`,`canvas`.`CanvasContent` AS `CanvasContent`,`user`.`UserID` AS `UserID`,`datasource`.`OrganizationID` AS `OrganizationID`,`user`.`FirstName` AS `FirstName`,`user`.`LastName` AS `LastName`,`datasource`.`DatasourceID` AS `DatasourceID` from (((`canvas` join `sharedcanvases` on((`canvas`.`CanvasID` = `sharedcanvases`.`CanvasID`))) join `user` on((`sharedcanvases`.`UserID` = `user`.`UserID`))) join `datasource` on((`canvas`.`DatasourceID` = `datasource`.`DatasourceID`)))) */;
/*!50001 SET character_set_client      = @saved_cs_client */;
/*!50001 SET character_set_results     = @saved_cs_results */;
/*!50001 SET collation_connection      = @saved_col_connection */;

--
-- Final view structure for view `vwcanvasuser`
--

/*!50001 DROP TABLE IF EXISTS `vwcanvasuser`*/;
/*!50001 DROP VIEW IF EXISTS `vwcanvasuser`*/;
/*!50001 SET @saved_cs_client          = @@character_set_client */;
/*!50001 SET @saved_cs_results         = @@character_set_results */;
/*!50001 SET @saved_col_connection     = @@collation_connection */;
/*!50001 SET character_set_client      = utf8 */;
/*!50001 SET character_set_results     = utf8 */;
/*!50001 SET collation_connection      = utf8_general_ci */;
/*!50001 CREATE ALGORITHM=UNDEFINED */
/*!50013 DEFINER=`root`@`localhost` SQL SECURITY DEFINER */
/*!50001 VIEW `vwcanvasuser` AS select `canvas`.`CanvasID` AS `CanvasID`,`canvas`.`CanvasName` AS `CanvasName`,`canvas`.`CanvasDescription` AS `CanvasDescription`,`canvas`.`CreatedDate` AS `CreatedDate`,`canvas`.`ModifiedDate` AS `ModifiedDate`,`canvas`.`CanvasContent` AS `CanvasContent`,`user`.`UserID` AS `UserID`,`user`.`UserName` AS `UserName`,`user`.`FirstName` AS `FirstName`,`user`.`LastName` AS `LastName`,`user`.`OrganizationID` AS `OrganizationID`,`datasource`.`DatasourceID` AS `DatasourceID`,`datasource`.`DatasourceName` AS `DatasourceName` from (((`canvas` join `user` on((`canvas`.`UserID` = `user`.`UserID`))) join `datasourceuser` on((`datasourceuser`.`UserID` = `user`.`UserID`))) join `datasource` on(((`canvas`.`DatasourceID` = `datasource`.`DatasourceID`) and (`datasourceuser`.`DatasourceID` = `datasource`.`DatasourceID`)))) */;
/*!50001 SET character_set_client      = @saved_cs_client */;
/*!50001 SET character_set_results     = @saved_cs_results */;
/*!50001 SET collation_connection      = @saved_col_connection */;

--
-- Final view structure for view `vwuserdatasource`
--

/*!50001 DROP TABLE IF EXISTS `vwuserdatasource`*/;
/*!50001 DROP VIEW IF EXISTS `vwuserdatasource`*/;
/*!50001 SET @saved_cs_client          = @@character_set_client */;
/*!50001 SET @saved_cs_results         = @@character_set_results */;
/*!50001 SET @saved_col_connection     = @@collation_connection */;
/*!50001 SET character_set_client      = utf8 */;
/*!50001 SET character_set_results     = utf8 */;
/*!50001 SET collation_connection      = utf8_general_ci */;
/*!50001 CREATE ALGORITHM=UNDEFINED */
/*!50013 DEFINER=`root`@`localhost` SQL SECURITY DEFINER */
/*!50001 VIEW `vwuserdatasource` AS select `user`.`UserID` AS `UserID`,`user`.`UserName` AS `UserName`,`user`.`OrganizationID` AS `OrganizationID`,`user`.`PasswordHash` AS `PasswordHash`,`securitylevel`.`SecurityLevelValue` AS `SecurityLevelValue`,`securitylevel`.`SecurityLevelID` AS `SecurityLevelID`,`securitylevel`.`SecurityLevelDescription` AS `SecurityLevelDescription`,`datasource`.`DatasourceID` AS `DatasourceID`,`datasource`.`DatasourceName` AS `DatasourceName`,`datasource`.`DatasourceServerName` AS `DatasourceServerName`,`datasource`.`DatabaseType` AS `DatabaseType`,`datasource`.`InitialCatalog` AS `InitialCatalog`,`datasource`.`PersistSecurityInfo` AS `PersistSecurityInfo`,`datasource`.`Password` AS `Password`,`datasource`.`TableName` AS `TableName`,`datasource`.`SQLQuery` AS `SQLQuery`,`datasource`.`SQLText` AS `SQLText`,`datasource`.`active` AS `active`,`datasource`.`DatabaseUserID` AS `DatabaseUserID` from (((`datasourceuser` join `datasource` on((`datasourceuser`.`DatasourceID` = `datasource`.`DatasourceID`))) join `user` on((`datasourceuser`.`UserID` = `user`.`UserID`))) join `securitylevel` on((`user`.`SecurityLevelID` = `securitylevel`.`SecurityLevelID`))) */;
/*!50001 SET character_set_client      = @saved_cs_client */;
/*!50001 SET character_set_results     = @saved_cs_results */;
/*!50001 SET collation_connection      = @saved_col_connection */;

--
-- Final view structure for view `vwcanvasuser1`
--

/*!50001 DROP TABLE IF EXISTS `vwcanvasuser1`*/;
/*!50001 DROP VIEW IF EXISTS `vwcanvasuser1`*/;
/*!50001 SET @saved_cs_client          = @@character_set_client */;
/*!50001 SET @saved_cs_results         = @@character_set_results */;
/*!50001 SET @saved_col_connection     = @@collation_connection */;
/*!50001 SET character_set_client      = utf8 */;
/*!50001 SET character_set_results     = utf8 */;
/*!50001 SET collation_connection      = utf8_general_ci */;
/*!50001 CREATE ALGORITHM=UNDEFINED */
/*!50013 DEFINER=`root`@`localhost` SQL SECURITY DEFINER */
/*!50001 VIEW `vwcanvasuser1` AS (select `user`.`UserName` AS `UserName`,`datasource`.`DatasourceName` AS `DatasourceName`,`canvas`.`CanvasID` AS `CanvasID`,`canvas`.`CanvasName` AS `CanvasName`,`canvas`.`CanvasDescription` AS `CanvasDescription`,`canvas`.`CreatedDate` AS `CreatedDate`,`canvas`.`ModifiedDate` AS `ModifiedDate`,`canvas`.`CanvasContent` AS `CanvasContent`,`user`.`UserID` AS `UserID`,`user`.`OrganizationID` AS `OrganizationID`,`user`.`FirstName` AS `FirstName`,`user`.`LastName` AS `LastName`,`datasource`.`DatasourceID` AS `DatasourceID` from (((`canvas` join `user` on((`canvas`.`UserID` = `user`.`UserID`))) join `datasourceuser` on((`user`.`UserID` = `datasourceuser`.`UserID`))) join `datasource` on(((`canvas`.`DatasourceID` = `datasource`.`DatasourceID`) and (`datasourceuser`.`DatasourceID` = `datasource`.`DatasourceID`))))) */;
/*!50001 SET character_set_client      = @saved_cs_client */;
/*!50001 SET character_set_results     = @saved_cs_results */;
/*!50001 SET collation_connection      = @saved_col_connection */;

--
-- Dumping routines for database 'osels_ewav'
--
/*!50003 DROP PROCEDURE IF EXISTS `up_delete_canvas` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8 */ ;
/*!50003 SET character_set_results = utf8 */ ;
/*!50003 SET collation_connection  = utf8_general_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50020 DEFINER=`root`@`localhost`*/ /*!50003 PROCEDURE `up_delete_canvas`(
	CanvasID int(11)    
)
BEGIN
DELETE FROM `osels_ewav`.`canvas`
WHERE   CanvasID = CanvasID        ;

END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `up_read_all_canvases` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8 */ ;
/*!50003 SET character_set_results = utf8 */ ;
/*!50003 SET collation_connection  = utf8_general_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50020 DEFINER=`root`@`localhost`*/ /*!50003 PROCEDURE `up_read_all_canvases`(IN  userID  INT(11)   )
BEGIN

	select  *, CanvasName  from vwCanvasUser where UserID =   @userID  UNION 
	select   *,  concat ( '*',   CanvasName ) as  CanvasName       from vwCanvasShare  where UserID = @userID  Order By CreatedDate DESC    ; 
	 

END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `up_save_canvas` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8 */ ;
/*!50003 SET character_set_results = utf8 */ ;
/*!50003 SET collation_connection  = utf8_general_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50020 DEFINER=`root`@`localhost`*/ /*!50003 PROCEDURE `up_save_canvas`(
  
 CanvasContent longtext ,  
 CanvasDescription longtext,
CanvasName varchar(50),  
  CreatedDate datetime, 
 DatasourceID int(11), 
  ModifiedDate datetime, 
  UserID int(11) ,                 
 
 out    CanvasID int(11)      
)
BEGIN

INSERT INTO `osels_ewav`.`canvas`
(CanvasContent,
CanvasDescription,
CanvasName,
CreatedDate,
DatasourceID,
ModifiedDate,
UserID)
VALUES
(
 CanvasContent,
CanvasDescription,
CanvasName,
CreatedDate,
DatasourceID,
ModifiedDate,
UserID 
);

set    CanvasID =       LAST_INSERT_ID()        ;    
            
END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `up_update_canvas` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8 */ ;
/*!50003 SET character_set_results = utf8 */ ;
/*!50003 SET collation_connection  = utf8_general_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50020 DEFINER=`root`@`localhost`*/ /*!50003 PROCEDURE `up_update_canvas`(

	CanvasContent longtext ,  
	CanvasID int(11),              
	ModifiedDate datetime     

)
BEGIN

UPDATE `osels_ewav`.`canvas`
SET
CanvasContent = CanvasContent ,
ModifiedDate =  ModifiedDate

WHERE   CanvasID = CanvasID;

END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2013-02-07 14:29:20
