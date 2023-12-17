CREATE PROCEDURE CalculateSumAndMedian
AS
BEGIN
    DECLARE @TotalSum BIGINT;
    DECLARE @Median FLOAT;

    SELECT @TotalSum = SUM([Integer]) FROM [Data];

    WITH Ordered AS (
        SELECT [Double], 
               ROW_NUMBER() OVER (ORDER BY [Double]) AS RowNum,
               COUNT(*) OVER () AS TotalCount
        FROM [Data]
    )
    SELECT @Median = AVG(CAST([Double] AS FLOAT))
    FROM Ordered
    WHERE RowNum IN ((TotalCount + 1) / 2, (TotalCount + 2) / 2);

    SELECT @TotalSum AS TotalIntegerSum, @Median AS DoubleMedian;
END;

EXEC CalculateSumAndMedian;
