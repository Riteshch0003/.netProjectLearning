import React, { useEffect, useState } from 'react';
import axios from 'axios';
import { Link } from 'react-router-dom';

function PostList() {
  const [posts, setPosts] = useState([]);
  const [error, setError] = useState(null);

  useEffect(() => {
    axios.get('http://localhost:5041/api/PostComments')
      .then(response => {
        console.log('API response:', response.data); // Log the response

        // Check if response data is an array and filter out unnecessary fields
        if (Array.isArray(response.data)) {
          const filteredPosts = response.data.map(post => {
            // Create a new object without the `$id` field
            const { $id, ...postData } = post;
            return postData;
          });
          setPosts(filteredPosts);
        } else {
          setError('Expected an array of posts, but got something else.');
        }
      })
      .catch(error => {
        console.error('Error fetching posts:', error);
        setError('Error fetching posts');
      });
  }, []);

  return (
    <div>
      <h1>Posts</h1>
      {error && <p>{error}</p>} {/* Show any error message */}
      <ul>
        {posts.map(post => (
          <li key={post.id}>
            <Link to={`/posts/${post.id}`}>{post.title}</Link>
          </li>
        ))}
      </ul>
    </div>
  );
}

export default PostList;
