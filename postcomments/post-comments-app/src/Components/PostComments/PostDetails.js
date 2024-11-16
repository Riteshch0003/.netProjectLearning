import React, { useEffect, useState } from 'react';
import axios from 'axios';
import { useParams } from 'react-router-dom';

function PostDetail() {
  const { id } = useParams(); 
  const [post, setPost] = useState(null);
  const [comments, setComments] = useState([]);
  const [error, setError] = useState(null);
  const [editingCommentId, setEditingCommentId] = useState(null);
  const [editContent, setEditContent] = useState("");

  useEffect(() => {
    axios.get(`http://localhost:5041/api/PostComments/${id}`)
      .then(response => {
        if (response.data) {
          const { $id, comments, ...postData } = response.data;
          setPost(postData);

          if (comments && comments.$values && Array.isArray(comments.$values)) {
            setComments(comments.$values);
          } else {
            setComments([]);
          }
        } else {
          setError("Post not found");
        }
      })
      .catch(error => {
        console.error("Error fetching post:", error);
        setError("Error fetching post");
      });
  }, [id]);

  const handleEdit = (commentId, currentContent) => {
    setEditingCommentId(commentId);
    setEditContent(currentContent);
  };

  const handleSave = async () => {
    if (!editContent.trim()) {
      alert("Content cannot be empty!");
      return;
    }

    try {
      const response = await axios.put(
        `http://localhost:5041/api/PostComments/post/${id}/comments/${editingCommentId}`,
        { content: editContent }
      );

      if (response.status === 200) {
        const updatedComment = response.data;
        setComments((prevComments) =>
          prevComments.map((comment) =>
            comment.id === updatedComment.id ? updatedComment : comment
          )
        );
        setEditingCommentId(null);
        setEditContent("");
      }
    } catch (error) {
      console.error("Error updating comment:", error);
      alert("Failed to update the comment.");
    }
  };

  if (error) {
    return <p>{error}</p>;
  }

  if (!post) {
    return <p>Loading...</p>;
  }

  return (
    <div>
      <h1>{post.title}</h1>
      <p>{post.content}</p>

      <h2>Comments</h2>
      {comments.length > 0 ? (
        <ul>
          {comments.map((comment) => (
            <li key={comment.id}>
              {editingCommentId === comment.id ? (
                <div>
                  <textarea
                    value={editContent}
                    onChange={(e) => setEditContent(e.target.value)}
                    rows="3"
                  />
                  <button onClick={handleSave}>Save</button>
                  <button onClick={() => setEditingCommentId(null)}>Cancel</button>
                </div>
              ) : (
                <div>
                  <strong>{comment.author}</strong>: {comment.content}
                  <button onClick={() => handleEdit(comment.id, comment.content)}>
                    Edit
                  </button>
                </div>
              )}
            </li>
          ))}
        </ul>
      ) : (
        <p>No comments available</p>
      )}
    </div>
  );
}

export default PostDetail;
