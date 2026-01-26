-- =============================================
-- Stored Procedure: pr_GetStockViewReportData
-- Description: Get Stock View Report data with Opening Stock (before fromDate) and Production (fromDate to toDate)
-- =============================================

IF OBJECT_ID('pr_GetStockViewReportData', 'P') IS NOT NULL
    DROP PROCEDURE pr_GetStockViewReportData
GO

CREATE PROCEDURE pr_GetStockViewReportData
    @FromDate DATETIME,
    @ToDate DATETIME
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Ensure dates are properly formatted (remove time component)
    SET @FromDate = CAST(CONVERT(VARCHAR(10), @FromDate, 120) AS DATETIME);
    SET @ToDate = CAST(CONVERT(VARCHAR(10), @ToDate, 120) + ' 23:59:59' AS DATETIME);

    -- Get all transaction data with packing master info
    -- Split into Opening (before fromDate) and Production (fromDate to toDate)
    -- NOTE: Use PRODDATE from TRANSACTION_PRODUCT_CALCULATION as the effective production date
    SELECT 
        tpc.PRODDATE AS TRANDATE,
        m.MTRLID AS ProductId,
        m.MTRLDESC AS ProductName,
        packing.PACKMID AS PackingMasterId,
        packing.PACKMDESC AS PackingMasterName,
        tpc.KGWGT,
        tpc.PCKBOX,
        tpc.CALCULATIONMODE,
        ISNULL(grade.GRADEDESC, '') AS GradeName,
        ISNULL(color.PCLRDESC, '') AS ColorName,
        ISNULL(rcvdType.RCVDTDESC, '') AS ReceivedTypeName,
        ISNULL(tm.CATENAME, '') AS SupplierName,
        
        -- PCK columns
        tpc.PCK1, tpc.PCK2, tpc.PCK3, tpc.PCK4, tpc.PCK5, 
        tpc.PCK6, tpc.PCK7, tpc.PCK8, tpc.PCK9, tpc.PCK10,
        tpc.PCK11, tpc.PCK12, tpc.PCK13, tpc.PCK14, tpc.PCK15,
        tpc.PCK16, tpc.PCK17,
        
        -- Date category: 0 = Opening (before fromDate), 1 = Production (fromDate to toDate)
        CASE 
            WHEN tpc.PRODDATE < @FromDate THEN 0  -- Opening Stock
            WHEN tpc.PRODDATE >= @FromDate AND tpc.PRODDATE <= @ToDate THEN 1  -- Production
            ELSE 2  -- Exclude (after toDate, should not happen)
        END AS DateCategory
    FROM 
        TRANSACTION_PRODUCT_CALCULATION tpc
        INNER JOIN TRANSACTIONDETAIL td ON tpc.TRANDID = td.TRANDID
        INNER JOIN MATERIALMASTER m ON td.MTRLID = m.MTRLID
        INNER JOIN TRANSACTIONMASTER tm ON td.TRANMID = tm.TRANMID
        INNER JOIN PACKINGMASTER packing ON tpc.PACKMID = packing.PACKMID
        LEFT JOIN GRADEMASTER grade ON tpc.GRADEID = grade.GRADEID
        LEFT JOIN PRODUCTIONCOLOURMASTER color ON tpc.PCLRID = color.PCLRID
        LEFT JOIN RECEIVEDTYPEMASTER rcvdType ON tpc.RCVDTID = rcvdType.RCVDTID
    WHERE 
        (tpc.DISPSTATUS = 0 OR tpc.DISPSTATUS IS NULL)
        AND (m.DISPSTATUS = 0 OR m.DISPSTATUS IS NULL)
        AND (tm.DISPSTATUS = 0 OR tm.DISPSTATUS IS NULL)
        AND tpc.PRODDATE <= @ToDate  -- Include all data up to toDate (by production date)
    ORDER BY 
        packing.PACKMID,
        m.MTRLID,
        tpc.KGWGT,
        grade.GRADEDESC,
        color.PCLRDESC,
        rcvdType.RCVDTDESC,
        tpc.PRODDATE
END
GO

PRINT 'Stored procedure pr_GetStockViewReportData created successfully'
GO
