-- Insert sample data into Users table

-- INSERT INTO Users (Username, Password, Email) VALUES
-- ('Ananya', 'hashed_password1', 'ananya@example.com'),
-- ('Ajinkya', 'hashed_password2', 'ajinkya@example.com'),
-- ('Ritesh', 'hashed_password3', 'ritesh@example.com'),
-- ('Arjun', 'hashed_password4', 'arjun@example.com'),
-- ('Priya', 'hashed_password5', 'priya@example.com'),
-- ('Aditya', 'hashed_password6', 'aditya@example.com'),
-- ('Neha', 'hashed_password7', 'neha@example.com'),
-- ('Rutuja', 'hashed_password8', 'rutuja@example.com');



INSERT INTO Posts (Title, Content, UserId) VALUES
('Startup Insights', 'This post shares insights on how to bootstrap a startup effectively.', 1),
('Travel Diaries', 'My experience exploring the Himalayan trails.', 2);



INSERT INTO Comments (PostId, UserId,Author, Content) VALUES
    (1, 1,'Ajinkya', 'This was super helpful! Can you elaborate on funding sources?'),
    (1, 2,'Ritesh', 'Loved this! Especially the part about market research.');
   