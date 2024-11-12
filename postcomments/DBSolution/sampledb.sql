INSERT INTO Posts (Title, Body)
VALUES 
    ('First Sample Post', 'This is the body of the first sample post.'),
    ('Second Sample Post', 'This is the body of the second sample post.'),
    ('Third Sample Post', 'This is the body of the third sample post.');
INSERT INTO Comments (PostId, CommentText, Author)
VALUES
    (1, 'Great post! Really enjoyed reading it.', 'Alice'),
    (1, 'I have a few questions about this post.', 'Bob'),
    (2, 'This post was very informative, thanks!', 'Charlie'),
    (2, 'Could you provide more details on this topic?', 'Daisy'),
    (3, 'I disagree with some points made in this post.', 'Edward'),
    (3, 'Interesting perspective, thanks for sharing!', 'Fiona');
