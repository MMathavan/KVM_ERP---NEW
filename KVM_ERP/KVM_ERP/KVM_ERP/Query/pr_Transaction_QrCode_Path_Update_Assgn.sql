CREATE PROCEDURE pr_Transaction_QrCode_Path_Update_Assgn @PTranMID int, @PPath varchar(max)    
AS    
BEGIN    
    
 SET NOCOUNT ON;    
    
 Update TRANSACTIONMASTER Set QRCODEPATH = @PPath Where TRANMID = @PTranMID    
    
END    