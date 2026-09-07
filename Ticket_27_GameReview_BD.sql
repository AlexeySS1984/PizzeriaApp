USE [master]
GO
IF DB_ID(N'_Ticket27_GameReviewBD') IS NOT NULL
BEGIN
    ALTER DATABASE [_Ticket27_GameReviewBD] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [_Ticket27_GameReviewBD];
END
GO
CREATE DATABASE [_Ticket27_GameReviewBD]
GO
ALTER DATABASE [_Ticket27_GameReviewBD] SET RECOVERY SIMPLE
GO
USE [_Ticket27_GameReviewBD]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*
    Билет №27: оценка компьютерных игр
    Скрипт MS SQL Server. Создаёт БД, таблицы, связи, ограничения и тестовые данные.
    Сгенерировано по структуре прикреплённых примеров БД: авторизация, справочники, основные сущности,
    записи/заказы/отзывы, способы оплаты, расчёт сдачи или итоговой оценки.
*/
GO
CREATE TABLE [dbo].[AppUser](
    [ID] int IDENTITY(1,1) NOT NULL,
    [Login] nvarchar(50) NOT NULL,
    [Password] nvarchar(100) NOT NULL,
    [FIO] nvarchar(150) NOT NULL,
    [Birthday] date NULL,
    CONSTRAINT [PK_AppUser] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [UQ_AppUser_Login] UNIQUE ([Login])
)
GO
CREATE TABLE [dbo].[Genre](
    [ID] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(100) NOT NULL,
    CONSTRAINT [PK_Genre] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [UQ_Genre_Name] UNIQUE ([Name])
)
GO
CREATE TABLE [dbo].[Developer](
    [ID] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(150) NOT NULL,
    CONSTRAINT [PK_Developer] PRIMARY KEY CLUSTERED ([ID] ASC)
)
GO
CREATE TABLE [dbo].[Game](
    [ID] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(150) NOT NULL,
    [Description] nvarchar(1000) NULL,
    [GenreID] int NOT NULL,
    [DeveloperID] int NOT NULL,
    [Price] money NOT NULL,
    CONSTRAINT [PK_Game] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [CK_Game_Price] CHECK ([Price] >= 0)
)
GO
CREATE TABLE [dbo].[Review](
    [ID] int IDENTITY(1,1) NOT NULL,
    [AppUserID] int NOT NULL,
    [GameID] int NOT NULL,
    [AvgRating] decimal(3,2) NOT NULL,
    [Gameplay] tinyint NOT NULL, -- геймплей
    [Graphics] tinyint NOT NULL, -- графика
    [Story] tinyint NOT NULL, -- сюжет
    [Sound] tinyint NOT NULL, -- звуковое оформление
    [Optimization] tinyint NOT NULL, -- оптимизация
    [Comment] nvarchar(1000) NULL,
    [CreatedAt] datetime NOT NULL CONSTRAINT [DF_Review_CreatedAt] DEFAULT (GETDATE()),
    CONSTRAINT [PK_Review] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [UQ_Review_User_Item] UNIQUE ([AppUserID], [GameID]),
    CONSTRAINT [CK_Review_AvgRating] CHECK ([AvgRating] BETWEEN 1 AND 5)
)
GO
ALTER TABLE [dbo].[Game] ADD CONSTRAINT [FK_Game_Genre] FOREIGN KEY ([GenreID]) REFERENCES [dbo].[Genre]([ID])
GO
ALTER TABLE [dbo].[Game] ADD CONSTRAINT [FK_Game_Developer] FOREIGN KEY ([DeveloperID]) REFERENCES [dbo].[Developer]([ID])
GO
ALTER TABLE [dbo].[Review] ADD CONSTRAINT [FK_Review_AppUser] FOREIGN KEY ([AppUserID]) REFERENCES [dbo].[AppUser]([ID])
GO
ALTER TABLE [dbo].[Review] ADD CONSTRAINT [FK_Review_Game] FOREIGN KEY ([GameID]) REFERENCES [dbo].[Game]([ID])
GO
ALTER TABLE [dbo].[Review] ADD CONSTRAINT [CK_Review_Gameplay] CHECK ([Gameplay] BETWEEN 1 AND 5)
GO
ALTER TABLE [dbo].[Review] ADD CONSTRAINT [CK_Review_Graphics] CHECK ([Graphics] BETWEEN 1 AND 5)
GO
ALTER TABLE [dbo].[Review] ADD CONSTRAINT [CK_Review_Story] CHECK ([Story] BETWEEN 1 AND 5)
GO
ALTER TABLE [dbo].[Review] ADD CONSTRAINT [CK_Review_Sound] CHECK ([Sound] BETWEEN 1 AND 5)
GO
ALTER TABLE [dbo].[Review] ADD CONSTRAINT [CK_Review_Optimization] CHECK ([Optimization] BETWEEN 1 AND 5)
GO
INSERT INTO [dbo].[AppUser] ([Login], [Password], [FIO], [Birthday])
VALUES
    (N'ivanov', N'12345', N'Иванов Иван Иванович', N'2002-04-15'),
    (N'petrova', N'12345', N'Петрова Анна Сергеевна', N'2001-09-22'),
    (N'sidorov', N'12345', N'Сидоров Павел Олегович', NULL),
    (N'test', N'test', N'Тестовый Пользователь', NULL);
GO
INSERT INTO [dbo].[Genre] ([Name])
VALUES
    (N'RPG'),
    (N'Action'),
    (N'Strategy'),
    (N'Simulator'),
    (N'Adventure');
GO
INSERT INTO [dbo].[Developer] ([Name])
VALUES
    (N'North Studio'),
    (N'Pixel Forge'),
    (N'Red Planet Games'),
    (N'IndieLab');
GO
INSERT INTO [dbo].[Game] ([Name], [Description], [GenreID], [DeveloperID], [Price])
VALUES
    (N'Legends of Code', N'RPG с открытым миром', 1, 1, 2499.00),
    (N'Neon Runner', N'Динамичный action-платформер', 2, 2, 1499.00),
    (N'Empire Logic', N'Пошаговая стратегия', 3, 3, 1999.00),
    (N'City Builder Pro', N'Градостроительный симулятор', 4, 4, 1799.00),
    (N'Lost Island', N'Приключенческая игра с загадками', 5, 2, 1299.00);
GO
INSERT INTO [dbo].[Review] ([AppUserID], [GameID], [AvgRating], [Gameplay], [Graphics], [Story], [Sound], [Optimization], [Comment], [CreatedAt])
VALUES
    (1, 1, 4.40, 5, 4, 5, 4, 4, N'Большой мир и интересные квесты.', GETDATE()),
    (2, 2, 4.20, 4, 5, 3, 5, 4, N'Очень стильная графика.', GETDATE()),
    (3, 3, 4.20, 5, 3, 4, 4, 5, N'Глубокая стратегия.', GETDATE());
GO
CREATE VIEW [dbo].[vwGameWithRating]
AS
SELECT
    i.[ID],
    i.[Name],
    i.[Description],
    c.[Name] AS [CategoryName],
    m.[Name] AS [MakerName],
    i.[Price],
    CAST(ISNULL(AVG(r.[AvgRating]), 0) AS decimal(3,2)) AS [CurrentRating],
    COUNT(r.[ID]) AS [ReviewCount]
FROM [dbo].[Game] i
INNER JOIN [dbo].[Genre] c ON i.[GenreID] = c.[ID]
INNER JOIN [dbo].[Developer] m ON i.[DeveloperID] = m.[ID]
LEFT JOIN [dbo].[Review] r ON i.[ID] = r.[GameID]
GROUP BY i.[ID], i.[Name], i.[Description], c.[Name], m.[Name], i.[Price];
GO
CREATE VIEW [dbo].[vwUserReviews]
AS
SELECT
    r.[ID],
    u.[Login],
    u.[FIO],
    i.[Name] AS [GameName],
    r.[AvgRating],
    r.[Gameplay],
    r.[Graphics],
    r.[Story],
    r.[Sound],
    r.[Optimization],
    r.[Comment],
    r.[CreatedAt]
FROM [dbo].[Review] r
INNER JOIN [dbo].[AppUser] u ON r.[AppUserID] = u.[ID]
INNER JOIN [dbo].[Game] i ON r.[GameID] = i.[ID];
GO
