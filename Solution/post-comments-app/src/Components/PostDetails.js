import React, { useEffect, useState } from 'react';
import axios from 'axios';
import { useParams } from 'react-router-dom';

function PostDetail() {
  const { id } = useParams(); // Get the post ID from the URL using useParams
  const [post, setPost] = useState(null);
  const [comments, setComments] = useState([]); // State for comments
  const [error, setError] = useState(null);

  useEffect(() => {
    // Fetch post data
    axios.get(`http://localhost:5041/api/PostComments/${id}`)
      .then(response => {
        console.log('API response:', response.data); // Log the response

        // Check if the post data exists
        if (response.data) {
          const { $id, comments, ...postData } = response.data; // Remove $id field
          setPost(postData);

          // Check if comments is an object and has $values, then set the comments state
          if (comments && Array.isArray(comments.$values)) {
            setComments(comments.$values);
          } else {
            setComments([]); // In case no comments or invalid structure
          }
        } else {
          setError('Post not found');
        }
      })
      .catch(error => {
        console.error('Error fetching post:', error);
        setError('Error fetching post');
      });
  }, [id]); // Re-fetch when the id changes

  if (error) {
    return <p>{error}</p>;
  }

  if (!post) {
    return <p>Loading...</p>;
  }

  return (
    <div>
      <h1>{post.title}</h1>
      <p>{post.body}</p>

      <h2>Comments</h2>
      {comments.length > 0 ? (
        <ul>
          {comments.map(comment => (
            <li key={comment.id}>
              <strong>{comment.author}</strong>: {comment.commentText}
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
