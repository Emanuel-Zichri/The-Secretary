USE [master]
GO
/****** Object:  Database [igroup14_prod]    Script Date: 02/07/2025 09:10:29 ******/
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
/****** Object:  User [sa]    Script Date: 02/07/2025 09:10:29 ******/
CREATE USER [sa] WITHOUT LOGIN WITH DEFAULT_SCHEMA=[sa]
GO
/****** Object:  User [igroup14_DBuser]    Script Date: 02/07/2025 09:10:29 ******/
CREATE USER [igroup14_DBuser] WITHOUT LOGIN WITH DEFAULT_SCHEMA=[dbo]
GO
ALTER ROLE [db_owner] ADD MEMBER [igroup14_DBuser]
GO
/****** Object:  Table [dbo].[CalculatorItemCandidates]    Script Date: 02/07/2025 09:10:29 ******/
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
/****** Object:  Table [dbo].[Customer]    Script Date: 02/07/2025 09:10:29 ******/
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
/****** Object:  Table [dbo].[CustomerFeedback]    Script Date: 02/07/2025 09:10:29 ******/
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
/****** Object:  Table [dbo].[ParquetTypes]    Script Date: 02/07/2025 09:10:29 ******/
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
/****** Object:  Table [dbo].[PriceCalculatorItem]    Script Date: 02/07/2025 09:10:29 ******/
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
/****** Object:  Table [dbo].[PriceEstimates]    Script Date: 02/07/2025 09:10:29 ******/
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
/****** Object:  Table [dbo].[Quote]    Script Date: 02/07/2025 09:10:29 ******/
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
/****** Object:  Table [dbo].[QuoteItem]    Script Date: 02/07/2025 09:10:29 ******/
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
/****** Object:  Table [dbo].[ScheduleSlot]    Script Date: 02/07/2025 09:10:29 ******/
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
/****** Object:  Table [dbo].[SpaceDetails]    Script Date: 02/07/2025 09:10:29 ******/
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
/****** Object:  Table [dbo].[SystemSettings]    Script Date: 02/07/2025 09:10:29 ******/
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
/****** Object:  Table [dbo].[Users]    Script Date: 02/07/2025 09:10:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[UserID] [int] IDENTITY(1,1) NOT NULL,
	[Username] [nvarchar](50) NOT NULL,
	[Password] [nvarchar](255) NOT NULL,
	[BusinessName] [nvarchar](100) NOT NULL,
	[BusinessPhone] [nvarchar](20) NULL,
	[BusinessEmail] [nvarchar](100) NULL,
	[WorkingHours] [nvarchar](200) NULL,
	[AboutUs] [nvarchar](max) NULL,
	[BusinessLogo] [nvarchar](500) NULL,
	[IntroVideoURL] [nvarchar](500) NULL,
	[IsActive] [bit] NULL,
	[CreatedAt] [datetime] NULL,
	[UpdatedAt] [datetime] NULL,
	[LastLogin] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[Username] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[WorkRequest]    Script Date: 02/07/2025 09:10:29 ******/
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
/****** Object:  Table [dbo].[WorkRequestStatuses]    Script Date: 02/07/2025 09:10:29 ******/
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
/****** Object:  Index [IX_Users_IsActive]    Script Date: 02/07/2025 09:10:29 ******/
CREATE NONCLUSTERED INDEX [IX_Users_IsActive] ON [dbo].[Users]
(
	[IsActive] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Users_Username]    Script Date: 02/07/2025 09:10:29 ******/
CREATE NONCLUSTERED INDEX [IX_Users_Username] ON [dbo].[Users]
(
	[Username] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
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
ALTER TABLE [dbo].[Users] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Users] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Users] ADD  DEFAULT (getdate()) FOR [UpdatedAt]
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
ALTER TABLE [dbo].[Users]  WITH CHECK ADD  CONSTRAINT [CHK_Users_Email] CHECK  (([BusinessEmail] like '%@%.%' OR [BusinessEmail] IS NULL))
GO
ALTER TABLE [dbo].[Users] CHECK CONSTRAINT [CHK_Users_Email]
GO
/****** Object:  StoredProcedure [dbo].[AddParquetType]    Script Date: 02/07/2025 09:10:29 ******/
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
/****** Object:  StoredProcedure [dbo].[AddPriceCalculatorItem]    Script Date: 02/07/2025 09:10:29 ******/
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
/****** Object:  StoredProcedure [dbo].[AddQuote]    Script Date: 02/07/2025 09:10:29 ******/
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
/****** Object:  StoredProcedure [dbo].[AddQuoteItem]    Script Date: 02/07/2025 09:10:29 ******/
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
/****** Object:  StoredProcedure [dbo].[AssignRequestToSlot]    Script Date: 02/07/2025 09:10:29 ******/
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
        Status = N'תואמה התקנה'
    WHERE
        RequestID = @RequestID;
END
GO
/****** Object:  StoredProcedure [dbo].[CalculateQuoteTotal]    Script Date: 02/07/2025 09:10:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- 2. Add procedure for calculating quote totals
CREATE   PROCEDURE [dbo].[CalculateQuoteTotal]
    @RequestID INT,
    @IncludeBaseFloor BIT = 1,
    @TotalArea DECIMAL(10,2),
    @FloorPricePerMeter DECIMAL(10,2),
    @DiscountPercent DECIMAL(5,2) = 0,
    @Items NVARCHAR(MAX) -- JSON string with items array
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @VATPercent DECIMAL(5,2) = 18.0; -- Default VAT percentage
    DECLARE @BaseFloorTotal DECIMAL(10,2) = 0;
    DECLARE @ItemsTotal DECIMAL(10,2) = 0;
    DECLARE @Subtotal DECIMAL(10,2);
    DECLARE @DiscountAmount DECIMAL(10,2);
    DECLARE @AfterDiscount DECIMAL(10,2);
    DECLARE @VATAmount DECIMAL(10,2);
    DECLARE @FinalTotal DECIMAL(10,2);
    
    -- Calculate base floor if included
    IF @IncludeBaseFloor = 1
    BEGIN
        SET @BaseFloorTotal = @TotalArea * @FloorPricePerMeter;
    END
    
    -- Parse and calculate items total (simplified - in real implementation you'd parse JSON)
    -- For now, we'll return a structure that the frontend can use
    
    -- Calculate totals
    SET @Subtotal = @BaseFloorTotal + @ItemsTotal;
    SET @DiscountAmount = @Subtotal * (@DiscountPercent / 100.0);
    SET @AfterDiscount = @Subtotal - @DiscountAmount;
    SET @VATAmount = @AfterDiscount * (@VATPercent / 100.0);
    SET @FinalTotal = @AfterDiscount + @VATAmount;
    
    -- Return calculation results
    SELECT 
        @Subtotal AS Subtotal,
        @DiscountPercent AS DiscountPercent,
        @DiscountAmount AS DiscountAmount,
        @AfterDiscount AS AfterDiscount,
        @VATPercent AS VATPercent,
        @VATAmount AS VATAmount,
        @FinalTotal AS FinalTotal,
        @BaseFloorTotal AS BaseFloorTotal;
END
GO
/****** Object:  StoredProcedure [dbo].[CreatePriceCalculatorItemFromCandidate]    Script Date: 02/07/2025 09:10:29 ******/
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
/****** Object:  StoredProcedure [dbo].[DeactivateCustomer]    Script Date: 02/07/2025 09:10:29 ******/
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
/****** Object:  StoredProcedure [dbo].[DeleteParquetType]    Script Date: 02/07/2025 09:10:29 ******/
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
/****** Object:  StoredProcedure [dbo].[DeletePriceCalculatorItem]    Script Date: 02/07/2025 09:10:29 ******/
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
/****** Object:  StoredProcedure [dbo].[DeleteQuote]    Script Date: 02/07/2025 09:10:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Delete a quote
CREATE   PROCEDURE [dbo].[DeleteQuote]
    @QuoteID INT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @DeletedCount INT;
    
    DELETE FROM Quote 
    WHERE QuoteID = @QuoteID;
    
    SET @DeletedCount = @@ROWCOUNT;
    
    -- Return number of deleted quotes (should be 1 or 0)
    SELECT @DeletedCount AS DeletedQuotes;
END
GO
/****** Object:  StoredProcedure [dbo].[DeleteQuoteItems]    Script Date: 02/07/2025 09:10:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ================================================
-- Stored Procedures for Quote Deletion
-- ================================================

-- Delete all quote items for a specific quote
CREATE   PROCEDURE [dbo].[DeleteQuoteItems]
    @QuoteID INT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @DeletedCount INT;
    
    DELETE FROM QuoteItem 
    WHERE QuoteID = @QuoteID;
    
    SET @DeletedCount = @@ROWCOUNT;
    
    -- Return number of deleted items
    SELECT @DeletedCount AS DeletedItems;
END
GO
/****** Object:  StoredProcedure [dbo].[GetAllParquetTypes]    Script Date: 02/07/2025 09:10:29 ******/
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
/****** Object:  StoredProcedure [dbo].[GetAllPriceCalculatorItems]    Script Date: 02/07/2025 09:10:29 ******/
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
/****** Object:  StoredProcedure [dbo].[GetAllPriceEstimates]    Script Date: 02/07/2025 09:10:29 ******/
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
/****** Object:  StoredProcedure [dbo].[GetBIStatistics]    Script Date: 02/07/2025 09:10:29 ******/
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
/****** Object:  StoredProcedure [dbo].[GetCustomerWithSpaceDetails]    Script Date: 02/07/2025 09:10:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[GetCustomerWithSpaceDetails]
    @CustomerID INT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Customer basic info
    SELECT 
        c.CustomerID,
        c.FirstName,
        c.LastName,
        c.Phone,
        c.Email,
        c.City,
        c.Street,
        c.Number,
        c.Notes,
        c.CreatedAt,
        c.IsActive
    FROM Customer c
    WHERE c.CustomerID = @CustomerID AND c.IsActive = 1;
    
    -- Work Request info
    SELECT TOP 1
        wr.RequestID,
        wr.CustomerID,
        wr.Status,
        wr.Notes AS RequestNotes,
        wr.PlannedDate,
        wr.CompletedDate,
        wr.PreferredDate,
        wr.PreferredSlot,
        wr.CreatedAt
    FROM WorkRequest wr
    WHERE wr.CustomerID = @CustomerID
    ORDER BY wr.CreatedAt DESC;
    
    -- Space details
    SELECT 
        sd.SpaceID,
        sd.RequestID,
        sd.Size,
        sd.FloorType,
        sd.ParquetType,
        sd.MediaURL,
        sd.Notes
    FROM SpaceDetails sd
    INNER JOIN WorkRequest wr ON sd.RequestID = wr.RequestID
    WHERE wr.CustomerID = @CustomerID
    ORDER BY sd.SpaceID;
END
GO
/****** Object:  StoredProcedure [dbo].[GetDashboardData]    Script Date: 02/07/2025 09:10:29 ******/
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
      sd.Notes AS SpaceNotes,
	  sd.MediaURL
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
/****** Object:  StoredProcedure [dbo].[GetDashboardSummary]    Script Date: 02/07/2025 09:10:29 ******/
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
/****** Object:  StoredProcedure [dbo].[GetEstimatesByComplexity]    Script Date: 02/07/2025 09:10:29 ******/
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
/****** Object:  StoredProcedure [dbo].[GetEstimateStatistics]    Script Date: 02/07/2025 09:10:29 ******/
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
/****** Object:  StoredProcedure [dbo].[GetItemSuggestions]    Script Date: 02/07/2025 09:10:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetItemSuggestions]
    @SearchText NVARCHAR(255),
    @MaxResults INT = 10
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT DISTINCT
        ItemName as SuggestionText,
        'קבוע' as Source,
        Price as SuggestedPrice,
        1 as Priority
    FROM PriceCalculatorItem 
    WHERE IsActive = 1 
        AND ItemName LIKE '%' + @SearchText + '%'
    
    UNION ALL
    
    SELECT DISTINCT
        CustomItemName as SuggestionText,
        'מועמד' as Source,
        SuggestedPrice,
        2 as Priority
    FROM CalculatorItemCandidates 
    WHERE IsCreated = 0 
        AND WasRejected = 0
        AND CustomItemName LIKE '%' + @SearchText + '%'
        
    
    ORDER BY Priority, SuggestionText
    OFFSET 0 ROWS FETCH NEXT @MaxResults ROWS ONLY
END

GO
/****** Object:  StoredProcedure [dbo].[GetLatestWorkRequestByCustomerID]    Script Date: 02/07/2025 09:10:29 ******/
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
/****** Object:  StoredProcedure [dbo].[GetMonthlyRequestsReport]    Script Date: 02/07/2025 09:10:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetMonthlyRequestsReport]
    @MonthsBack INT = 12
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @StartDate DATE = DATEADD(MONTH, -@MonthsBack, GETDATE());
    DECLARE @EndDate DATE = GETDATE();
    
    WITH MonthlyData AS (
        -- יצירת רשימת חודשים בטווח המבוקש
        SELECT 
            DATEFROMPARTS(YEAR(@StartDate), MONTH(@StartDate), 1) AS MonthStart,
            FORMAT(DATEFROMPARTS(YEAR(@StartDate), MONTH(@StartDate), 1), 'yyyy-MM') AS Month,
            DATENAME(MONTH, DATEFROMPARTS(YEAR(@StartDate), MONTH(@StartDate), 1)) + ' ' + 
            CAST(YEAR(DATEFROMPARTS(YEAR(@StartDate), MONTH(@StartDate), 1)) AS NVARCHAR(4)) AS MonthName
        
        UNION ALL
        
        SELECT 
            DATEADD(MONTH, 1, MonthStart),
            FORMAT(DATEADD(MONTH, 1, MonthStart), 'yyyy-MM'),
            DATENAME(MONTH, DATEADD(MONTH, 1, MonthStart)) + ' ' + 
            CAST(YEAR(DATEADD(MONTH, 1, MonthStart)) AS NVARCHAR(4))
        FROM MonthlyData
        WHERE DATEADD(MONTH, 1, MonthStart) <= @EndDate
    )
    SELECT 
        md.Month,
        md.MonthName,
        COUNT(DISTINCT wr.RequestID) AS NewRequests,
        COUNT(DISTINCT c.CustomerID) AS NewCustomers,
        SUM(CASE WHEN wr.Status = N'התקנה בוצעה' THEN 1 ELSE 0 END) AS CompletedRequests,
        ISNULL(SUM(sd.Size), 0) AS TotalArea
    FROM MonthlyData md
    LEFT JOIN Customer c ON 
        YEAR(c.CreatedAt) = YEAR(md.MonthStart) AND 
        MONTH(c.CreatedAt) = MONTH(md.MonthStart) AND
        c.IsActive = 1
    LEFT JOIN WorkRequest wr ON 
        c.CustomerID = wr.CustomerID AND
        YEAR(wr.CreatedAt) = YEAR(md.MonthStart) AND 
        MONTH(wr.CreatedAt) = MONTH(md.MonthStart)
    LEFT JOIN SpaceDetails sd ON wr.RequestID = sd.RequestID
    GROUP BY md.Month, md.MonthName, md.MonthStart
    ORDER BY md.MonthStart
    OPTION (MAXRECURSION 120); -- מגביל עד 10 שנים
END
GO
/****** Object:  StoredProcedure [dbo].[GetMonthlySalesReport]    Script Date: 02/07/2025 09:10:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetMonthlySalesReport]
    @MonthsBack INT = 12
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @StartDate DATE = DATEADD(MONTH, -@MonthsBack, GETDATE());
    DECLARE @EndDate DATE = GETDATE();
    
    WITH MonthlyData AS (
        -- יצירת רשימת חודשים בטווח המבוקש
        SELECT 
            DATEFROMPARTS(YEAR(@StartDate), MONTH(@StartDate), 1) AS MonthStart,
            FORMAT(DATEFROMPARTS(YEAR(@StartDate), MONTH(@StartDate), 1), 'yyyy-MM') AS Month,
            DATENAME(MONTH, DATEFROMPARTS(YEAR(@StartDate), MONTH(@StartDate), 1)) + ' ' + 
            CAST(YEAR(DATEFROMPARTS(YEAR(@StartDate), MONTH(@StartDate), 1)) AS NVARCHAR(4)) AS MonthName
        
        UNION ALL
        
        SELECT 
            DATEADD(MONTH, 1, MonthStart),
            FORMAT(DATEADD(MONTH, 1, MonthStart), 'yyyy-MM'),
            DATENAME(MONTH, DATEADD(MONTH, 1, MonthStart)) + ' ' + 
            CAST(YEAR(DATEADD(MONTH, 1, MonthStart)) AS NVARCHAR(4))
        FROM MonthlyData
        WHERE DATEADD(MONTH, 1, MonthStart) <= @EndDate
    )
    SELECT 
        md.Month,
        md.MonthName,
        ISNULL(SUM(q.TotalPrice), 0) AS TotalQuotes,
        COUNT(DISTINCT c.CustomerID) AS TotalCustomers,
        SUM(CASE WHEN wr.Status = N'התקנה בוצעה' THEN 1 ELSE 0 END) AS CompletedInstalls,
        ISNULL(SUM(sd.Size), 0) AS TotalArea
    FROM MonthlyData md
    LEFT JOIN Customer c ON 
        YEAR(c.CreatedAt) = YEAR(md.MonthStart) AND 
        MONTH(c.CreatedAt) = MONTH(md.MonthStart) AND
        c.IsActive = 1
    LEFT JOIN WorkRequest wr ON c.CustomerID = wr.CustomerID
    LEFT JOIN Quote q ON wr.RequestID = q.RequestID
    LEFT JOIN SpaceDetails sd ON wr.RequestID = sd.RequestID
    GROUP BY md.Month, md.MonthName, md.MonthStart
    ORDER BY md.MonthStart
    OPTION (MAXRECURSION 120); -- מגביל עד 10 שנים
END
GO
/****** Object:  StoredProcedure [dbo].[GetPriceEstimateByRequestID]    Script Date: 02/07/2025 09:10:29 ******/
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
/****** Object:  StoredProcedure [dbo].[GetQuoteDetails]    Script Date: 02/07/2025 09:10:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- 4. Enhanced procedure for getting quote details
CREATE   PROCEDURE [dbo].[GetQuoteDetails]
    @QuoteID INT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Quote header
    SELECT 
        q.QuoteID,
        q.RequestID,
        q.TotalPrice,
        q.DiscountAmount,
        q.DiscountPercent,
        q.CreatedAt
    FROM Quote q
    WHERE q.QuoteID = @QuoteID;
    
    -- Quote items
    SELECT 
        qi.QuoteItemID,
        qi.QuoteID,
        qi.CalculatorItemID,
        qi.CustomItemName,
        qi.PriceForItem,
        qi.Quantity,
        qi.FinalPrice,
        qi.Notes,
        pci.ItemName,
        pci.Description
    FROM QuoteItem qi
    LEFT JOIN PriceCalculatorItem pci ON qi.CalculatorItemID = pci.CalculatorItemID
    WHERE qi.QuoteID = @QuoteID
    ORDER BY qi.QuoteItemID;
END
GO
/****** Object:  StoredProcedure [dbo].[GetQuoteDetailsByCustomerID]    Script Date: 02/07/2025 09:10:29 ******/
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
    ORDER BY q.CreatedAt DESC, qi.QuoteItemID ASC
END
GO
/****** Object:  StoredProcedure [dbo].[GetQuotesByCustomerID]    Script Date: 02/07/2025 09:10:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- 3. Enhanced procedure for getting quotes by customer
CREATE   PROCEDURE [dbo].[GetQuotesByCustomerID]
    @CustomerID INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        q.QuoteID,
        q.RequestID,
        q.TotalPrice,
        q.DiscountAmount,
        q.DiscountPercent,
        q.CreatedAt,
        COUNT(qi.QuoteItemID) AS ItemCount
    FROM Quote q
    INNER JOIN WorkRequest wr ON q.RequestID = wr.RequestID
    LEFT JOIN QuoteItem qi ON q.QuoteID = qi.QuoteID
    WHERE wr.CustomerID = @CustomerID
    GROUP BY 
        q.QuoteID, q.RequestID, q.TotalPrice, 
        q.DiscountAmount, q.DiscountPercent, q.CreatedAt
    ORDER BY q.CreatedAt DESC;
END
GO
/****** Object:  StoredProcedure [dbo].[GetRecentActivity]    Script Date: 02/07/2025 09:10:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetRecentActivity]
    @Limit INT = 10
AS
BEGIN
    SET NOCOUNT ON;
    ---טבלת CTE
    WITH RecentActivities AS (
        -- התקנות שהושלמו
        SELECT 
            'INSTALL_COMPLETED' AS ActivityType,
            'התקנה הושלמה' AS Description,
            CONCAT(c.FirstName, ' ', c.LastName) AS CustomerName,
            c.City AS Location,
            wr.CompletedDate AS ActivityDate,
            'check' AS IconType,
            'green' AS ColorClass
        FROM WorkRequest wr
        INNER JOIN Customer c ON wr.CustomerID = c.CustomerID
        WHERE wr.CompletedDate IS NOT NULL 
            AND wr.Status = 'התקנה בוצעה'
            AND wr.CompletedDate >= DATEADD(day, -30, GETDATE())
        
        UNION ALL
        
        -- לקוחות חדשים
        SELECT 
            'NEW_CUSTOMER' AS ActivityType,
            'לקוח חדש נרשם' AS Description,
            CONCAT(c.FirstName, ' ', c.LastName) AS CustomerName,
            c.City AS Location,
            c.CreatedAt AS ActivityDate,
            'user-plus' AS IconType,
            'blue' AS ColorClass
        FROM Customer c
        WHERE c.CreatedAt >= DATEADD(day, -30, GETDATE())
            AND c.IsActive = 1
        
        UNION ALL
        
        -- הצעות מחיר שנשלחו
        SELECT 
            'QUOTE_SENT' AS ActivityType,
            'הצעת מחיר נשלחה' AS Description,
            CONCAT(c.FirstName, ' ', c.LastName) AS CustomerName,
            c.City AS Location,
            q.CreatedAt AS ActivityDate,
            'file-text' AS IconType,
            'yellow' AS ColorClass
        FROM Quote q
        INNER JOIN WorkRequest wr ON q.RequestID = wr.RequestID
        INNER JOIN Customer c ON wr.CustomerID = c.CustomerID
        WHERE q.CreatedAt >= DATEADD(day, -30, GETDATE())
        
        UNION ALL
        
        -- תאריכי התקנה שנקבעו
        SELECT 
            'SCHEDULE_SET' AS ActivityType,
            'תאריך התקנה נקבע' AS Description,
            CONCAT(c.FirstName, ' ', c.LastName) AS CustomerName,
            c.City AS Location,
            wr.PlannedDate AS ActivityDate,
            'calendar' AS IconType,
            'purple' AS ColorClass
        FROM WorkRequest wr
        INNER JOIN Customer c ON wr.CustomerID = c.CustomerID
        WHERE wr.PlannedDate IS NOT NULL 
            AND wr.Status = 'יתואם'
            AND wr.PlannedDate >= DATEADD(day, -30, GETDATE())
    )
    
    SELECT TOP (@Limit)
        ActivityType,
        Description,
        CustomerName,
        Location,
        ActivityDate,
        CASE 
            WHEN DATEDIFF(hour, ActivityDate, GETDATE()) < 1 THEN 'לפני פחות משעה'
            WHEN DATEDIFF(hour, ActivityDate, GETDATE()) < 24 THEN CONCAT('לפני ', DATEDIFF(hour, ActivityDate, GETDATE()), ' שעות')
            WHEN DATEDIFF(day, ActivityDate, GETDATE()) = 1 THEN 'אתמול'
            WHEN DATEDIFF(day, ActivityDate, GETDATE()) < 7 THEN CONCAT('לפני ', DATEDIFF(day, ActivityDate, GETDATE()), ' ימים')
            ELSE CONCAT('לפני ', DATEDIFF(week, ActivityDate, GETDATE()), ' שבועות')
        END AS RelativeTime,
        IconType,
        ColorClass
    FROM RecentActivities
    WHERE ActivityDate IS NOT NULL
    ORDER BY ActivityDate DESC;
END
GO
/****** Object:  StoredProcedure [dbo].[GetScheduleSlots]    Script Date: 02/07/2025 09:10:29 ******/
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
/****** Object:  StoredProcedure [dbo].[GetSystemSettings]    Script Date: 02/07/2025 09:10:29 ******/
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
/****** Object:  StoredProcedure [dbo].[GetUncreatedPopularCandidates]    Script Date: 02/07/2025 09:10:29 ******/
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
/****** Object:  StoredProcedure [dbo].[GetUpcomingInstalls]    Script Date: 02/07/2025 09:10:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetUpcomingInstalls]
    @Days INT = 7
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        wr.RequestID,
        CONCAT(c.FirstName, ' ', c.LastName) AS CustomerName,
        c.City AS Location,
        wr.PlannedDate AS InstallDate,
        FORMAT(wr.PlannedDate, 'HH:mm') AS TimeSlot,
        wr.Status,
        CASE 
            WHEN wr.Status = 'יתואם' AND wr.PlannedDate <= DATEADD(day, 1, GETDATE()) THEN 'orange'
            WHEN wr.Status = 'יתואם' THEN 'blue'
            WHEN wr.Status = 'בתהליך התקנה' THEN 'green'
            ELSE 'gray'
        END AS StatusColor,
        CASE 
            WHEN CAST(wr.PlannedDate AS DATE) = CAST(GETDATE() AS DATE) THEN 'היום ' + FORMAT(wr.PlannedDate, 'HH:mm')
            WHEN CAST(wr.PlannedDate AS DATE) = CAST(DATEADD(day, 1, GETDATE()) AS DATE) THEN 'מחר ' + FORMAT(wr.PlannedDate, 'HH:mm')
            WHEN DATEPART(dw, wr.PlannedDate) = 1 THEN 'יום א׳ ' + FORMAT(wr.PlannedDate, 'HH:mm')
            WHEN DATEPART(dw, wr.PlannedDate) = 2 THEN 'יום ב׳ ' + FORMAT(wr.PlannedDate, 'HH:mm')
            WHEN DATEPART(dw, wr.PlannedDate) = 3 THEN 'יום ג׳ ' + FORMAT(wr.PlannedDate, 'HH:mm')
            WHEN DATEPART(dw, wr.PlannedDate) = 4 THEN 'יום ד׳ ' + FORMAT(wr.PlannedDate, 'HH:mm')
            WHEN DATEPART(dw, wr.PlannedDate) = 5 THEN 'יום ה׳ ' + FORMAT(wr.PlannedDate, 'HH:mm')
            WHEN DATEPART(dw, wr.PlannedDate) = 6 THEN 'יום ו׳ ' + FORMAT(wr.PlannedDate, 'HH:mm')
            WHEN DATEPART(dw, wr.PlannedDate) = 7 THEN 'שבת ' + FORMAT(wr.PlannedDate, 'HH:mm')
            ELSE FORMAT(wr.PlannedDate, 'dd/MM') + ' ' + FORMAT(wr.PlannedDate, 'HH:mm')
        END AS FormattedDate
    FROM WorkRequest wr
    INNER JOIN Customer c ON wr.CustomerID = c.CustomerID
    WHERE wr.PlannedDate IS NOT NULL
        AND wr.PlannedDate >= GETDATE()
        AND wr.PlannedDate <= DATEADD(day, @Days, GETDATE())
        AND wr.Status IN ('יתואם', 'בתהליך התקנה')
        AND c.IsActive = 1
    ORDER BY wr.PlannedDate ASC;
END
GO
/****** Object:  StoredProcedure [dbo].[GetVideosByCustomerID]    Script Date: 02/07/2025 09:10:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- ================================================
-- Template generated from Template Explorer using:
-- Create Procedure (New Menu).SQL


CREATE PROCEDURE [dbo].[GetVideosByCustomerID]
    @CustomerID int
AS
BEGIN
    SELECT MediaURL
    FROM Customer c
    INNER JOIN WorkRequest wr ON c.CustomerID = wr.CustomerID
    LEFT JOIN SpaceDetails sd ON wr.RequestID = sd.RequestID
    WHERE c.CustomerID = @CustomerID
    AND MediaURL IS NOT NULL 
    AND MediaURL != ''
    ORDER BY sd.SpaceID
END
GO
/****** Object:  StoredProcedure [dbo].[GetWorkRequestByID]    Script Date: 02/07/2025 09:10:29 ******/
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
/****** Object:  StoredProcedure [dbo].[GetWorkRequestsByCustomerID]    Script Date: 02/07/2025 09:10:29 ******/
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
/****** Object:  StoredProcedure [dbo].[GetWorkRequestStatuses]    Script Date: 02/07/2025 09:10:29 ******/
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
/****** Object:  StoredProcedure [dbo].[InsertCustomer]    Script Date: 02/07/2025 09:10:29 ******/
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
/****** Object:  StoredProcedure [dbo].[InsertSpaceDetails]    Script Date: 02/07/2025 09:10:29 ******/
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
/****** Object:  StoredProcedure [dbo].[InsertWorkRequest]    Script Date: 02/07/2025 09:10:29 ******/
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
/****** Object:  StoredProcedure [dbo].[ReactivateCustomer]    Script Date: 02/07/2025 09:10:29 ******/
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
/****** Object:  StoredProcedure [dbo].[RejectCalculatorItemCandidate]    Script Date: 02/07/2025 09:10:29 ******/
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
/****** Object:  StoredProcedure [dbo].[SavePriceEstimate]    Script Date: 02/07/2025 09:10:29 ******/
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
/****** Object:  StoredProcedure [dbo].[SetCompletedTimeOnStatusChange]    Script Date: 02/07/2025 09:10:29 ******/
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
/****** Object:  StoredProcedure [dbo].[UpdateCustomerDetails]    Script Date: 02/07/2025 09:10:29 ******/
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
/****** Object:  StoredProcedure [dbo].[UpdateCustomerStatus]    Script Date: 02/07/2025 09:10:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[UpdateCustomerStatus]
    @CustomerID INT,
    @NewStatus NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    
    -- עדכון הסטטוס ב-WorkRequest הפעיל האחרון של הלקוח
    UPDATE WorkRequest
    SET Status = @NewStatus
    WHERE CustomerID = @CustomerID
    AND RequestID = (
        SELECT TOP 1 RequestID 
        FROM WorkRequest 
        WHERE CustomerID = @CustomerID 
        ORDER BY CreatedAt DESC
    )
    
    -- אם הסטטוס הוא "התקנה בוצעה" - עדכן תאריך השלמה
    IF @NewStatus = N'התקנה בוצעה'
    BEGIN
        UPDATE WorkRequest
        SET CompletedDate = GETDATE()
        WHERE CustomerID = @CustomerID
        AND Status = N'התקנה בוצעה'
        AND CompletedDate IS NULL
    END
    
    SELECT @@ROWCOUNT AS RowsAffected
END
GO
/****** Object:  StoredProcedure [dbo].[UpdateParquetType]    Script Date: 02/07/2025 09:10:29 ******/
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
/****** Object:  StoredProcedure [dbo].[UpdatePriceCalculatorItem]    Script Date: 02/07/2025 09:10:29 ******/
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
/****** Object:  StoredProcedure [dbo].[UpdateSpaceDetails]    Script Date: 02/07/2025 09:10:29 ******/
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
/****** Object:  StoredProcedure [dbo].[UpdateSystemSetting]    Script Date: 02/07/2025 09:10:29 ******/
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
/****** Object:  StoredProcedure [dbo].[UpdateWorkRequestStatus]    Script Date: 02/07/2025 09:10:29 ******/
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
/****** Object:  StoredProcedure [dbo].[UpsertCalculatorItemCandidate]    Script Date: 02/07/2025 09:10:29 ******/
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
