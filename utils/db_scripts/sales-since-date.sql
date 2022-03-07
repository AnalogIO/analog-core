-- Completed purchases (sales) since date
-- - ph.DateCreated YYYY-MM-DD
SELECT ph.Id AS 'Purchase Id',
    ph.DateCreated AT TIME ZONE 'UTC' AT TIME ZONE 'W. Europe Standard Time' AS 'Purchase DateTime EST',
    ph.ProductId AS 'Product Id',
    p.Name AS 'Product Name',
    ph.NumberOfTickets AS 'Tickets',
    ph.Price AS 'Purchase Price'
FROM Purchases ph
    RIGHT JOIN Products p ON ph.ProductId = p.Id
WHERE ph.Completed = 1
    -- YYYY-MM-DD 
    AND ph.DateCreated > '2022-01-01'