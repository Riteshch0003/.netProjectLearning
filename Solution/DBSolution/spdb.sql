DELIMITER //

CREATE PROCEDURE InsertPost(
    IN p_Title VARCHAR(255),
    IN p_Body TEXT
)
BEGIN
    INSERT INTO Posts (Title, Body) VALUES (p_Title, p_Body);
    SELECT LAST_INSERT_ID() AS NewPostId;
END //

DELIMITER ;
CALL InsertPost('Fourth Sample Post', 'This is the body of the fourth sample post.');





DELIMITER //

CREATE PROCEDURE InsertComment(
    IN p_PostId INT,
    IN p_CommentText TEXT,
    IN p_Author VARCHAR(255)
)
BEGIN
    INSERT INTO Comments (PostId, CommentText, Author) VALUES (p_PostId, p_CommentText, p_Author);
    SELECT LAST_INSERT_ID() AS NewCommentId;
END //

DELIMITER ;
CALL InsertComment(1, 'This is another comment on the first post.', 'George');
CALL InsertComment(2, 'This is another comment on the second post.', 'Ritesh');
