USE [CHAKRA_ERP]
GO

TRUNCATE TABLE LOANMASTER

SET IDENTITY_INSERT [dbo].[LOANMASTER] ON 
GO
INSERT INTO [dbo].[LOANMASTER]
           (LOANMID, [LOANDATE]
           ,[COMPYID]
           ,[BRNCHID]
           ,[LOANNO]
           ,[DLOANNO]
           ,[CATEID]
           ,[CATENAME]
           ,[CATEADDR]
           ,[CATEPHN1]
           ,[CATEPHN2]
           ,[LOANGNAME]
           ,[LOANGADDR]
           ,[LOANGPHN1]
           ,[LOANGPHN2]
           ,[BRNDID]
           ,[MDLID]
           ,[CLRID]
           ,[VHLNO]
           ,[ENGNO]
           ,[KEYNO]
           ,[CHANO]
           ,[VHLAMT]
           ,[ITLAMT]
           ,[LOANAMT]
           ,[SCHMID]
           ,[LOANRATE]
           ,[LOANDCHRG]
           ,[LOANSDATE]
           ,[LOANEDATE]
           ,[EMIAMT]
           ,[INSID]
           ,[INSPLYNO]
           ,[INSDATE]
           ,[INSAMT]
           ,[RNWDATE]
           ,[RNWAMT]
           ,[DLRID]
           ,[SDLRID]
           ,[SDLRNAME]
           ,[INVNO]
           ,[INVDATE]
           ,[INVAMT]
           ,[LOANPDTYPE]
           ,[LOANTID]
           ,[CONSMNO]
           ,[CUSRID]
           ,[LMUSRID]
           ,[DISPSTATUS]
           ,[LOAN_CATE_CODE]
           ,[PRCSDATE])
     sELECT  LOANMID, [LOANDATE]
           ,LOANMASTER_29042024.COMPYID
           ,LOANMASTER_29042024.BRNCHID
           ,[LOANNO]
           ,[DLOANNO]
           ,CUSTOMERMASTER.CATEID
           ,LOANMASTER_29042024.CATENAME
           ,LOANMASTER_29042024.CATEADDR
           ,LOANMASTER_29042024.CATEPHN1
           ,LOANMASTER_29042024.CATEPHN2
           ,[LOANGNAME]
           ,[LOANGADDR]
           ,[LOANGPHN1]
           ,[LOANGPHN2]
           ,[BRNDID]
           ,[MDLID]
           ,[CLRID]
           ,[VHLNO]
           ,[ENGNO]
           ,[KEYNO]
           ,[CHANO]
           ,[VHLAMT]
           ,[ITLAMT]
           ,[LOANAMT]
           ,[SCHMID]
           ,[LOANRATE]
           ,[LOANDCHRG]
           ,[LOANSDATE]
           ,[LOANEDATE]
           ,[EMIAMT]
           ,[INSID]
           ,[INSPLYNO]
           ,[INSDATE]
           ,[INSAMT]
           ,[RNWDATE]
           ,[RNWAMT]
           ,[DLRID]
           ,[SDLRID]
           ,[SDLRNAME]
           ,[INVNO]
           ,[INVDATE]
           ,[INVAMT]
           ,[LOANPDTYPE]
           ,[LOANTID]
           ,[CONSMNO]
           ,'admin'
           ,1
           ,LOANMASTER_29042024.DISPSTATUS
           ,[CATECODE]
           ,GETDATE()
		   FROM LOANMASTER_29042024
		   INNER JOIN CUSTOMERMASTER ON LOANMASTER_29042024.CATEID = CUSTOMERMASTER.CATEID
SET IDENTITY_INSERT [dbo].[LOANMASTER] OFF
GO
GO


