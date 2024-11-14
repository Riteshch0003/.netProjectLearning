DELIMITER //

CREATE PROCEDURE InsertPost(
    IN p_Title VARCHAR(255),
    IN p_Content TEXT,
    IN p_UserId INT
)
BEGIN
    INSERT INTO Posts (Title, Content, UserId) VALUES (p_Title, p_Content, p_UserId);
    
    SELECT LAST_INSERT_ID() AS NewPostId;
END //

DELIMITER ;


CALL InsertPost('My First Post', 'This is the content of my first post.', 1);




DELIMITER //

DELIMITER //

DELIMITER //

CREATE PROCEDURE InsertComment(
    IN p_PostId INT,
    IN p_Content TEXT,
    IN p_Author VARCHAR(255),
    IN p_UserId INT
)
BEGIN
    INSERT INTO Comments (PostId, Content, Author, UserId) 
    VALUES (p_PostId, p_Content, p_Author, p_UserId);
    
    SELECT LAST_INSERT_ID() AS NewCommentId;
END //

DELIMITER ;

CALL InsertComment(1, 'This is a comment on the first post.', 'Alice', 1);
