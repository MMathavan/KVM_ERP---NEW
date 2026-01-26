-- =============================================
-- Stored Procedure: pr_GetAdminDashboardStats
-- Description: Get Admin Dashboard statistics for all transaction menus
-- =============================================

IF OBJECT_ID('pr_GetAdminDashboardStats', 'P') IS NOT NULL
    DROP PROCEDURE pr_GetAdminDashboardStats
GO

CREATE PROCEDURE pr_GetAdminDashboardStats
AS
BEGIN
    SET NOCOUNT ON;

    -- ========================================
    -- 1. SHRIMP TYPES (Material Master)
    -- ========================================
    SELECT 
        'ShrimpTypes' AS StatType,
        COUNT(DISTINCT MTRLID) AS TotalCount,
        STRING_AGG(MTRLDESC, ', ') AS Details
    FROM MATERIALMASTER
    WHERE (DISPSTATUS = 0 OR DISPSTATUS IS NULL)
    
    UNION ALL
    
    -- ========================================
    -- 2. RAW MATERIAL INVOICES - TOTAL
    -- ========================================
    SELECT 
        'TotalInvoices' AS StatType,
        COUNT(*) AS TotalCount,
        CAST(SUM(ISNULL(TRANNAMT, 0)) AS NVARCHAR(MAX)) AS Details
    FROM TRANSACTIONMASTER
    WHERE REGSTRID = 2  -- Raw Material Invoices
        AND (DISPSTATUS = 0 OR DISPSTATUS IS NULL)
    
    UNION ALL
    
    -- ========================================
    -- 3. RAW MATERIAL INVOICES - WAITING FOR APPROVAL
    -- ========================================
    SELECT 
        'InvoicesWaitingApproval' AS StatType,
        COUNT(*) AS TotalCount,
        CAST(SUM(ISNULL(TRANNAMT, 0)) AS NVARCHAR(MAX)) AS Details
    FROM TRANSACTIONMASTER tm
    LEFT JOIN TRANSACTIONMASTERFACTOR tmf ON tm.TRANMID = tmf.TRANMID
    WHERE tm.REGSTRID = 2  -- Raw Material Invoices
        AND (tm.DISPSTATUS = 0 OR tm.DISPSTATUS IS NULL)
        AND (tm.TRANAPPROVAL IS NULL OR tm.TRANAPPROVAL = 0)  -- Not approved
    
    UNION ALL
    
    -- ========================================
    -- 4. RAW MATERIAL INVOICES - APPROVED
    -- ========================================
    SELECT 
        'InvoicesApproved' AS StatType,
        COUNT(*) AS TotalCount,
        CAST(SUM(ISNULL(TRANNAMT, 0)) AS NVARCHAR(MAX)) AS Details
    FROM TRANSACTIONMASTER
    WHERE REGSTRID = 2  -- Raw Material Invoices
        AND (DISPSTATUS = 0 OR DISPSTATUS IS NULL)
        AND TRANAPPROVAL = 1  -- Approved
    
    UNION ALL
    
    -- ========================================
    -- 5. RAW MATERIAL INTAKE TRANSACTIONS
    -- ========================================
    SELECT 
        'RawMaterialIntake' AS StatType,
        COUNT(*) AS TotalCount,
        CAST(SUM(ISNULL(TRANNAMT, 0)) AS NVARCHAR(MAX)) AS Details
    FROM TRANSACTIONMASTER
    WHERE REGSTRID = 1  -- Raw Material Intake
        AND (DISPSTATUS = 0 OR DISPSTATUS IS NULL)
    
    UNION ALL
    
    -- ========================================
    -- 6. TOTAL SUPPLIERS
    -- ========================================
    SELECT 
        'TotalSuppliers' AS StatType,
        COUNT(*) AS TotalCount,
        '' AS Details
    FROM SUPPLIERMASTER
    WHERE (DISPSTATUS = 0 OR DISPSTATUS IS NULL)
    
    UNION ALL
    
    -- ========================================
    -- 7. TOTAL CUSTOMERS
    -- ========================================
    SELECT 
        'TotalCustomers' AS StatType,
        COUNT(*) AS TotalCount,
        '' AS Details
    FROM CUSTOMERMASTER
    WHERE (DISPSTATUS = 0 OR DISPSTATUS IS NULL)
    
    UNION ALL
    
    -- ========================================
    -- 8. THIS MONTH INVOICES
    -- ========================================
    SELECT 
        'ThisMonthInvoices' AS StatType,
        COUNT(*) AS TotalCount,
        CAST(SUM(ISNULL(TRANNAMT, 0)) AS NVARCHAR(MAX)) AS Details
    FROM TRANSACTIONMASTER
    WHERE REGSTRID = 2  -- Raw Material Invoices
        AND (DISPSTATUS = 0 OR DISPSTATUS IS NULL)
        AND MONTH(TRANDATE) = MONTH(GETDATE())
        AND YEAR(TRANDATE) = YEAR(GETDATE())
    
    UNION ALL
    
    -- ========================================
    -- 9. TODAY'S TRANSACTIONS
    -- ========================================
    SELECT 
        'TodayTransactions' AS StatType,
        COUNT(*) AS TotalCount,
        '' AS Details
    FROM TRANSACTIONMASTER
    WHERE (DISPSTATUS = 0 OR DISPSTATUS IS NULL)
        AND CAST(TRANDATE AS DATE) = CAST(GETDATE() AS DATE)
    
    UNION ALL
    
    -- ========================================
    -- 10. TOTAL STOCK VALUE (Product Calculations)
    -- ========================================
    SELECT 
        'TotalStockValue' AS StatType,
        COUNT(DISTINCT TRANPID) AS TotalCount,
        '' AS Details
    FROM TRANSACTION_PRODUCT_CALCULATION
    WHERE (DISPSTATUS = 0 OR DISPSTATUS IS NULL);

    -- ========================================
    -- SHRIMP TYPES BY RECEIVED TYPE (for chart)
    -- ========================================
    SELECT 
        ISNULL(rt.RCVDTDESC, 'Unknown') AS ReceivedType,
        COUNT(DISTINCT tpc.TRANPID) AS Count
    FROM TRANSACTION_PRODUCT_CALCULATION tpc
    LEFT JOIN RECEIVEDTYPEMASTER rt ON tpc.RCVDTID = rt.RCVDTID
    WHERE (tpc.DISPSTATUS = 0 OR tpc.DISPSTATUS IS NULL)
    GROUP BY rt.RCVDTDESC
    ORDER BY Count DESC;

    -- ========================================
    -- MONTHLY INVOICE TREND (Last 6 months)
    -- ========================================
    WITH MonthlyData AS (
        SELECT 
            DATENAME(MONTH, TRANDATE) AS MonthName,
            MONTH(TRANDATE) AS MonthNum,
            COUNT(*) AS InvoiceCount
        FROM TRANSACTIONMASTER
        WHERE REGSTRID = 2  -- Raw Material Invoices
            AND (DISPSTATUS = 0 OR DISPSTATUS IS NULL)
            AND TRANDATE >= DATEADD(MONTH, -6, GETDATE())
        GROUP BY DATENAME(MONTH, TRANDATE), MONTH(TRANDATE)
    )
    SELECT 
        MonthName,
        InvoiceCount
    FROM MonthlyData
    ORDER BY MonthNum;

    -- ========================================
    -- TOP 5 SHRIMP TYPES BY QUANTITY
    -- ========================================
    SELECT TOP 5
        m.MTRLDESC AS ShrimpType,
        COUNT(tpc.TRANPID) AS Transactions,
        SUM(tpc.PCK1 + tpc.PCK2 + tpc.PCK3 + tpc.PCK4 + tpc.PCK5 + 
            tpc.PCK6 + tpc.PCK7 + tpc.PCK8 + tpc.PCK9 + tpc.PCK10 + 
            tpc.PCK11 + tpc.PCK12 + tpc.PCK13 + tpc.PCK14 + tpc.PCK15 + 
            tpc.PCK16 + tpc.PCK17) AS TotalQuantity
    FROM TRANSACTION_PRODUCT_CALCULATION tpc
    INNER JOIN TRANSACTIONDETAIL td ON tpc.TRANDID = td.TRANDID
    INNER JOIN MATERIALMASTER m ON td.MTRLID = m.MTRLID
    WHERE (tpc.DISPSTATUS = 0 OR tpc.DISPSTATUS IS NULL)
        AND (m.DISPSTATUS = 0 OR m.DISPSTATUS IS NULL)
    GROUP BY m.MTRLDESC
    ORDER BY TotalQuantity DESC;

END
GO

PRINT 'Stored procedure pr_GetAdminDashboardStats created successfully'
GO
