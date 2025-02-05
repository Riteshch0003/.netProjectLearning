import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';

const CreatePost = () => {
  const navigate = useNavigate();

  const [post, setPost] = useState({
    title: '',
    body: '',
    userId: '', 
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
      // Get the userId from localStorage (for the logged-in user)
      const userId = localStorage.getItem('userId');
      if (!userId) {
        alert('You need to log in to create a post.');
        return;
      }

      // Save the post
      const postResponse = await fetch(`http://localhost:5041/api/PostComments/user/${userId}`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          Title: post.title,
          Content: post.body,
          UserId: userId, 
        }),
      });

      if (!postResponse.ok) {
        const errorDetail = await postResponse.json();
        console.error('Error creating post:', errorDetail);
        alert('Error creating post: ' + JSON.stringify(errorDetail.errors || 'Unknown error'));
        throw new Error('Error creating post');
      }

      const createdPost = await postResponse.json();
      setNewPost({ ...createdPost, comments: createdPost.comments || [] });

      // Save the comment for the post
      const commentData = {
        Content: comment.commentText, 
        Author: comment.author,
        PostId: createdPost.id,
        UserId: userId, 
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
        alert('Error adding comment: ' + JSON.stringify(errorDetail.errors || 'Unknown error'));
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

      // Reset the form after successful submission
      setPost({ title: '', body: '', userId: '' });
      setComment({ commentText: '', author: '' });

      // Optionally redirect to the post list or detail page
      navigate('/postlist'); // Redirect to the Post List page
    } catch (error) {
      console.error('Error:', error);
      alert('An error occurred while creating the post or comment.');
    }
  };

  return (
    <div>
      <h2>Create New Post</h2>
      <form onSubmit={handleSubmit}>
        <div>
          <label>User ID:</label>
          <input
            type="number"
            name="userId"
            value={post.userId}
            onChange={handlePostInputChange}
            required
          />
        </div>
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
          <h2>Created Post</h2>
          <p><strong>Title:</strong> {newPost.title}</p>
          <p><strong>Content:</strong> {newPost.content}</p>
          <h3>Comments:</h3>
          {newPost.comments && newPost.comments.length > 0 ? (
            newPost.comments.map((comment, index) => (
              <div key={index}>
                <p><strong>{comment.author}:</strong> {comment.content}</p>
              </div>
            ))
          ) : (
            <p>No comments yet.</p>
          )}
        </div>
      )}
    </div>
  );
};

export default CreatePost;
