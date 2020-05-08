-- Select daily sales, grouped by product, within from and to date
-- Exclude vouchers and purchases sold by baristas in the POS app
SELECT
	CONVERT(DATE, ph.DateCreated) as 'Date',
	p.Name as 'Product name',
	COUNT(ph.DateCreated) as 'App sales',
	-- Select the product price from Purchase table 
	-- which reflects the price at the time of purchase
	ph.Price as 'Product Price',
	(COUNT(ph.DateCreated) * ph.Price) as 'Subtotal (DKK)'
FROM
	Purchases ph
INNER JOIN Products p ON
	ph.ProductId = p.Id
WHERE
	-- Only completed purchases
	ph.Completed = 1
	-- From Date
	AND ph.DateCreated >= '2018-01-01'
	-- To Date
	AND ph.DateCreated <= '2018-12-31'
	-- Exclude voucher usage
	AND ph.TransactionId NOT LIKE '%voucher%'
	-- Exclude sales from pos app
	AND ph.OrderId NOT LIKE 'Analog'
GROUP BY
	(CONVERT(DATE, ph.DateCreated)),
	p.Name,
	ph.Price
ORDER BY
	(CONVERT(DATE, ph.DateCreated)) ASC
