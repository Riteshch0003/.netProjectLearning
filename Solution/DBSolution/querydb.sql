SELECT * FROM Users;

SELECT * FROM Posts;

SELECT * FROM Comments;


INSERT INTO Posts (Title, Content, UserId) VALUES
('Travel Diaries', 'My experience exploring the Himalayan trails..', 2);



INSERT INTO Comments (PostId, UserId, Content) VALUES
    (2, 2,'Loved this! Especially the part about market research.');

