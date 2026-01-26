CREATE PROCEDURE [dbo].[pr_IRN_Transaction_Update_Assgn] @PTranMID int, @PIRNNO varchar(100), @PACKNO varchar(50),    
@PACKDT DATETIME, @PCUSRID VARCHAR(100)    
AS    
BEGIN    
    
 SET NOCOUNT ON;    
    
    -- Insert statements for procedure here    
 Update TRANSACTIONMASTER Set    
 IRNNO = @PIRNNO,    
 ACKNO = @PACKNO,    
 ACKDT = @PACKDT,    
 EINV_UPLOADEDBY = @PCUSRID,    
 EINV_UPLOADED_DATETIME = getdate()    
 Where TRANMID = @PTranMID    
    
    
END    
    