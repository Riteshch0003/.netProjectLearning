import React, { useEffect, useState } from 'react';
import axios from 'axios';
import { Link } from 'react-router-dom';

function PostList() {
  const [posts, setPosts] = useState([]);
  const [error, setError] = useState(null);

  useEffect(() => {
    axios.get('http://localhost:5041/api/PostComments')
      .then(response => {

        if (response.data && Array.isArray(response.data.$values)) {
          const filteredPosts = response.data.$values.map(post => {
            const { $id, ...postData } = post;
            return postData;
          });
          setPosts(filteredPosts);
        } else {
          setError('Expected an array of posts in $values, but got something else.');
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
      {error && <p>{error}</p>} 
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
