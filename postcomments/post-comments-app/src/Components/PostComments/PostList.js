import React, { useEffect, useState } from 'react';
import axios from 'axios';
import { Link } from 'react-router-dom';

function PostList() {
  const [posts, setPosts] = useState([]);
  const [error, setError] = useState(null);
  const userId = localStorage.getItem('userId'); // Retrieve userId from localStorage

  useEffect(() => {
    if (!userId) {
      setError('User is not logged in.');
      return;
    }

    axios
      .get(`http://localhost:5041/api/PostComments/user/${userId}`)
      .then(response => {
        console.log('API Response:', response.data); // Debug the response structure

        // Check if response has $values at the top level (array of posts)
        if (response.data && Array.isArray(response.data.$values)) {
          const filteredPosts = response.data.$values.map(post => {
            const { $id, comments, ...postData } = post;

            // If comments are present and have $values, extract them as an array
            const postComments = comments && comments.$values
              ? comments.$values.map(comment => {
                  const { $id, ...commentData } = comment;
                  return commentData; 
                })
              : [];

            return {
              ...postData,
              comments: postComments 
            };
          });

          setPosts(filteredPosts);
        } else {
          setError('Unexpected API response format.');
        }
      })
      .catch(error => {
        console.error('Error fetching posts:', error);
        setError('Failed to fetch posts for the logged-in user.');
      });
  }, [userId]);

  return (
    <div>
      <h1>User Posts</h1>
      {error && <p style={{ color: 'red' }}>{error}</p>}
      <ul>
        {posts.map(post => (
          <li key={post.id}>
            <h2>{post.title}</h2>
            <p>{post.content}</p>
            {post.comments && post.comments.length > 0 && (
              <div>
                <h3>Comments:</h3>
                <ul>
                  {post.comments.map(comment => (
                    <li key={comment.id}>
                      <p><strong>{comment.author}</strong>: {comment.content}</p>
                    </li>
                  ))}
                </ul>
              </div>
            )}
          </li>
        ))}
      </ul>
    </div>
  );
}

export default PostList;
