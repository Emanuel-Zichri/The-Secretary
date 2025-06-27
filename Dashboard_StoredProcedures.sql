-- ============================================
-- Stored Procedures for Dashboard Enhancements
-- Created for "The Secretary" Parquet Installation System
-- ============================================

USE [igroup14_prod]
GO

-- ============================================
-- Get Recent Activity for Dashboard
-- ============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetRecentActivity]') AND type in (N'P'))
DROP PROCEDURE [dbo].[GetRecentActivity]
GO

CREATE PROCEDURE [dbo].[GetRecentActivity]
    @Limit INT = 10
AS
BEGIN
    SET NOCOUNT ON;
    
    -- יצירת CTE לפעילויות שונות
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

-- ============================================
-- Get Upcoming Installs for Dashboard
-- ============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetUpcomingInstalls]') AND type in (N'P'))
DROP PROCEDURE [dbo].[GetUpcomingInstalls]
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

-- ============================================
-- Enhanced Dashboard Summary (if doesn't exist)
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetDashboardSummary]') AND type in (N'P'))
BEGIN
    EXEC('
    CREATE PROCEDURE [dbo].[GetDashboardSummary]
    AS
    BEGIN
        SET NOCOUNT ON;
        
        SELECT 
            -- ממתינים לתאריך
            (SELECT COUNT(*) FROM WorkRequest WHERE Status = ''טיוטה להצעת מחיר'' OR Status = ''שליחת טיוטה ללקוח'') AS WaitingForDate,
            
            -- התקנות בהמתנה
            (SELECT COUNT(*) FROM WorkRequest WHERE Status = ''יתואם'' AND PlannedDate >= GETDATE()) AS PendingInstalls,
            
            -- הצעות בהמתנה
            (SELECT COUNT(*) FROM WorkRequest WHERE Status = ''טיוטה להצעת מחיר'') AS PendingQuotes,
            
            -- ממתינים לאישור
            (SELECT COUNT(*) FROM WorkRequest WHERE Status = ''שליחת טיוטה ללקוח'') AS PendingApproval,
            
            -- ממתינים לסרטון
            (SELECT COUNT(*) FROM WorkRequest WHERE Status = ''צפייה בסרטון לקוח'') AS WaitingForVideo,
            
            -- התקנות שהושלמו (החודש)
            (SELECT COUNT(*) FROM WorkRequest WHERE Status = ''התקנה בוצעה'' AND CompletedDate >= DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1)) AS CompletedInstalls,
            
            -- סה״כ לקוחות פעילים
            (SELECT COUNT(*) FROM Customer WHERE IsActive = 1) AS TotalActiveCustomers,
            
            -- סה״כ בקשות פעילות
            (SELECT COUNT(*) FROM WorkRequest WHERE Status NOT IN (''התקנה בוצעה'', ''בוטל'')) AS TotalActiveRequests,
            
            -- התקנות השבוע
            (SELECT COUNT(*) FROM WorkRequest WHERE PlannedDate >= CAST(GETDATE() AS DATE) AND PlannedDate < DATEADD(day, 7, CAST(GETDATE() AS DATE))) AS ThisWeekInstalls,
            
            -- התקנות החודש
            (SELECT COUNT(*) FROM WorkRequest WHERE PlannedDate >= DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1) AND PlannedDate < DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()) + 1, 1)) AS ThisMonthInstalls;
    END')
END
GO

PRINT 'Dashboard stored procedures created successfully!'
PRINT 'You can now use:'
PRINT '- EXEC GetRecentActivity @Limit = 10'
PRINT '- EXEC GetUpcomingInstalls @Days = 7'
PRINT '- EXEC GetDashboardSummary' 