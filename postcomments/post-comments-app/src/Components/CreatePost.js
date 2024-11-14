import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';

const CreatePost = () => {
  const navigate = useNavigate();

  const [post, setPost] = useState({
    title: '',
    body: '',
  });

  const [comment, setComment] = useState({
    commentText: '',
    author: '',
  });

  const [newPost, setNewPost] = useState(null); 

  const handlePostInputChange = (e) => {
    const { name, value } = e.target;
    setPost({ ...post, [name]: value });
  };

  const handleCommentInputChange = (e) => {
    const { name, value } = e.target;
    setComment({ ...comment, [name]: value });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();

    try {
      const postResponse = await fetch('http://localhost:5041/api/PostComments', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(post),
      });

      if (!postResponse.ok) {
        throw new Error('Error creating post');
      }

      const createdPost = await postResponse.json();
      setNewPost({ ...createdPost, comments: createdPost.comments || [] }); 

      const commentData = {
        commentText: comment.commentText,
        author: comment.author,
        postId: createdPost.id,
        createdAt: new Date().toISOString(), 
      };

      const commentResponse = await fetch(
        `http://localhost:5041/api/PostComments/${createdPost.id}/comments`,
        {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
          },
          body: JSON.stringify(commentData),
        }
      );

      if (!commentResponse.ok) {
        const errorDetail = await commentResponse.json();
        console.error('Error adding comment to post:', errorDetail);
        throw new Error('Error adding comment to post');
      }

      const createdComment = await commentResponse.json();

      setNewPost((prevPost) => {
        const comments = Array.isArray(prevPost.comments) ? prevPost.comments : [];
        return {
          ...prevPost,
          comments: [...comments, createdComment],
        };
      });

      alert('Post and comment created successfully');
            navigate('/');

      setPost({ title: '', body: '' });
      setComment({ commentText: '', author: '' });
    } catch (error) {
      console.error('Error:', error);
    }
  };

  return (
    <div>
      <h2>Create New Post</h2>
      <form onSubmit={handleSubmit}>
        <div>
          <label>Title:</label>
          <input
            type="text"
            name="title"
            value={post.title}
            onChange={handlePostInputChange}
            required
          />
        </div>
        <div>
          <label>Body:</label>
          <textarea
            name="body"
            value={post.body}
            onChange={handlePostInputChange}
            required
          />
        </div>
        <h3>Comment</h3>
        <div>
          <label>Comment Text:</label>
          <input
            type="text"
            name="commentText"
            value={comment.commentText}
            onChange={handleCommentInputChange}
            required
          />
        </div>
        <div>
          <label>Author:</label>
          <input
            type="text"
            name="author"
            value={comment.author}
            onChange={handleCommentInputChange}
            required
          />
        </div>
        <button type="submit">Create Post with Comment</button>
      </form>

      {newPost && (
        <div>
          <h3>Created Post</h3>
          <p><strong>Title:</strong> {newPost.title}</p>
          <p><strong>Body:</strong> {newPost.body}</p>

          <h4>Comments</h4>
          {newPost.comments.length > 0 ? (
            <ul>
              {newPost.comments.map((comment, index) => (
                <li key={index}>
                  <p><strong>Author:</strong> {comment.author}</p>
                  <p><strong>Comment:</strong> {comment.commentText}</p>
                  <p><strong>Created At:</strong> {new Date(comment.createdAt).toLocaleString()}</p>
                </li>
              ))}
            </ul>
          ) : (
            <p>No comments available.</p>
          )}
        </div>
      )}
    </div>
  );
};

export default CreatePost;
