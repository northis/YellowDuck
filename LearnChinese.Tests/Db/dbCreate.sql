USE master

CREATE DATABASE [LearnChinese]
GO

USE [LearnChinese]
GO
/****** Object:  Table [dbo].[Score]    Script Date: 25.04.2017 19:02:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Score](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[IdUser] [bigint] NOT NULL,
	[IdWord] [bigint] NOT NULL,
	[OriginalWordCount] [int] NULL,
	[OriginalWordSuccessCount] [int] NULL,
	[LastView] [datetime] NOT NULL,
	[LastLearned] [datetime] NULL,
	[LastLearnMode] [varchar](50) NULL,
	[IsInLearnMode] [bit] NOT NULL,
	[RightAnswerNumber] [tinyint] NULL,
	[PronunciationCount] [int] NULL,
	[PronunciationSuccessCount] [int] NULL,
	[TranslationCount] [int] NULL,
	[TranslationSuccessCount] [int] NULL,
	[ViewCount] [int] NULL,
 CONSTRAINT [PK_Score] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[User]    Script Date: 25.04.2017 19:02:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[User](
	[IdUser] [bigint] NOT NULL,
	[Name] [nvarchar](50) NULL,
	[LastCommand] [nvarchar](50) NULL,
	[JoinDate] [datetime] NOT NULL,
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
	[IdUser] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[UserSharing]    Script Date: 25.04.2017 19:02:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserSharing](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[IdOwner] [bigint] NOT NULL,
	[IdFriend] [bigint] NOT NULL,
 CONSTRAINT [PK_UserSharing] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Word]    Script Date: 25.04.2017 19:02:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Word](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[OriginalWord] [nvarchar](100) NOT NULL,
	[Pronunciation] [nvarchar](100) NULL,
	[LastModified] [datetime] NOT NULL,
	[Translation] [nvarchar](250) NOT NULL,
	[Usage] [nvarchar](max) NULL,
	[CardAll] [varbinary](max) NULL,
	[CardOriginalWord] [varbinary](max) NULL,
	[CardTranslation] [varbinary](max) NULL,
	[CardPronunciation] [varbinary](max) NULL,
	[IdOwner] [bigint] NOT NULL,
	[SyllablesCount] [int] NOT NULL,
 CONSTRAINT [PK_Word] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

CREATE NONCLUSTERED INDEX [IX_Word_SyllablesCount] ON [dbo].[Word]
(
	SyllablesCount ASC
)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO

SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[Score] ADD  CONSTRAINT [DF_Score_IsInLearnMode]  DEFAULT ((0)) FOR [IsInLearnMode]
GO
ALTER TABLE [dbo].[User] ADD  CONSTRAINT [DF_User_JoinDate]  DEFAULT (getdate()) FOR [JoinDate]
GO
ALTER TABLE [dbo].[Word] ADD  CONSTRAINT [DF_Word_SyllablesCount]  DEFAULT ((1)) FOR [SyllablesCount]
GO
ALTER TABLE [dbo].[Score]  WITH CHECK ADD  CONSTRAINT [FK_Score_User] FOREIGN KEY([IdUser])
REFERENCES [dbo].[User] ([IdUser])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Score] CHECK CONSTRAINT [FK_Score_User]
GO
ALTER TABLE [dbo].[UserSharing]  WITH CHECK ADD  CONSTRAINT [FK_UserSharing_UserFriend] FOREIGN KEY([IdFriend])
REFERENCES [dbo].[User] ([IdUser])
GO
ALTER TABLE [dbo].[UserSharing] CHECK CONSTRAINT [FK_UserSharing_UserFriend]
GO
ALTER TABLE [dbo].[UserSharing]  WITH CHECK ADD  CONSTRAINT [FK_UserSharing_UserOwner] FOREIGN KEY([IdOwner])
REFERENCES [dbo].[User] ([IdUser])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UserSharing] CHECK CONSTRAINT [FK_UserSharing_UserOwner]
GO
ALTER TABLE [dbo].[Word]  WITH CHECK ADD  CONSTRAINT [FK_Word_User] FOREIGN KEY([IdOwner])
REFERENCES [dbo].[User] ([IdUser])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Word] CHECK CONSTRAINT [FK_Word_User]
GO
