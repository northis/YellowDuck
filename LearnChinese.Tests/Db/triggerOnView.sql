
CREATE VIEW [dbo].[Word]
AS
SELECT        dbo.WordMain.Id, dbo.WordMain.OriginalWord, dbo.WordMain.Pronunciation, dbo.WordMain.LastModified, dbo.WordMain.Translation, dbo.WordMain.Usage, dbo.WordMain.IdOwner, 
                         dbo.WordMain.SyllablesCount, dbo.WordFileA.Bytes AS CardAll, dbo.WordFileO.Bytes AS CardOriginalWord, dbo.WordFileP.Bytes AS CardPronunciation, dbo.WordFileT.Bytes AS CardTranslation
FROM            dbo.WordMain LEFT JOIN
                         dbo.WordFileA ON dbo.WordMain.IdCardAll = dbo.WordFileA.Id LEFT JOIN
                         dbo.WordFileO ON dbo.WordMain.IdCardOriginalWord = dbo.WordFileO.Id LEFT JOIN
                         dbo.WordFileP ON dbo.WordMain.IdCardPronunciation = dbo.WordFileP.Id LEFT JOIN
                         dbo.WordFileT ON dbo.WordMain.IdCardTranslation = dbo.WordFileT.Id
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TRIGGER dbo.OnWordInsert
   ON  dbo.Word
   instead of INSERT-- <Data_Modification_Statements, , INSERT,DELETE,UPDATE>
AS 
BEGIN
	SET NOCOUNT ON;

	DECLARE @OriginalWord nvarchar(100);
	DECLARE @Pronunciation nvarchar(100);
	DECLARE @LastModified datetime;
	DECLARE @Translation nvarchar(100);
	DECLARE @Usage nvarchar(max);
	DECLARE @IdOwner bigint;
	DECLARE @SyllablesCount int;
	DECLARE @IdWord bigint;

	DECLARE @IdCardAll uniqueidentifier;
	DECLARE @IdCardOriginalWord uniqueidentifier;
	DECLARE @IdCardPronunciation uniqueidentifier;
	DECLARE @IdCardTranslation uniqueidentifier;

	DECLARE @IdCardAllTbl table (id uniqueidentifier);
	DECLARE @IdCardOriginalWordTbl table (id uniqueidentifier);
	DECLARE @IdCardPronunciationTbl table (id uniqueidentifier);
	DECLARE @IdCardTranslationTbl table (id uniqueidentifier);

	DECLARE @CardAll varbinary(max);
	DECLARE @CardOriginalWord varbinary(max);
	DECLARE @CardPronunciation varbinary(max);
	DECLARE @CardTranslation varbinary(max);
		   
DECLARE insertWordCursor CURSOR FOR  
SELECT OriginalWord
      ,Pronunciation
      ,LastModified
      ,Translation
      ,Usage
      ,IdOwner
      ,SyllablesCount 
	  ,CardAll
      ,CardOriginalWord
      ,CardPronunciation
      ,CardTranslation
FROM inserted 

OPEN insertWordCursor   
FETCH NEXT FROM insertWordCursor INTO @OriginalWord,@Pronunciation,@LastModified,@Translation,@Usage,@IdOwner,@SyllablesCount,@CardAll,@CardOriginalWord,@CardTranslation,@CardPronunciation

WHILE @@FETCH_STATUS = 0   
BEGIN   
	if(@CardAll is not null)
	Begin
	INSERT INTO dbo.WordFileA
           (Bytes)
		   OUTPUT INSERTED.Id INTO @IdCardAllTbl
     VALUES
           (@CardAll)
	end else if(@CardOriginalWord is not null)
	Begin
	INSERT INTO dbo.WordFileO
           (Bytes)
		   OUTPUT INSERTED.Id INTO @IdCardOriginalWordTbl
     VALUES
           (@CardOriginalWord)
	end else if(@CardPronunciation is not null)
	Begin
	INSERT INTO dbo.WordFileP
           (Bytes)
		   OUTPUT INSERTED.Id INTO @IdCardPronunciationTbl
     VALUES
           (@CardPronunciation)
	end else if(@CardTranslation is not null)
	Begin
	INSERT INTO dbo.WordFileT
           (Bytes)
		   OUTPUT INSERTED.Id INTO @IdCardTranslationTbl
     VALUES
           (@CardTranslation)
		   end

	 INSERT INTO dbo.WordMain
           (OriginalWord
           ,Pronunciation
           ,LastModified
           ,Translation
           ,Usage
           ,IdOwner
           ,SyllablesCount
           ,IdCardAll
           ,IdCardOriginalWord
           ,IdCardPronunciation
           ,IdCardTranslation)

		   values(@OriginalWord,@Pronunciation,@LastModified,@Translation,@Usage,@IdOwner,@SyllablesCount,@IdCardAll,@IdCardOriginalWord,@IdCardPronunciation,@IdCardTranslation)
END   

CLOSE dinsertWordCursor  
DEALLOCATE insertWordCursor
END
GO
