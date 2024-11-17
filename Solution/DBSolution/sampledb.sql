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
('Travel Diaries', 'My experience exploring the Himalayan trails.', 2),
('Cricket: Game Strategy', 'An analysis of India’s performance in the last ODI series.', 3),
('Spicy Indian Recipes', 'Authentic recipes for popular Indian dishes like Biryani and Butter Chicken.', 4);


INSERT INTO Comments (PostId, UserId, Content) VALUES
    (1, 1, 'This was super helpful! Can you elaborate on funding sources?'),
    (1, 2, 'Loved this! Especially the part about market research.'),
    (2, 3, 'I’ve been to the Himalayas too. Your tips were spot on!'),
    (2, 4, 'Planning my trek next month. Thanks for sharing this.'),
    (3, 5, 'Great analysis! Do you think the team should have tried a different bowling strategy?'),
    (3, 6, 'I think fielding was the real issue in that match.'),
    (4, 7, 'This Butter Chicken recipe turned out amazing! Thanks.'),
    (4, 8, 'Loved the Biryani recipe. Can you share more vegetarian options?');
