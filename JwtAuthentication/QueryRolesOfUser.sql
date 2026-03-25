select
u.FullName, u.UserName, u.Password,
STRING_AGG(r.name,', ')
from [User] u
join UserRole ur on u.Id = ur.UserId
join Role r on ur.RoleId = r.Id
group by u.id, u.FullName, u.UserName, u.Password
order by u.Id