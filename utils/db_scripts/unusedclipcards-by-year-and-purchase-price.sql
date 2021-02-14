-- Unused clips cards and their net worth
-- 
-- - Group by purchase year
-- - The value is calculated based on the product at the time of purchase

SELECT
	YEAR(t.DateCreated) AS 'Purchase Year',
	p.Id  as 'ProductId',
	p.Name AS 'Product',
	Count(t.Id) AS 'Unused tickets',
	(ph.Price / ph.NumberOfTickets) AS 'Per ticket incl moms', -- Product price as the time of purchase
	(ph.Price / ph.NumberOfTickets) * COUNT(t.Id) AS 'Subtotal incl moms' -- Subtotal of unused tickets with purchase price
FROM
	Tickets t
INNER JOIN Purchases ph ON
	ph.Id = t.Purchase_Id
INNER JOIN Products p ON
	t.ProductId = p.Id
WHERE
	ph.Completed = 1 -- Only look at completed Purchases
	AND t.IsUsed = 0 -- and unused tickets
	AND ph.DateCreated > '2017-12-01 00:00' -- fromDate exclusive (only look at unused tickets after this date)
GROUP BY
	p.Id,
	p.Name,
	YEAR(t.DateCreated),
	(ph.Price / ph.NumberOfTickets)
ORDER BY
	YEAR(t.DateCreated)