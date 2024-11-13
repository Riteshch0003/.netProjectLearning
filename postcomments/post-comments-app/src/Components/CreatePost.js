import React, { useState } from 'react';

const CreatePost = () => {
  const [post, setPost] = useState({
    title: '',
    body: '',
  });

  const [comment, setComment] = useState({
    commentText: '',
    author: '',
  });

  // Initialize newPost with an empty comments array
  const [newPost, setNewPost] = useState(null); // Updated to initialize as null for conditional rendering

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
      // Step 1: Create the post
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
      setNewPost({ ...createdPost, comments: createdPost.comments || [] }); // Ensure comments is an array

      // Step 2: Add comment to the created post
      const commentData = {
        commentText: comment.commentText,
        author: comment.author,
        postId: createdPost.id,
        createdAt: new Date().toISOString(), // Setting createdAt dynamically
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

      // Refactor to simplify comment update and prevent iterable error
      setNewPost((prevPost) => {
        // Ensure prevPost.comments is an array
        const comments = Array.isArray(prevPost.comments) ? prevPost.comments : [];
        return {
          ...prevPost,
          comments: [...comments, createdComment],
        };
      });

      alert('Post and comment created successfully');

      // Optional: Clear form fields after submission
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

      {/* Display the post and comments after creation */}
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
