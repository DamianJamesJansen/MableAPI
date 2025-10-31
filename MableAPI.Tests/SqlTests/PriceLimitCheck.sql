-- SQLite
/*as you can see it only has a few results. I tried >= 1000, but that returned zero results
that was what the expectation was, but it shows the full database if no results found*/
SELECT Id, CatergoryId, Name, DateAdded, Price, Discount, DiscountPrice
FROM Products WHERE DiscountPrice >= 99;