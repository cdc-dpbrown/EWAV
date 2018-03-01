
USE [OSELS_EWAV]
GO
/****** Object:  StoredProcedure [dbo].[usp_pivot_responseXml   ]    Script Date: 01/10/2014 11:40:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

 ALTER PROCEDURE [dbo].[usp_pivot_responseXml   ]
	@SurveyId VARCHAR (100), 
	@metaDataTable EIWSSurveyMetaData READONLY, 
	@responsesTable EIWSSurveyResponses READONLY, 
	@dropStatement VARCHAR (MAX), 
	@createStatement VARCHAR (MAX), 
	@workTablePlaceholderName VARCHAR (50)
	
AS
BEGIN

    SET NOCOUNT ON;

    IF OBJECT_ID('tempdb..##outputTable') IS NOT NULL
        DROP TABLE ##outputTable;

    --Create temp Table name    
    DECLARE @outputTableName AS NVARCHAR (70);
    DECLARE @nstrGUID AS NCHAR (36) = NEWID();
    SET @outputTableName = '##outputTable' + @nstrGUID;
    SET @outputTableName = REPLACE(@outputTableName, '-', '');

    -- Prepare drop statement 
    SET @dropStatement = replace(@dropStatement, @workTablePlaceholderName, @outputTableName);
    -- Prepare create statement    
    SET @createStatement = replace(@createStatement, @workTablePlaceholderName, @outputTableName);

    -- Create  Work Table    
    EXECUTE (@createStatement);

    DECLARE @responseId AS UNIQUEIDENTIFIER;

    DECLARE RESPONSES CURSOR FAST_FORWARD
        FOR SELECT *
            FROM   @responsesTable;
    OPEN RESPONSES;
    FETCH NEXT FROM RESPONSES INTO @responseId;
    WHILE (@@FETCH_STATUS = 0)
        BEGIN

            -- Get response xml                   
            DECLARE @xmlDocument AS XML = (SELECT ResponseXML
                                           FROM   [OSELS_EIWS].[dbo].[SurveyResponse]
                                           WHERE  responseId = @responseId);

            -- Simple replace of Yes with 1 / No with 
            DECLARE @xmltext AS VARCHAR (MAX);
            SET @xmltext = CONVERT (VARCHAR (MAX), @xmlDocument);
            SET @xmltext = REPLACE(@xmltext, 'Yes', 1);
            SET @xmltext = REPLACE(@xmltext, 'No', 0);
            SET @xmlDocument = CONVERT (XML, @xmltext);

            IF OBJECT_ID('tempdb..#responses') IS NOT NULL
                DROP TABLE #responses;
            IF OBJECT_ID('tempdb..#metaData') IS NOT NULL
                DROP TABLE #metaData;

            -- Create responses tab    
            SELECT   T.c.value('(@QuestionName)[1 ]  ', 'Varchar(100 )  ') AS Question,
                     T.c.value('(.)', 'Varchar(100 )  ') AS Response
            INTO     #responses
            FROM     @xmlDocument.nodes('/SurveyResponse/Page/*') AS T(c)
            ORDER BY Question;

             --  Explicilty deal with fields that should NULL    
            UPDATE  #responses
                SET response = NULL
            WHERE   response = '';

            DECLARE @cols AS NVARCHAR (MAX);
            DECLARE @queryCreate AS NVARCHAR (MAX);
            DECLARE @queryAppend AS NVARCHAR (MAX);

            SELECT *
            INTO   #metaData
            FROM   @metaDataTable;

            -- Get a master list of cold from the metadata    
            SET @cols = STUFF((SELECT ',' + QUOTENAME(c.name)
                               FROM   #metaData AS c
                               FOR    XML PATH (''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 1, '');

            -- Set up for pivot on Append     
            SET @queryAppend = '  INSERT   INTO   ' + @outputTableName + '  SELECT  ' + @cols + '  from 
            (
                select   *  
                from   #responses    
           ) x               
            pivot 
            (
                  max  ( x.response                             )   
                for  x.question    in (' + @cols + ')
            ) p      ';

            -- Execute pivot             
            EXECUTE (@queryAppend);

            FETCH NEXT FROM RESPONSES INTO @responseId;
        END

    -- Drop columns     
    EXECUTE (@dropStatement);
	
	-- select results    
    EXECUTE (' select * from ' + @outputTableName);

    CLOSE RESPONSES;
    DEALLOCATE RESPONSES;
END



