import React, { useState, useEffect } from "react";
import axios from "axios";

const MyPosts = () => {
  const [posts, setPosts] = useState([]);
  const [error, setError] = useState(null);

  useEffect(() => {
    const token = localStorage.getItem("token");
    const userId = localStorage.getItem("userId");

    if (!token || !userId) {
      setError("Unauthorized. Please log in.");
      return;
    }

    axios
      .get(`http://localhost:5041/api/PostComments/user/${userId}`, {
        headers: { Authorization: `Bearer ${token}` },
      })
      .then((response) => {
        const postsData = response.data.$values || [];
        setPosts(postsData);
      })
      .catch((error) => {
        setError("Failed to fetch posts.");
      });
  }, []);

  // Handle logout and remove token
  const handleLogout = () => {
    // Remove token and userId from localStorage
    localStorage.removeItem("token");
    localStorage.removeItem("userId");

    // Redirect the user to login page
    window.location.href = "/login";
  };

  if (error) {
    return (
      <div className="flex justify-center items-center min-h-screen bg-gradient-to-r from-indigo-500 via-purple-500 to-pink-500 text-white">
        <div className="text-center p-6 bg-white text-gray-800 rounded-xl shadow-lg w-full max-w-lg">
          <h2 className="text-3xl font-bold mb-4">Error</h2>
          <p>{error}</p>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gradient-to-r from-indigo-500 via-purple-500 to-pink-500 flex justify-center items-center text-white">
      <div className="bg-white text-gray-800 p-8 rounded-xl shadow-2xl w-full max-w-4xl">
        <div className="flex justify-between items-center mb-6">
          <h1 className="text-3xl font-extrabold">My Posts</h1>
          <button
            onClick={handleLogout}
            className="bg-red-600 text-white px-4 py-2 rounded-full hover:bg-red-700 transition duration-300"
          >
            Logout
          </button>
        </div>

        {/* Posts */}
        {posts.length === 0 ? (
          <p className="text-xl text-center text-gray-700">No posts found.</p>
        ) : (
          <ul className="space-y-6">
            {posts.map((post) => (
              <li key={post.id} className="bg-gray-100 p-6 rounded-lg shadow-lg">
                <h3 className="text-2xl font-semibold text-indigo-600">{post.title}</h3>
                <p className="mt-2 text-gray-700">{post.content}</p>
              </li>
            ))}
          </ul>
        )}
      </div>
    </div>
  );
};

export default MyPosts;
