USE [master]
GO
/****** Object:  Database [igroup14_prod]    Script Date: 27/06/2025 15:06:52 ******/
CREATE DATABASE [igroup14_prod]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'igroup14_prod', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL13.PROJDB\MSSQL\DATA\igroup14_prod.mdf' , SIZE = 36096KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'igroup14_prod_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL13.PROJDB\MSSQL\DATA\igroup14_prod_log.ldf' , SIZE = 114432KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [igroup14_prod] SET COMPATIBILITY_LEVEL = 100
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [igroup14_prod].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [igroup14_prod] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [igroup14_prod] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [igroup14_prod] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [igroup14_prod] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [igroup14_prod] SET ARITHABORT OFF 
GO
ALTER DATABASE [igroup14_prod] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [igroup14_prod] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [igroup14_prod] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [igroup14_prod] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [igroup14_prod] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [igroup14_prod] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [igroup14_prod] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [igroup14_prod] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [igroup14_prod] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [igroup14_prod] SET  DISABLE_BROKER 
GO
ALTER DATABASE [igroup14_prod] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [igroup14_prod] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [igroup14_prod] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [igroup14_prod] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [igroup14_prod] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [igroup14_prod] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [igroup14_prod] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [igroup14_prod] SET RECOVERY FULL 
GO
ALTER DATABASE [igroup14_prod] SET  MULTI_USER 
GO
ALTER DATABASE [igroup14_prod] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [igroup14_prod] SET DB_CHAINING OFF 
GO
ALTER DATABASE [igroup14_prod] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [igroup14_prod] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
ALTER DATABASE [igroup14_prod] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [igroup14_prod] SET QUERY_STORE = OFF
GO
USE [igroup14_prod]
GO
ALTER DATABASE SCOPED CONFIGURATION SET LEGACY_CARDINALITY_ESTIMATION = OFF;
GO
ALTER DATABASE SCOPED CONFIGURATION SET MAXDOP = 0;
GO
ALTER DATABASE SCOPED CONFIGURATION SET PARAMETER_SNIFFING = ON;
GO
ALTER DATABASE SCOPED CONFIGURATION SET QUERY_OPTIMIZER_HOTFIXES = OFF;
GO
USE [igroup14_prod]
GO
/****** Object:  User [sa]    Script Date: 27/06/2025 15:06:52 ******/
CREATE USER [sa] WITHOUT LOGIN WITH DEFAULT_SCHEMA=[sa]
GO
/****** Object:  User [igroup14_DBuser]    Script Date: 27/06/2025 15:06:52 ******/
CREATE USER [igroup14_DBuser] WITHOUT LOGIN WITH DEFAULT_SCHEMA=[dbo]
GO
ALTER ROLE [db_owner] ADD MEMBER [igroup14_DBuser]
GO
/****** Object:  Table [dbo].[CalculatorItemCandidates]    Script Date: 27/06/2025 15:06:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CalculatorItemCandidates](
	[CandidateID] [int] IDENTITY(1,1) NOT NULL,
	[CustomItemName] [nvarchar](255) NOT NULL,
	[SuggestedPrice] [decimal](10, 2) NOT NULL,
	[SuggestedDescription] [nvarchar](255) NOT NULL,
	[Occurrences] [int] NULL,
	[IsCreated] [bit] NULL,
	[WasRejected] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[CandidateID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Customer]    Script Date: 27/06/2025 15:06:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Customer](
	[CustomerID] [int] IDENTITY(1,1) NOT NULL,
	[FirstName] [nvarchar](255) NULL,
	[LastName] [nvarchar](255) NULL,
	[Phone] [nvarchar](50) NULL,
	[Email] [nvarchar](255) NULL,
	[CreatedAt] [datetime] NULL,
	[City] [nvarchar](255) NULL,
	[Street] [nvarchar](255) NULL,
	[Number] [nvarchar](50) NULL,
	[Notes] [nvarchar](255) NULL,
	[IsActive] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[CustomerID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CustomerFeedback]    Script Date: 27/06/2025 15:06:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CustomerFeedback](
	[FeedbackID] [int] IDENTITY(1,1) NOT NULL,
	[CustomerID] [int] NULL,
	[RequestID] [int] NULL,
	[Sent] [bit] NULL,
	[SentAt] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[FeedbackID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ParquetTypes]    Script Date: 27/06/2025 15:06:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ParquetTypes](
	[ParquetTypeID] [int] IDENTITY(1,1) NOT NULL,
	[TypeName] [nvarchar](255) NOT NULL,
	[PricePerUnit] [decimal](10, 2) NOT NULL,
	[IsActive] [bit] NULL,
	[ImageURL] [nvarchar](500) NULL,
	[Type] [nvarchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[ParquetTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PriceCalculatorItem]    Script Date: 27/06/2025 15:06:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PriceCalculatorItem](
	[CalculatorItemID] [int] IDENTITY(1,1) NOT NULL,
	[ItemName] [nvarchar](255) NULL,
	[Description] [nvarchar](255) NULL,
	[Price] [decimal](10, 2) NULL,
	[IsActive] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[CalculatorItemID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PriceEstimates]    Script Date: 27/06/2025 15:06:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PriceEstimates](
	[EstimateID] [int] IDENTITY(1,1) NOT NULL,
	[RequestID] [int] NOT NULL,
	[TotalArea] [decimal](10, 2) NOT NULL,
	[ParquetType] [nvarchar](100) NOT NULL,
	[RoomCount] [int] NOT NULL,
	[BasePrice] [decimal](10, 2) NOT NULL,
	[EstimatedMinPrice] [decimal](10, 2) NOT NULL,
	[EstimatedMaxPrice] [decimal](10, 2) NOT NULL,
	[EstimatedMinDays] [int] NOT NULL,
	[EstimatedMaxDays] [int] NOT NULL,
	[ComplexityMultiplier] [decimal](5, 3) NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
	[Notes] [nvarchar](500) NULL,
PRIMARY KEY CLUSTERED 
(
	[EstimateID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Quote]    Script Date: 27/06/2025 15:06:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Quote](
	[QuoteID] [int] IDENTITY(1,1) NOT NULL,
	[RequestID] [int] NOT NULL,
	[CreatedAt] [datetime] NULL,
	[TotalPrice] [decimal](10, 2) NULL,
	[DiscountAmount] [decimal](10, 2) NULL,
	[DiscountPercent] [decimal](5, 2) NULL,
PRIMARY KEY CLUSTERED 
(
	[QuoteID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[QuoteItem]    Script Date: 27/06/2025 15:06:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[QuoteItem](
	[QuoteItemID] [int] IDENTITY(1,1) NOT NULL,
	[QuoteID] [int] NOT NULL,
	[CalculatorItemID] [int] NULL,
	[CustomItemName] [nvarchar](255) NULL,
	[PriceForItem] [decimal](10, 2) NULL,
	[Quantity] [int] NULL,
	[FinalPrice] [decimal](10, 2) NULL,
	[Notes] [nvarchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[QuoteItemID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ScheduleSlot]    Script Date: 27/06/2025 15:06:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ScheduleSlot](
	[SlotID] [int] IDENTITY(1,1) NOT NULL,
	[Date] [date] NULL,
	[Slot] [int] NULL,
	[Status] [nvarchar](50) NULL,
	[RequestID] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[SlotID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SpaceDetails]    Script Date: 27/06/2025 15:06:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SpaceDetails](
	[SpaceID] [int] IDENTITY(1,1) NOT NULL,
	[RequestID] [int] NOT NULL,
	[Size] [decimal](10, 2) NULL,
	[FloorType] [nvarchar](100) NULL,
	[MediaURL] [nvarchar](255) NULL,
	[Notes] [nvarchar](max) NULL,
	[ParquetType] [nvarchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[SpaceID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SystemSettings]    Script Date: 27/06/2025 15:06:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SystemSettings](
	[SettingID] [int] IDENTITY(1,1) NOT NULL,
	[SettingKey] [nvarchar](100) NOT NULL,
	[SettingValue] [nvarchar](500) NOT NULL,
	[SettingType] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](255) NULL,
	[UpdatedAt] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[SettingID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[WorkRequest]    Script Date: 27/06/2025 15:06:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WorkRequest](
	[RequestID] [int] IDENTITY(1,1) NOT NULL,
	[CustomerID] [int] NOT NULL,
	[CreatedAt] [datetime] NULL,
	[Status] [nvarchar](50) NULL,
	[Notes] [nvarchar](255) NULL,
	[PlannedDate] [datetime] NULL,
	[CompletedDate] [datetime] NULL,
	[PreferredDate] [datetime] NULL,
	[PreferredSlot] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[RequestID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[WorkRequestStatuses]    Script Date: 27/06/2025 15:06:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WorkRequestStatuses](
	[StatusID] [int] IDENTITY(1,1) NOT NULL,
	[StatusName] [nvarchar](100) NOT NULL,
	[StatusOrder] [int] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[Description] [nvarchar](255) NULL,
	[CreatedAt] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[StatusID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[CalculatorItemCandidates] ADD  DEFAULT ((1)) FOR [Occurrences]
GO
ALTER TABLE [dbo].[CalculatorItemCandidates] ADD  DEFAULT ((0)) FOR [IsCreated]
GO
ALTER TABLE [dbo].[CalculatorItemCandidates] ADD  DEFAULT ((0)) FOR [WasRejected]
GO
ALTER TABLE [dbo].[Customer] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Customer] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[ParquetTypes] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[PriceEstimates] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Quote] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[SystemSettings] ADD  DEFAULT ('STRING') FOR [SettingType]
GO
ALTER TABLE [dbo].[SystemSettings] ADD  DEFAULT (getdate()) FOR [UpdatedAt]
GO
ALTER TABLE [dbo].[WorkRequest] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[WorkRequestStatuses] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[WorkRequestStatuses] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[CustomerFeedback]  WITH CHECK ADD FOREIGN KEY([CustomerID])
REFERENCES [dbo].[Customer] ([CustomerID])
GO
ALTER TABLE [dbo].[CustomerFeedback]  WITH CHECK ADD FOREIGN KEY([RequestID])
REFERENCES [dbo].[WorkRequest] ([RequestID])
GO
ALTER TABLE [dbo].[PriceEstimates]  WITH CHECK ADD FOREIGN KEY([RequestID])
REFERENCES [dbo].[WorkRequest] ([RequestID])
GO
ALTER TABLE [dbo].[Quote]  WITH CHECK ADD FOREIGN KEY([RequestID])
REFERENCES [dbo].[WorkRequest] ([RequestID])
GO
ALTER TABLE [dbo].[QuoteItem]  WITH CHECK ADD FOREIGN KEY([CalculatorItemID])
REFERENCES [dbo].[PriceCalculatorItem] ([CalculatorItemID])
GO
ALTER TABLE [dbo].[QuoteItem]  WITH CHECK ADD FOREIGN KEY([QuoteID])
REFERENCES [dbo].[Quote] ([QuoteID])
GO
ALTER TABLE [dbo].[ScheduleSlot]  WITH CHECK ADD FOREIGN KEY([RequestID])
REFERENCES [dbo].[WorkRequest] ([RequestID])
GO
ALTER TABLE [dbo].[SpaceDetails]  WITH CHECK ADD FOREIGN KEY([RequestID])
REFERENCES [dbo].[WorkRequest] ([RequestID])
GO
ALTER TABLE [dbo].[SpaceDetails]  WITH CHECK ADD FOREIGN KEY([RequestID])
REFERENCES [dbo].[WorkRequest] ([RequestID])
GO
ALTER TABLE [dbo].[SpaceDetails]  WITH CHECK ADD FOREIGN KEY([RequestID])
REFERENCES [dbo].[WorkRequest] ([RequestID])
GO
ALTER TABLE [dbo].[SpaceDetails]  WITH CHECK ADD FOREIGN KEY([RequestID])
REFERENCES [dbo].[WorkRequest] ([RequestID])
GO
ALTER TABLE [dbo].[WorkRequest]  WITH CHECK ADD FOREIGN KEY([CustomerID])
REFERENCES [dbo].[Customer] ([CustomerID])
GO
/****** Object:  StoredProcedure [dbo].[AddParquetType]    Script Date: 27/06/2025 15:06:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[AddParquetType]
    @TypeName NVARCHAR(255),
    @PricePerUnit DECIMAL(10, 2),
    @ImageURL NVARCHAR(500),
	@Type NVARCHAR(255)
AS
BEGIN
    INSERT INTO ParquetTypes (TypeName, PricePerUnit, ImageURL, IsActive,[Type])
    VALUES (@TypeName, @PricePerUnit, @ImageURL, 1,@Type);
END
GO
/****** Object:  StoredProcedure [dbo].[AddPriceCalculatorItem]    Script Date: 27/06/2025 15:06:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[AddPriceCalculatorItem]
    @ItemName NVARCHAR(255),
    @Description NVARCHAR(255),
    @Price DECIMAL(10, 2)
    
AS
BEGIN
    

    INSERT INTO PriceCalculatorItem (ItemName, Description, Price, IsActive)
    VALUES (@ItemName, @Description, @Price, 1);

    SELECT SCOPE_IDENTITY() AS NewCalculatorItemID;
END;
GO
/****** Object:  StoredProcedure [dbo].[AddQuote]    Script Date: 27/06/2025 15:06:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[AddQuote]
    @RequestID INT,
    @TotalPrice DECIMAL(10, 2),
    @DiscountAmount DECIMAL(10, 2) = NULL,
    @DiscountPercent DECIMAL(5, 2) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Quote (RequestID, TotalPrice, DiscountAmount, DiscountPercent)
    VALUES (@RequestID, @TotalPrice, @DiscountAmount, @DiscountPercent);

    SELECT SCOPE_IDENTITY() AS NewQuoteID;
END
GO
/****** Object:  StoredProcedure [dbo].[AddQuoteItem]    Script Date: 27/06/2025 15:06:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[AddQuoteItem]
    @QuoteID INT,
    @CalculatorItemID INT = NULL,
    @CustomItemName NVARCHAR(255) = NULL,
    @PriceForItem DECIMAL(10, 2),
    @Quantity INT,
    @FinalPrice DECIMAL(10, 2),
    @Notes NVARCHAR(255) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- הכנסת פריט להצעת מחיר
    INSERT INTO QuoteItem 
    (QuoteID, CalculatorItemID, CustomItemName, PriceForItem, Quantity, FinalPrice, Notes)
    VALUES 
    (@QuoteID, @CalculatorItemID, @CustomItemName, @PriceForItem, @Quantity, @FinalPrice, @Notes);

    -- בדיקת מועמדים
    IF @CalculatorItemID IS NULL AND @CustomItemName IS NOT NULL
    BEGIN
        EXEC UpsertCalculatorItemCandidate @CustomItemName, @PriceForItem;
    END

    SELECT SCOPE_IDENTITY() AS NewQuoteItemID;
END
GO
/****** Object:  StoredProcedure [dbo].[AssignRequestToSlot]    Script Date: 27/06/2025 15:06:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[AssignRequestToSlot]
    @RequestID INT,
    @Date DATE,
    @Slot INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE WorkRequest
    SET
        PlannedDate = @Date,
        PreferredSlot = @Slot,
        Status = N'תיאום התקנה'
    WHERE
        RequestID = @RequestID;
END
GO
/****** Object:  StoredProcedure [dbo].[CreatePriceCalculatorItemFromCandidate]    Script Date: 27/06/2025 15:06:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[CreatePriceCalculatorItemFromCandidate]
    @CustomItemName NVARCHAR(255)
AS
BEGIN
    DECLARE @Price DECIMAL(10,2)

    -- שלב 1: בדיקה אם כבר נוצר רכיב במחשבון עם אותו שם
    IF EXISTS (
        SELECT 1 
        FROM PriceCalculatorItem 
        WHERE ItemName = @CustomItemName
    )
    BEGIN
        RAISERROR('already exists!!!', 16, 1)
        RETURN
    END


    SELECT TOP 1 @Price = SuggestedPrice
    FROM CalculatorItemCandidates
    WHERE CustomItemName = @CustomItemName AND IsCreated = 0


    INSERT INTO PriceCalculatorItem (ItemName, Description, Price, IsActive)
    VALUES (@CustomItemName, @CustomItemName, @Price, 1)

    UPDATE CalculatorItemCandidates
    SET IsCreated = 1
    WHERE CustomItemName = @CustomItemName
END
GO
/****** Object:  StoredProcedure [dbo].[DeactivateCustomer]    Script Date: 27/06/2025 15:06:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[DeactivateCustomer]
    @CustomerID INT
AS
BEGIN
    UPDATE Customer
    SET IsActive = 0
    WHERE CustomerID = @CustomerID;
END
GO
/****** Object:  StoredProcedure [dbo].[DeleteParquetType]    Script Date: 27/06/2025 15:06:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[DeleteParquetType]
    @ParquetTypeID INT
AS
BEGIN
    UPDATE ParquetTypes
    SET IsActive = 0
    WHERE ParquetTypeID = @ParquetTypeID
END
GO
/****** Object:  StoredProcedure [dbo].[DeletePriceCalculatorItem]    Script Date: 27/06/2025 15:06:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[DeletePriceCalculatorItem] 
	@CalculatorItemID int
AS
BEGIN
	update PriceCalculatorItem
	set IsActive=0
	where CalculatorItemID =@CalculatorItemID
END
GO
/****** Object:  StoredProcedure [dbo].[GetAllParquetTypes]    Script Date: 27/06/2025 15:06:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetAllParquetTypes]
AS
BEGIN
    SELECT 
        ParquetTypeID,
        TypeName,
        PricePerUnit,
        ImageURL,
        IsActive,
		[Type]
    FROM ParquetTypes
    WHERE IsActive = 1
END
GO
/****** Object:  StoredProcedure [dbo].[GetAllPriceCalculatorItems]    Script Date: 27/06/2025 15:06:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetAllPriceCalculatorItems]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        CalculatorItemID,
        ItemName,
        [Description],
        Price,
        IsActive
    FROM PriceCalculatorItem
	where IsActive=1
END
GO
/****** Object:  StoredProcedure [dbo].[GetAllPriceEstimates]    Script Date: 27/06/2025 15:06:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetAllPriceEstimates]
    @FromDate DATETIME = NULL,
    @ToDate DATETIME = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        pe.EstimateID,
        pe.RequestID,
        pe.TotalArea,
        pe.ParquetType,
        pe.RoomCount,
        pe.BasePrice,
        pe.EstimatedMinPrice,
        pe.EstimatedMaxPrice,
        pe.EstimatedMinDays,
        pe.EstimatedMaxDays,
        pe.ComplexityMultiplier,
        pe.CreatedAt,
        pe.Notes,
        -- מידע על הלקוח והבקשה
        c.FirstName,
        c.LastName,
        c.Phone,
        c.City,
        wr.Status,
        wr.PlannedDate
    FROM PriceEstimates pe
    INNER JOIN WorkRequest wr ON pe.RequestID = wr.RequestID
    INNER JOIN Customer c ON wr.CustomerID = c.CustomerID
    WHERE 
        (@FromDate IS NULL OR pe.CreatedAt >= @FromDate)
        AND (@ToDate IS NULL OR pe.CreatedAt <= @ToDate)
        AND c.IsActive = 1
    ORDER BY pe.CreatedAt DESC
END
GO
/****** Object:  StoredProcedure [dbo].[GetBIStatistics]    Script Date: 27/06/2025 15:06:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetBIStatistics]
    @FromDate DATETIME = NULL,
    @ToDate DATETIME = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @Results TABLE (
        StatType NVARCHAR(50),
        StatValue DECIMAL(18,2),
        StatCount INT,
        StatText NVARCHAR(255)
    );
    
    -- סך הצעות מחיר
    INSERT INTO @Results (StatType, StatValue, StatCount)
    SELECT 
        'TotalQuotes',
        ISNULL(SUM(q.TotalPrice), 0),
        COUNT(*)
    FROM Quote q
    INNER JOIN WorkRequest wr ON q.RequestID = wr.RequestID
    INNER JOIN Customer c ON wr.CustomerID = c.CustomerID
    WHERE 
        (@FromDate IS NULL OR c.CreatedAt >= @FromDate)
        AND (@ToDate IS NULL OR c.CreatedAt <= @ToDate)
        AND c.IsActive = 1;
    
    -- מספר לקוחות
    INSERT INTO @Results (StatType, StatCount)
    SELECT 
        'TotalCustomers',
        COUNT(DISTINCT c.CustomerID)
    FROM Customer c
    INNER JOIN WorkRequest wr ON c.CustomerID = wr.CustomerID
    WHERE 
        (@FromDate IS NULL OR c.CreatedAt >= @FromDate)
        AND (@ToDate IS NULL OR c.CreatedAt <= @ToDate)
        AND c.IsActive = 1;
    
    -- התקנות שבוצעו
    INSERT INTO @Results (StatType, StatCount)
    SELECT 
        'CompletedInstalls',
        COUNT(*)
    FROM WorkRequest wr
    INNER JOIN Customer c ON wr.CustomerID = c.CustomerID
    WHERE 
        wr.Status = N'התקנה בוצעה'
        AND (@FromDate IS NULL OR c.CreatedAt >= @FromDate)
        AND (@ToDate IS NULL OR c.CreatedAt <= @ToDate)
        AND c.IsActive = 1;
    
    -- סה"כ שטח
    INSERT INTO @Results (StatType, StatValue)
    SELECT 
        'TotalArea',
        ISNULL(SUM(sd.Size), 0)
    FROM SpaceDetails sd
    INNER JOIN WorkRequest wr ON sd.RequestID = wr.RequestID
    INNER JOIN Customer c ON wr.CustomerID = c.CustomerID
    WHERE 
        (@FromDate IS NULL OR c.CreatedAt >= @FromDate)
        AND (@ToDate IS NULL OR c.CreatedAt <= @ToDate)
        AND c.IsActive = 1;
    
    -- סטטוסים
    INSERT INTO @Results (StatType, StatText, StatCount)
    SELECT 
        'StatusCount',
        wr.Status,
        COUNT(*)
    FROM WorkRequest wr
    INNER JOIN Customer c ON wr.CustomerID = c.CustomerID
    WHERE 
        (@FromDate IS NULL OR c.CreatedAt >= @FromDate)
        AND (@ToDate IS NULL OR c.CreatedAt <= @ToDate)
        AND c.IsActive = 1
    GROUP BY wr.Status;
    
    -- פילוח לפי עיר
    INSERT INTO @Results (StatType, StatText, StatCount)
    SELECT 
        'CityDistribution',
        c.City,
        COUNT(*)
    FROM Customer c
    INNER JOIN WorkRequest wr ON c.CustomerID = wr.CustomerID
    WHERE 
        (@FromDate IS NULL OR c.CreatedAt >= @FromDate)
        AND (@ToDate IS NULL OR c.CreatedAt <= @ToDate)
        AND c.IsActive = 1
    GROUP BY c.City;
    
    -- פילוח לפי סוג פרקט
    INSERT INTO @Results (StatType, StatText, StatCount)
    SELECT 
        'ParquetTypeDistribution',
        sd.ParquetType,
        COUNT(*)
    FROM SpaceDetails sd
    INNER JOIN WorkRequest wr ON sd.RequestID = wr.RequestID
    INNER JOIN Customer c ON wr.CustomerID = c.CustomerID
    WHERE 
        sd.ParquetType IS NOT NULL
        AND (@FromDate IS NULL OR c.CreatedAt >= @FromDate)
        AND (@ToDate IS NULL OR c.CreatedAt <= @ToDate)
        AND c.IsActive = 1
    GROUP BY sd.ParquetType;
    
    SELECT * FROM @Results;
END
GO
/****** Object:  StoredProcedure [dbo].[GetDashboardData]    Script Date: 27/06/2025 15:06:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetDashboardData]
  @CustomerID INT = NULL,
  @City NVARCHAR(255) = NULL,
  @FromDate DATETIME = NULL,
  @ToDate DATETIME = NULL,
  @FloorType NVARCHAR(100) = NULL,
  @Status NVARCHAR(50) = NULL
AS
BEGIN
  --################## עבור לקוח ספציפי ##################
  IF @CustomerID IS NOT NULL
  BEGIN
    SELECT 
      c.CustomerID,
      c.FirstName,
      c.LastName,
      c.Phone,
      c.City,
      c.Street,
      c.Number,
      c.Notes,
      c.CreatedAt AS CustomerCreatedAt,
      c.Email,
      wr.RequestID,
      wr.PlannedDate,
      wr.CompletedDate,
      wr.PreferredDate,
      wr.PreferredSlot,
      wr.Status,
      sd.SpaceID,
      sd.Size,
      sd.FloorType,
      sd.ParquetType,
      sd.Notes AS SpaceNotes
    FROM Customer c
    INNER JOIN WorkRequest wr ON c.CustomerID = wr.CustomerID
    LEFT JOIN SpaceDetails sd ON wr.RequestID = sd.RequestID
    WHERE 
      (@CustomerID IS NULL OR c.CustomerID = @CustomerID)
      AND (@City IS NULL OR c.City = @City)
      AND (@FromDate IS NULL OR wr.PlannedDate >= @FromDate)
      AND (@ToDate IS NULL OR wr.PlannedDate <= @ToDate)
      AND (@FloorType IS NULL OR sd.FloorType = @FloorType)
      AND (@Status IS NULL OR wr.Status = @Status)
  END

  --################## עבור כל הלקוחות (תצוגה מקובצת) ##################
  ELSE
  BEGIN
    SELECT 
      c.CustomerID,
      c.FirstName,
      c.LastName,
      c.Phone,
      c.City,
      c.Street,
      c.Number,
      c.Notes,
      c.CreatedAt AS CustomerCreatedAt,
      c.Email,
      wr.RequestID,
      wr.PlannedDate,
      wr.CompletedDate,
      wr.PreferredDate,
      wr.PreferredSlot,
      wr.Status,
      COUNT(sd.SpaceID) AS SpaceCount,
      SUM(sd.Size) AS TotalSpaceSize,
     STUFF((
  SELECT ', ' + sd2.ParquetType
  FROM SpaceDetails sd2
  WHERE sd2.RequestID = wr.RequestID
    AND sd2.ParquetType IS NOT NULL
  FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 2, '') AS FloorTypes

    FROM Customer c
    INNER JOIN WorkRequest wr ON c.CustomerID = wr.CustomerID
    LEFT JOIN SpaceDetails sd ON wr.RequestID = sd.RequestID
    WHERE 
      (@City IS NULL OR c.City = @City)
      AND (@FromDate IS NULL OR wr.PlannedDate >= @FromDate)
      AND (@ToDate IS NULL OR wr.PlannedDate <= @ToDate)
      AND (@FloorType IS NULL OR sd.FloorType = @FloorType)
      AND (@Status IS NULL OR wr.Status = @Status)
      AND c.IsActive = 1
    GROUP BY 
      c.CustomerID, c.FirstName, c.LastName, c.Phone, c.City, c.CreatedAt,
      wr.RequestID, wr.PlannedDate, wr.CompletedDate, wr.PreferredDate, wr.Status,
      c.Email, c.Street, c.Number, c.Notes, wr.PreferredSlot
  END
END
GO
/****** Object:  StoredProcedure [dbo].[GetDashboardSummary]    Script Date: 27/06/2025 15:06:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetDashboardSummary]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        -- סטטוסים שונים
        SUM(CASE WHEN wr.Status = N'ממתין לקביעת תאריך' THEN 1 ELSE 0 END) AS WaitingForDate,
        SUM(CASE WHEN wr.Status = N'תיאום התקנה' THEN 1 ELSE 0 END) AS PendingInstalls,
        SUM(CASE WHEN wr.Status = N'טיוטה להצעת מחיר' THEN 1 ELSE 0 END) AS PendingQuotes,
        SUM(CASE WHEN wr.Status = N'שליחת טיוטה ללקוח' THEN 1 ELSE 0 END) AS PendingApproval,
        SUM(CASE WHEN wr.Status = N'צפייה בסרטון לקוח' THEN 1 ELSE 0 END) AS WaitingForVideo,
        SUM(CASE WHEN wr.Status = N'התקנה בוצעה' THEN 1 ELSE 0 END) AS CompletedInstalls,
        
        -- סטטיסטיקות כלליות
        COUNT(DISTINCT c.CustomerID) AS TotalActiveCustomers,
        COUNT(wr.RequestID) AS TotalActiveRequests,
        
        -- התקנות השבוע
        SUM(CASE 
            WHEN wr.PlannedDate >= DATEADD(week, DATEDIFF(week, 0, GETDATE()), 0)
            AND wr.PlannedDate < DATEADD(week, DATEDIFF(week, 0, GETDATE()) + 1, 0)
            THEN 1 ELSE 0 
        END) AS ThisWeekInstalls,
        
        -- התקנות החודש
        SUM(CASE 
            WHEN wr.PlannedDate >= DATEADD(month, DATEDIFF(month, 0, GETDATE()), 0)
            AND wr.PlannedDate < DATEADD(month, DATEDIFF(month, 0, GETDATE()) + 1, 0)
            THEN 1 ELSE 0 
        END) AS ThisMonthInstalls
        
    FROM Customer c
    INNER JOIN WorkRequest wr ON c.CustomerID = wr.CustomerID
    WHERE c.IsActive = 1
END
GO
/****** Object:  StoredProcedure [dbo].[GetEstimatesByComplexity]    Script Date: 27/06/2025 15:06:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetEstimatesByComplexity]
    @MinComplexity DECIMAL(5,3) = 1.0,
    @MaxComplexity DECIMAL(5,3) = 2.0
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        pe.EstimateID,
        pe.RequestID,
        pe.TotalArea,
        pe.ParquetType,
        pe.RoomCount,
        pe.EstimatedMinPrice,
        pe.EstimatedMaxPrice,
        pe.ComplexityMultiplier,
        pe.Notes,
        c.FirstName + ' ' + c.LastName AS CustomerName,
        c.Phone,
        wr.Status
    FROM PriceEstimates pe
    INNER JOIN WorkRequest wr ON pe.RequestID = wr.RequestID
    INNER JOIN Customer c ON wr.CustomerID = c.CustomerID
    WHERE 
        pe.ComplexityMultiplier BETWEEN @MinComplexity AND @MaxComplexity
        AND c.IsActive = 1
    ORDER BY pe.ComplexityMultiplier DESC, pe.CreatedAt DESC
END
GO
/****** Object:  StoredProcedure [dbo].[GetEstimateStatistics]    Script Date: 27/06/2025 15:06:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetEstimateStatistics]
    @FromDate DATETIME = NULL,
    @ToDate DATETIME = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        COUNT(*) AS TotalEstimates,
        AVG(EstimatedMinPrice) AS AvgMinPrice,
        AVG(EstimatedMaxPrice) AS AvgMaxPrice,
        AVG(EstimatedMinDays) AS AvgMinDays,
        AVG(EstimatedMaxDays) AS AvgMaxDays,
        AVG(TotalArea) AS AvgArea,
        AVG(ComplexityMultiplier) AS AvgComplexity,
        MIN(EstimatedMinPrice) AS MinPrice,
        MAX(EstimatedMaxPrice) AS MaxPrice,
        -- פילוח לפי סוג פרקט
        ParquetType,
        COUNT(*) AS CountByType,
        AVG(BasePrice / TotalArea) AS AvgPricePerM2ByType
    FROM PriceEstimates
    WHERE 
        (@FromDate IS NULL OR CreatedAt >= @FromDate)
        AND (@ToDate IS NULL OR CreatedAt <= @ToDate)
    GROUP BY ParquetType
    ORDER BY CountByType DESC
END
GO
/****** Object:  StoredProcedure [dbo].[GetLatestWorkRequestByCustomerID]    Script Date: 27/06/2025 15:06:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetLatestWorkRequestByCustomerID]
    @CustomerID INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT TOP 1 
        RequestID,
        CustomerID,
        CreatedAt,
        Status,
        Notes,
        PlannedDate,
        CompletedDate,
        PreferredDate,
        PreferredSlot
    FROM WorkRequest 
    WHERE CustomerID = @CustomerID 
    ORDER BY CreatedAt DESC
END
GO
/****** Object:  StoredProcedure [dbo].[GetPriceEstimateByRequestID]    Script Date: 27/06/2025 15:06:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetPriceEstimateByRequestID]
    @RequestID INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        EstimateID,
        RequestID,
        TotalArea,
        ParquetType,
        RoomCount,
        BasePrice,
        EstimatedMinPrice,
        EstimatedMaxPrice,
        EstimatedMinDays,
        EstimatedMaxDays,
        ComplexityMultiplier,
        CreatedAt,
        Notes
    FROM PriceEstimates 
    WHERE RequestID = @RequestID
END
GO
/****** Object:  StoredProcedure [dbo].[GetQuoteDetailsByCustomerID]    Script Date: 27/06/2025 15:06:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetQuoteDetailsByCustomerID]
    @CustomerID INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        wr.RequestID,
        wr.PlannedDate,
        wr.CompletedDate,
        wr.Status,

        q.QuoteID,
        q.TotalPrice,
        q.DiscountAmount,
        q.DiscountPercent,
        q.CreatedAt,

        qi.QuoteItemID,
        qi.CalculatorItemID,
        qi.CustomItemName,
        qi.PriceForItem,
        qi.Quantity,
        qi.FinalPrice,
        qi.Notes

    FROM WorkRequest wr
    INNER JOIN Quote q ON wr.RequestID = q.RequestID
    INNER JOIN QuoteItem qi ON q.QuoteID = qi.QuoteID

    WHERE wr.CustomerID = @CustomerID
      AND q.QuoteID = (
          SELECT TOP 1 qInner.QuoteID
          FROM WorkRequest wrInner
          INNER JOIN Quote qInner ON wrInner.RequestID = qInner.RequestID
          WHERE wrInner.CustomerID = @CustomerID
          ORDER BY qInner.CreatedAt DESC
      )
END
GO
/****** Object:  StoredProcedure [dbo].[GetScheduleSlots]    Script Date: 27/06/2025 15:06:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetScheduleSlots]
    @FromDate DATE,
    @ToDate DATE
AS
BEGIN
    SELECT 
        SlotID,
        [Date],
        Slot,
        Status,
        RequestID
    FROM ScheduleSlot
    WHERE [Date] BETWEEN @FromDate AND @ToDate
    ORDER BY [Date], Slot;
END
GO
/****** Object:  StoredProcedure [dbo].[GetSystemSettings]    Script Date: 27/06/2025 15:06:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetSystemSettings]
    @SettingKey NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    IF @SettingKey IS NOT NULL
    BEGIN
        SELECT 
            SettingID,
            SettingKey,
            SettingValue,
            SettingType,
            Description,
            UpdatedAt
        FROM SystemSettings 
        WHERE SettingKey = @SettingKey
    END
    ELSE
    BEGIN
        SELECT 
            SettingID,
            SettingKey,
            SettingValue,
            SettingType,
            Description,
            UpdatedAt
        FROM SystemSettings 
        ORDER BY SettingKey ASC
    END
END
GO
/****** Object:  StoredProcedure [dbo].[GetUncreatedPopularCandidates]    Script Date: 27/06/2025 15:06:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetUncreatedPopularCandidates]
    @MinOccurrences INT = 3
AS
BEGIN
    SELECT 
        CandidateID,
        CustomItemName,
        SuggestedPrice,
        SuggestedDescription,
        Occurrences
    FROM CalculatorItemCandidates
    WHERE 
        Occurrences >= @MinOccurrences
        AND IsCreated = 0
		AND WasRejected = 0
    ORDER BY Occurrences DESC
END
GO
/****** Object:  StoredProcedure [dbo].[GetWorkRequestByID]    Script Date: 27/06/2025 15:06:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetWorkRequestByID]
    @RequestID INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        RequestID,
        CustomerID,
        CreatedAt,
        Status,
        Notes,
        PlannedDate,
        CompletedDate,
        PreferredDate,
        PreferredSlot
    FROM WorkRequest 
    WHERE RequestID = @RequestID
END
GO
/****** Object:  StoredProcedure [dbo].[GetWorkRequestsByCustomerID]    Script Date: 27/06/2025 15:06:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetWorkRequestsByCustomerID]
    @CustomerID INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        RequestID,
        CustomerID,
        CreatedAt,
        Status,
        Notes,
        PlannedDate,
        CompletedDate,
        PreferredDate,
        PreferredSlot
    FROM WorkRequest 
    WHERE CustomerID = @CustomerID 
    ORDER BY CreatedAt DESC
END
GO
/****** Object:  StoredProcedure [dbo].[GetWorkRequestStatuses]    Script Date: 27/06/2025 15:06:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetWorkRequestStatuses]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        StatusID,
        StatusName,
        StatusOrder,
        IsActive,
        Description,
        CreatedAt
    FROM WorkRequestStatuses 
    WHERE IsActive = 1
    ORDER BY StatusOrder ASC
END
GO
/****** Object:  StoredProcedure [dbo].[InsertCustomer]    Script Date: 27/06/2025 15:06:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Oshri>
-- Create date: <30/03>
-- Description:	<this proc insert new client to the database after register>
-- =============================================
CREATE PROCEDURE [dbo].[InsertCustomer]
    @FirstName NVARCHAR(255),
    @LastName NVARCHAR(255),
    @Phone NVARCHAR(50),
    @Email NVARCHAR(255),
    @City NVARCHAR(255),
    @Street NVARCHAR(255),
    @Number NVARCHAR(50),
    @Notes NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Customer (FirstName, LastName, Phone, Email, City, Street, Number, Notes)
    VALUES (@FirstName, @LastName, @Phone, @Email, @City, @Street, @Number, @Notes);

    -- Return the new CustomerID generated by the identity column
    SELECT CAST(SCOPE_IDENTITY() AS INT) AS NewCustomerID;
END

GO
/****** Object:  StoredProcedure [dbo].[InsertSpaceDetails]    Script Date: 27/06/2025 15:06:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[InsertSpaceDetails]
    @RequestID INT,
    @Size DECIMAL(10,2),
    @FloorType NVARCHAR(100),
    @MediaURL NVARCHAR(255),
    @Notes NVARCHAR(MAX),
	@ParquetType nvarchar(100)
AS
BEGIN
    INSERT INTO SpaceDetails (RequestID, Size, FloorType, MediaURL, Notes,ParquetType)
    VALUES (@RequestID, @Size, @FloorType, @MediaURL, @Notes,@ParquetType)
END
GO
/****** Object:  StoredProcedure [dbo].[InsertWorkRequest]    Script Date: 27/06/2025 15:06:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[InsertWorkRequest]
    @CustomerID INT,
    @PreferredDate DATETIME,
    @PreferredSlot INT,
    @RequestID INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- וידוא שה-CustomerID קיים ופעיל
    IF NOT EXISTS (SELECT 1 FROM Customer WHERE CustomerID = @CustomerID AND IsActive = 1)
    BEGIN
        RAISERROR(N'CustomerID %d לא קיים או לא פעיל בטבלת Customer', 16, 1, @CustomerID)
        RETURN
    END

    -- הכנסת רשומה חדשה
    INSERT INTO WorkRequest (CustomerID, Status, Notes, PreferredDate, PreferredSlot)
    VALUES (@CustomerID, N'צפייה בסרטון לקוח', NULL, @PreferredDate, @PreferredSlot)

    -- קבלת RequestID החדש
    SET @RequestID = SCOPE_IDENTITY()
    
    -- בדיקה שהID נוצר
    IF @RequestID IS NULL OR @RequestID <= 0
    BEGIN
        RAISERROR(N'שגיאה ביצירת RequestID חדש', 16, 1)
        RETURN
    END

    -- החזרת RequestID גם ב-SELECT (עבור ExecuteScalar)
    SELECT @RequestID AS NewRequestID
END
GO
/****** Object:  StoredProcedure [dbo].[ReactivateCustomer]    Script Date: 27/06/2025 15:06:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[ReactivateCustomer]
    @CustomerID INT
AS
BEGIN
    UPDATE Customer
    SET IsActive = 1
    WHERE CustomerID = @CustomerID;
END
GO
/****** Object:  StoredProcedure [dbo].[RejectCalculatorItemCandidate]    Script Date: 27/06/2025 15:06:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[RejectCalculatorItemCandidate]
    @CustomItemName NVARCHAR(255)
AS
BEGIN
    UPDATE CalculatorItemCandidates
    SET WasRejected = 1
    WHERE CustomItemName = @CustomItemName
END
GO
/****** Object:  StoredProcedure [dbo].[SavePriceEstimate]    Script Date: 27/06/2025 15:06:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SavePriceEstimate]
    @RequestID INT,
    @TotalArea DECIMAL(10,2),
    @ParquetType NVARCHAR(100),
    @RoomCount INT,
    @BasePrice DECIMAL(10,2),
    @EstimatedMinPrice DECIMAL(10,2),
    @EstimatedMaxPrice DECIMAL(10,2),
    @EstimatedMinDays INT,
    @EstimatedMaxDays INT,
    @ComplexityMultiplier DECIMAL(5,2),
    @Notes NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    -- בדיקה שה-RequestID קיים
    IF NOT EXISTS (SELECT 1 FROM WorkRequest WHERE RequestID = @RequestID)
    BEGIN
        RAISERROR(N'RequestID %d לא קיים בטבלת WorkRequest', 16, 1, @RequestID)
        RETURN
    END
    
    -- בדיקה אם כבר קיימת הערכה לRequestID זה
    IF EXISTS (SELECT 1 FROM PriceEstimates WHERE RequestID = @RequestID)
    BEGIN
        -- עדכון הערכה קיימת
        UPDATE PriceEstimates 
        SET 
            TotalArea = @TotalArea,
            ParquetType = @ParquetType,
            RoomCount = @RoomCount,
            BasePrice = @BasePrice,
            EstimatedMinPrice = @EstimatedMinPrice,
            EstimatedMaxPrice = @EstimatedMaxPrice,
            EstimatedMinDays = @EstimatedMinDays,
            EstimatedMaxDays = @EstimatedMaxDays,
            ComplexityMultiplier = @ComplexityMultiplier,
            Notes = @Notes,
            CreatedAt = GETDATE()
        WHERE RequestID = @RequestID
        
        -- החזרת EstimateID הקיים
        SELECT EstimateID FROM PriceEstimates WHERE RequestID = @RequestID
    END
    ELSE
    BEGIN
        -- יצירת הערכה חדשה
        INSERT INTO PriceEstimates (
            RequestID, TotalArea, ParquetType, RoomCount, BasePrice,
            EstimatedMinPrice, EstimatedMaxPrice, EstimatedMinDays, EstimatedMaxDays,
            ComplexityMultiplier, Notes
        )
        VALUES (
            @RequestID, @TotalArea, @ParquetType, @RoomCount, @BasePrice,
            @EstimatedMinPrice, @EstimatedMaxPrice, @EstimatedMinDays, @EstimatedMaxDays,
            @ComplexityMultiplier, @Notes
        )
        
        -- החזרת EstimateID החדש
        SELECT SCOPE_IDENTITY() AS EstimateID
    END
END
GO
/****** Object:  StoredProcedure [dbo].[SetCompletedTimeOnStatusChange]    Script Date: 27/06/2025 15:06:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SetCompletedTimeOnStatusChange] 
    @CustomerID INT
AS
BEGIN
    UPDATE WorkRequest
    SET CompletedDate = GETDATE()
    WHERE CustomerID = @CustomerID
      AND Status = N'התקנה בוצעה'
      AND CompletedDate IS NULL;
END
GO
/****** Object:  StoredProcedure [dbo].[UpdateCustomerDetails]    Script Date: 27/06/2025 15:06:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[UpdateCustomerDetails]
    @CustomerID INT,
    @FirstName NVARCHAR(255) = NULL,
    @LastName NVARCHAR(255) = NULL,
    @Phone NVARCHAR(50) = NULL,
    @Email NVARCHAR(255) = NULL,
    @City NVARCHAR(255) = NULL,
    @Street NVARCHAR(255) = NULL,
    @Number NVARCHAR(50) = NULL,
    @Notes NVARCHAR(255) = NULL
AS
BEGIN
    UPDATE Customer
    SET
        FirstName = ISNULL(@FirstName, FirstName),
        LastName = ISNULL(@LastName, LastName),
        Phone = ISNULL(@Phone, Phone),
        Email = ISNULL(@Email, Email),
        City = ISNULL(@City, City),
        Street = ISNULL(@Street, Street),
        Number = ISNULL(@Number, Number),
        Notes = ISNULL(@Notes, Notes)
    WHERE CustomerID = @CustomerID;
END
GO
/****** Object:  StoredProcedure [dbo].[UpdateParquetType]    Script Date: 27/06/2025 15:06:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[UpdateParquetType]
    @ParquetTypeID INT,
    @TypeName NVARCHAR(255) = NULL,
    @PricePerUnit DECIMAL(10, 2) = NULL,
    @ImageURL NVARCHAR(500) = NULL,
	@Type NVARCHAR(255) = NULL
AS
BEGIN
    UPDATE ParquetTypes
    SET 
        TypeName = ISNULL(@TypeName, TypeName),
        PricePerUnit = ISNULL(@PricePerUnit, PricePerUnit),
        ImageURL = ISNULL(@ImageURL, ImageURL),
		[Type] = ISNULL(@Type, [Type])
    WHERE ParquetTypeID = @ParquetTypeID
END
GO
/****** Object:  StoredProcedure [dbo].[UpdatePriceCalculatorItem]    Script Date: 27/06/2025 15:06:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[UpdatePriceCalculatorItem]
    @CalculatorItemID INT,
    @ItemName NVARCHAR(255),
    @Description NVARCHAR(255),
    @Price DECIMAL(10, 2),
	@IsActive BIT
AS
BEGIN
    UPDATE PriceCalculatorItem
    SET 
        ItemName = @ItemName,
        [Description] = @Description,
        Price = @Price,
		IsActive = isnull(@IsActive,1)
    WHERE CalculatorItemID = @CalculatorItemID;
END
GO
/****** Object:  StoredProcedure [dbo].[UpdateSpaceDetails]    Script Date: 27/06/2025 15:06:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[UpdateSpaceDetails]
    @SpaceID INT,
    @RequestID INT = NULL,
    @Size DECIMAL(10,2) = NULL,
    @FloorType NVARCHAR(100) = NULL,
    @MediaURL NVARCHAR(255) = NULL,
    @Notes NVARCHAR(255) = NULL,
    @ParquetType NVARCHAR(100) = NULL
AS
BEGIN
    UPDATE SpaceDetails
    SET
        RequestID = ISNULL(@RequestID, RequestID),
        Size = ISNULL(@Size, Size),
        FloorType = ISNULL(@FloorType, FloorType),
        MediaURL = ISNULL(@MediaURL, MediaURL),
        Notes = ISNULL(@Notes, Notes),
        ParquetType = ISNULL(@ParquetType, ParquetType)
    WHERE SpaceID = @SpaceID;
END
GO
/****** Object:  StoredProcedure [dbo].[UpdateSystemSetting]    Script Date: 27/06/2025 15:06:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[UpdateSystemSetting]
    @SettingKey NVARCHAR(100),
    @SettingValue NVARCHAR(500),
    @SettingType NVARCHAR(50) = 'STRING',
    @Description NVARCHAR(255) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    IF EXISTS (SELECT 1 FROM SystemSettings WHERE SettingKey = @SettingKey)
    BEGIN
        UPDATE SystemSettings 
        SET 
            SettingValue = @SettingValue,
            SettingType = ISNULL(@SettingType, SettingType),
            Description = ISNULL(@Description, Description),
            UpdatedAt = GETDATE()
        WHERE SettingKey = @SettingKey
    END
    ELSE
    BEGIN
        INSERT INTO SystemSettings (SettingKey, SettingValue, SettingType, Description)
        VALUES (@SettingKey, @SettingValue, @SettingType, @Description)
    END
END
GO
/****** Object:  StoredProcedure [dbo].[UpdateWorkRequestStatus]    Script Date: 27/06/2025 15:06:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[UpdateWorkRequestStatus]
    @RequestID INT,
    @NewStatus NVARCHAR(50)
AS
BEGIN
    UPDATE WorkRequest
    SET Status = @NewStatus
    WHERE RequestID = @RequestID
END
GO
/****** Object:  StoredProcedure [dbo].[UpsertCalculatorItemCandidate]    Script Date: 27/06/2025 15:06:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[UpsertCalculatorItemCandidate]
    @CustomItemName NVARCHAR(255),
    @Price DECIMAL(10,2)
AS
BEGIN
--אם כבר קיים נוסיף מופע ונחשב מחדש את ממוצע המחיר
    IF EXISTS (
        SELECT 1 FROM CalculatorItemCandidates WHERE CustomItemName = @CustomItemName
    )
    BEGIN
        UPDATE CalculatorItemCandidates
        SET 
            Occurrences = Occurrences + 1,
            SuggestedPrice = (SuggestedPrice * (Occurrences * 1.0) + @Price) / (Occurrences + 1)
        WHERE CustomItemName = @CustomItemName
    END
    ELSE
	--אם לא קיים ניצור אותו
    BEGIN
        INSERT INTO CalculatorItemCandidates (CustomItemName, SuggestedPrice, SuggestedDescription)
        VALUES (@CustomItemName, @Price, @CustomItemName)
    END
END
GO
USE [master]
GO
ALTER DATABASE [igroup14_prod] SET  READ_WRITE 
GO
