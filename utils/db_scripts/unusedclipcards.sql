-- Count unused clip cards, grouped by product, created after date
SELECT
	p.Name, 
	COUNT(*)
FROM
	Tickets t
INNER JOIN Products p ON
	t.ProductId = p.Id
WHERE
	t.DateCreated >= '2019-01-01'
	AND t.IsUsed = 0
GROUP BY
	p.Name