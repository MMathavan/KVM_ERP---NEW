-- =============================================
-- Stored Procedure: pr_GetStockViewReportData
-- Description: Get Stock View Report data with Opening Stock (before fromDate) and Production (fromDate to toDate)
-- =============================================

IF OBJECT_ID('dbo.pr_GetStockViewReportData', 'P') IS NOT NULL
	DROP PROCEDURE dbo.pr_GetStockViewReportData
GO

CREATE PROCEDURE [dbo].[pr_GetStockViewReportData]
	@FromDate DATETIME,
	@ToDate DATETIME
AS
BEGIN
	SET NOCOUNT ON;
	
	-- Ensure dates are properly formatted (remove time component)
	SET @FromDate = CAST(CONVERT(VARCHAR(10), @FromDate, 120) AS DATETIME);
	SET @ToDate   = CAST(CONVERT(VARCHAR(10), @ToDate,   120) + ' 23:59:59' AS DATETIME);

	;WITH PackingTypes AS (
		SELECT
			pt.PACKMID,
			pt.PACKTMID,
			pt.PACKTMDESC,
			pt.PACKTMCODE,
			ROW_NUMBER() OVER (
				PARTITION BY pt.PACKMID
				ORDER BY pt.PACKTMCODE
			) AS PackIndex
		FROM PACKINGTYPEMASTER pt
		WHERE (pt.DISPSTATUS = 0 OR pt.DISPSTATUS IS NULL)
			AND UPPER(ISNULL(pt.PACKTMDESC, '')) NOT LIKE '%BKN%'
			AND UPPER(ISNULL(pt.PACKTMDESC, '')) NOT LIKE '%BROKEN%'
			AND UPPER(ISNULL(pt.PACKTMDESC, '')) NOT LIKE '%OTHERS%'
			AND UPPER(ISNULL(pt.PACKTMDESC, '')) NOT LIKE '%OTHER%'
	)
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
		pt.PackIndex,
		pt.PACKTMDESC AS SizeName,
		
		-- Date category: 0 = Opening (before fromDate), 1 = Production (fromDate to toDate)
		CASE 
			WHEN tpc.PRODDATE < @FromDate THEN 0  -- Opening Stock
			WHEN tpc.PRODDATE >= @FromDate AND tpc.PRODDATE <= @ToDate THEN 1  -- Production
			ELSE 2  -- Exclude (after toDate, should not happen)
		END AS DateCategory,
		
		-- Slab value for this specific size (one row per size)
		tpc.SLABVALUE AS SlabValue
	FROM 
		TRANSACTION_PRODUCT_CALCULATION tpc
		INNER JOIN TRANSACTIONDETAIL td ON tpc.TRANDID = td.TRANDID
		INNER JOIN MATERIALMASTER m ON td.MTRLID = m.MTRLID
		INNER JOIN TRANSACTIONMASTER tm ON td.TRANMID = tm.TRANMID
		INNER JOIN PACKINGMASTER packing ON tpc.PACKMID = packing.PACKMID
		LEFT JOIN GRADEMASTER grade ON tpc.GRADEID = grade.GRADEID
		LEFT JOIN PRODUCTIONCOLOURMASTER color ON tpc.PCLRID = color.PCLRID
		LEFT JOIN RECEIVEDTYPEMASTER rcvdType ON tpc.RCVDTID = rcvdType.RCVDTID
		INNER JOIN PackingTypes pt ON pt.PACKMID = tpc.PACKMID AND pt.PACKTMID = tpc.PACKTMID
	WHERE 
		(tpc.DISPSTATUS = 0 OR tpc.DISPSTATUS IS NULL)
		AND (m.DISPSTATUS = 0 OR m.DISPSTATUS IS NULL)
		AND (tm.DISPSTATUS = 0 OR tm.DISPSTATUS IS NULL)
		AND tpc.PRODDATE <= @ToDate  -- Include all data up to toDate (by production date)
		AND tpc.SLABVALUE > 0        -- Only rows with real slab data
		AND tpc.PACKTMID <> 0        -- Ignore header rows; slab rows carry PACKTMID
	ORDER BY 
		packing.PACKMID,
		m.MTRLID,
		tpc.KGWGT,
		grade.GRADEDESC,
		color.PCLRDESC,
		rcvdType.RCVDTDESC,
		pt.PackIndex,
		tpc.PRODDATE
END
GO

PRINT 'Stored procedure pr_GetStockViewReportData created successfully'
GO
