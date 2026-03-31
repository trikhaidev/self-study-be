INSERT INTO Author (Name, Age)
VALUES 
(N'Nguyễn Văn A', 35),
(N'Trần Thị B', 29),
(N'Lê Văn C', 42);

DECLARE @Author1Id INT;
DECLARE @Author2Id INT;
DECLARE @Author3Id INT;

SELECT @Author1Id = Id FROM Author WHERE Name = N'Nguyễn Văn A';
SELECT @Author2Id = Id FROM Author WHERE Name = N'Trần Thị B';
SELECT @Author3Id = Id FROM Author WHERE Name = N'Lê Văn C';

INSERT INTO Article (Title, Description, AuthorId)
VALUES
(N'Bài viết 1 của A', N'Nội dung bài viết 1', @Author1Id),
(N'Bài viết 2 của A', N'Nội dung bài viết 2', @Author1Id),
(N'Bài viết 3 của A', N'Nội dung bài viết 3', @Author1Id);

INSERT INTO Article (Title, Description, AuthorId)
VALUES
(N'Bài viết 1 của B', N'Nội dung bài viết 1', @Author2Id),
(N'Bài viết 2 của B', N'Nội dung bài viết 2', @Author2Id);