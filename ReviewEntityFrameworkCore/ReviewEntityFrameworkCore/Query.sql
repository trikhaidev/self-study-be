SELECT 
    a.Id,
    a.Name,
    COUNT(ar.Id) AS TotalArticles
FROM Author a
LEFT JOIN Article ar ON a.Id = ar.AuthorId
GROUP BY a.Id, a.Name;

select * from Article