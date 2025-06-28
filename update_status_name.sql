-- סקריפט עדכון שם סטטוס מ"תיאום התקנה" ל"תואמה התקנה"
-- הפעל את הסקריפט הזה כדי לעדכן את כל הנתונים הקיימים במסד הנתונים

USE [igroup14_prod]
GO

BEGIN TRANSACTION;

BEGIN TRY
    PRINT 'מתחיל עדכון שם סטטוס מ"תיאום התקנה" ל"תואמה התקנה"...'
    
    -- 1. עדכון טבלת WorkRequestStatus (אם קיימת)
    IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'WorkRequestStatus')
    BEGIN
        UPDATE WorkRequestStatus 
        SET StatusName = N'תואמה התקנה'
        WHERE StatusName = N'תיאום התקנה';
        
        PRINT 'עודכנה טבלת WorkRequestStatus: ' + CAST(@@ROWCOUNT AS VARCHAR(10)) + ' שורות'
    END
    
    -- 2. עדכון טבלת WorkRequest - כל הרשומות עם הסטטוס הישן
    UPDATE WorkRequest 
    SET Status = N'תואמה התקנה'
    WHERE Status = N'תיאום התקנה';
    
    PRINT 'עודכנה טבלת WorkRequest: ' + CAST(@@ROWCOUNT AS VARCHAR(10)) + ' שורות'
    
    -- 3. עדכון טבלת ScheduleSlot (אם קיימת ויש בה סטטוסים)
    IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'ScheduleSlot' AND COLUMN_NAME = 'Status')
    BEGIN
        UPDATE ScheduleSlot 
        SET Status = N'תואמה התקנה'
        WHERE Status = N'תיאום התקנה';
        
        PRINT 'עודכנה טבלת ScheduleSlot: ' + CAST(@@ROWCOUNT AS VARCHAR(10)) + ' שורות'
    END
    
    -- 4. בדיקת תוצאות
    DECLARE @CountAfterUpdate INT
    SELECT @CountAfterUpdate = COUNT(*) 
    FROM WorkRequest 
    WHERE Status = N'תואמה התקנה'
    
    PRINT 'סה"כ רשומות עם הסטטוס החדש "תואמה התקנה": ' + CAST(@CountAfterUpdate AS VARCHAR(10))
    
    -- 5. בדיקה שלא נשארו רשומות עם הסטטוס הישן
    DECLARE @CountOldStatus INT
    SELECT @CountOldStatus = COUNT(*) 
    FROM WorkRequest 
    WHERE Status = N'תיאום התקנה'
    
    IF @CountOldStatus > 0
    BEGIN
        PRINT 'אזהרה: עדיין נמצאו ' + CAST(@CountOldStatus AS VARCHAR(10)) + ' רשומות עם הסטטוס הישן!'
    END
    ELSE
    BEGIN
        PRINT 'מעולה! כל הרשומות עודכנו בהצלחה'
    END
    
    COMMIT TRANSACTION;
    PRINT 'העדכון הושלם בהצלחה!'
    
END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    
    DECLARE @ErrorMessage NVARCHAR(4000);
    DECLARE @ErrorSeverity INT;
    DECLARE @ErrorState INT;
    
    SELECT 
        @ErrorMessage = ERROR_MESSAGE(),
        @ErrorSeverity = ERROR_SEVERITY(),
        @ErrorState = ERROR_STATE();
    
    PRINT 'שגיאה בעדכון: ' + @ErrorMessage;
    RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
END CATCH

-- בדיקה סופית - הצגת כל הסטטוסים הקיימים
PRINT ''
PRINT 'רשימת כל הסטטוסים הקיימים כעת במערכת:'
SELECT 
    Status,
    COUNT(*) as [מספר רשומות]
FROM WorkRequest 
GROUP BY Status
ORDER BY COUNT(*) DESC

PRINT ''
PRINT 'העדכון הושלם!' 