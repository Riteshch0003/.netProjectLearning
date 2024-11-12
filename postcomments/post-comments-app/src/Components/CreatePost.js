import React, { useState } from 'react';

const CreatePost = () => {
  const [post, setPost] = useState({
    title: '',
    body: '',
  });

  const [comment, setComment] = useState({
    commentText: '',
    author: '',
    createdAt: new Date().toISOString(),
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
    setNewPost({ ...createdPost, comments: createdPost.comments || [] });

    // Step 2: Add comment to the created post
    const commentData = {
      commentText: comment.commentText,
      author: comment.author,
      postId: createdPost.id, // Make sure to use the actual postId
      createdAt: comment.createdAt,
    };

    console.log('Sending comment data:', commentData); // Log the comment data being sent

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
      const errorDetail = await commentResponse.json(); // Retrieve detailed error if available
      console.error('Error adding comment to post:', errorDetail);
      throw new Error('Error adding comment to post');
    }

    const createdComment = await commentResponse.json();

    setNewPost((prevPost) => ({
      ...prevPost,
      comments: [...(prevPost.comments || []), createdComment],
    }));

    alert('Post and comment created successfully');
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
          <h3>Post Created:</h3>
          <p>ID: {newPost.id}</p>
          <p>Title: {newPost.title}</p>
          <p>Body: {newPost.body}</p>
          <h4>Comments:</h4>
          {newPost.comments.length > 0 ? (
            newPost.comments.map((c, index) => (
              <div key={index}>
                <p>Comment ID: {c.id}</p>
                <p>Text: {c.commentText}</p>
                <p>Author: {c.author}</p>
                <p>Created At: {c.createdAt}</p>
              </div>
            ))
          ) : (
            <p>No comments yet</p>
          )}
        </div>
      )}
    </div>
  );
};

export default CreatePost;
