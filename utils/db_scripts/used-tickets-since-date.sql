-- Used tickets since date
-- - t.DateUsed YYYY-MM-DD
SELECT t.Id AS 'Ticket Id',
    t.DateUsed AT TIME ZONE 'UTC' AT TIME ZONE 'W. Europe Standard Time' AS 'Ticket Usage DateTime EST',
    t.ProductId AS 'Product Id',
    p.Name AS 'Product Name'
FROM Tickets t
    RIGHT JOIN Products p ON t.ProductId = p.Id
WHERE t.IsUsed = 1
    AND t.DateUsed > '2022-01-01'