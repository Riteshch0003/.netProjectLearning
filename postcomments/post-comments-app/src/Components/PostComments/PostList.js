import React, { useState, useEffect } from "react";
import axios from "axios";

const MyPosts = () => {
  const [posts, setPosts] = useState([]);
  const [error, setError] = useState(null);

  useEffect(() => {
    const token = localStorage.getItem("token");
    const userId = localStorage.getItem("userId"); // Assuming userId is stored in localStorage
    console.log(token);
    console.log(userId);
  
    if (!token || !userId) {
      setError("Unauthorized. Please log in.");
      return;
    }
  
    axios
      .get(`http://localhost:5041/api/PostComments/user/${userId}`, {
        headers: { Authorization: `Bearer ${token}` },
      })
      .then((response) => {
        console.log(response.data); // Log the response for debugging
  
        // Access the posts from the $values property
        const postsData = response.data.$values || [];
        setPosts(postsData);
      })
      .catch((err) => {
        console.error(err); // Log the error for debugging
        setError("Failed to fetch posts.");
      });
  }, []);
  
  if (error) {
    return <div>{error}</div>;
  }

  return (
    <div>
      <h1>My Posts</h1>
      {posts.length === 0 ? (
        <p>No posts found.</p>
      ) : (
        <ul>
          {posts.map((post) => (
            <li key={post.id}>
              <h3>{post.title}</h3>
              <p>{post.content}</p>
            </li>
          ))}
        </ul>
      )}
    </div>
  );
};

export default MyPosts;
