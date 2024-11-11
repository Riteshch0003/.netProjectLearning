import React, { useState } from 'react';

const CreatePost = () => {
  const [post, setPost] = useState({
    title: '',
    body: ''
  });

  const [newPost, setNewPost] = useState(null);

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setPost({ ...post, [name]: value });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      const response = await fetch('http://localhost:5041/api/PostComments', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(post),
      });

      if (response.ok) {
        const data = await response.json();
        setNewPost(data);
        alert('Post created successfully');
      } else {
        console.error('Error creating post');
      }
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
            onChange={handleInputChange}
            required
          />
        </div>
        <div>
          <label>Body:</label>
          <textarea
            name="body"
            value={post.body}
            onChange={handleInputChange}
            required
          />
        </div>
        <button type="submit">Create Post</button>
      </form>

      {newPost && (
        <div>
          <h3>Post Created:</h3>
          <p>ID: {newPost.id}</p>
          <p>Title: {newPost.title}</p>
          <p>Body: {newPost.body}</p>
        </div>
      )}
    </div>
  );
};

export default CreatePost;
