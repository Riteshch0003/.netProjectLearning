-- Insert sample data into Users table

INSERT INTO Users (Username, Password, Email) VALUES
('Alice', 'password_1', 'alice@example.com'),
('Bob', 'password_2', 'bob@example.com'),
('Charlie', 'password_3', 'charlie@example.com'),
('Daisy', 'password_4', 'daisy@example.com'),
('Edward', 'password_5', 'edward@example.com'),
('Fiona', 'password_6', 'fiona@example.com');


INSERT INTO Posts (Title, Content, UserId) VALUES
('First Post', 'This is the content of the first post.', 1),
('Second Post', 'Here is the second post content.', 2),
('Another Post by John', 'John’s additional post content.', 1),
('Alice’s First Post', 'Content for Alice’s first post.', 3);


INSERT INTO Comments (PostId, UserId, Author, Content) VALUES
    (1, 1, 'Alice', 'Great post! Really enjoyed reading it.'),
    (1, 2, 'Bob', 'I have a few questions about this post.'),
    (2, 3, 'Charlie', 'This post was very informative, thanks!'),
    (2, 4, 'Daisy', 'Could you provide more details on this topic?'),
    (3, 5, 'Edward', 'I disagree with some points made in this post.'),
    (3, 6, 'Fiona', 'Interesting perspective, thanks for sharing!');
