// MyPosts.js
import React, { useState, useEffect } from 'react';
import axios from 'axios';

const MyPosts = () => {
  const [posts, setPosts] = useState([]);
  const [error, setError] = useState(null);
  const [editingPostId, setEditingPostId] = useState(null);
  const [editTitle, setEditTitle] = useState("");
  const [editContent, setEditContent] = useState("");

  const userId = localStorage.getItem("userId"); 

  useEffect(() => {
    if (!userId) {
      setError("User is not logged in or userId is invalid.");
      return;
    }

    axios
      .get(`http://localhost:5041/api/PostComments/user/${userId}`)
      .then((response) => {
        if (response.data && Array.isArray(response.data.$values)) {
          setPosts(response.data.$values);
        } else {
          setError("Unexpected API response format.");
        }
      })
      .catch((error) => {
        console.error("Error fetching posts:", error);
        setError("Failed to fetch posts.");
      });
  }, [userId]);

  const handleEdit = (post) => {
    setEditingPostId(post.id);
    setEditTitle(post.title);
    setEditContent(post.content);
  };

  const handleSave = async () => {
    if (!editTitle.trim() || !editContent.trim()) {
      alert("Both title and content are required.");
      return;
    }

    try {
      const response = await axios.put(
        `http://localhost:5041/api/PostComments/user/${userId}/post/${editingPostId}`,
        { title: editTitle, content: editContent }
      );

      if (response.status === 200) {
        const updatedPost = response.data;
        setPosts((prevPosts) =>
          prevPosts.map((post) =>
            post.id === updatedPost.id ? { ...post, ...updatedPost } : post
          )
        );
        setEditingPostId(null);
        setEditTitle("");
        setEditContent("");
      } else {
        alert("Failed to update the post.");
      }
    } catch (error) {
      console.error("Error updating post:", error);
      alert("An error occurred while updating the post.");
    }
  };

  const handleCancel = () => {
    setEditingPostId(null);
    setEditTitle("");
    setEditContent("");
  };

  return (
    <div style={styles.container}>
      <h1 style={styles.header}>My Posts</h1>
      {error && <p style={{ color: "red" }}>{error}</p>}
      {posts.length === 0 ? (
        <p>You don't have any posts yet.</p>
      ) : (
        <ul style={styles.postsList}>
          {posts.map((post) => (
            <li key={post.id} style={styles.postItem}>
              {editingPostId === post.id ? (
                <div>
                  <input
                    type="text"
                    value={editTitle}
                    onChange={(e) => setEditTitle(e.target.value)}
                    placeholder="Edit title"
                    style={styles.input}
                  />
                  <textarea
                    value={editContent}
                    onChange={(e) => setEditContent(e.target.value)}
                    placeholder="Edit content"
                    rows={4}
                    cols={50}
                    style={styles.textarea}
                  ></textarea>
                  <button onClick={handleSave} style={styles.button}>
                    Save
                  </button>
                  <button onClick={handleCancel} style={styles.button}>
                    Cancel
                  </button>
                </div>
              ) : (
                <div>
                  <h2 style={styles.postTitle}>{post.title}</h2>
                  <p style={styles.postContent}>{post.content}</p>
                  <button
                    onClick={() => handleEdit(post)}
                    style={styles.button}
                  >
                    Edit Post
                  </button>
                </div>
              )}
            </li>
          ))}
        </ul>
      )}
    </div>
  );
};

const styles = {
  container: {
    padding: '20px',
    backgroundColor: '#f8f9fa',
    borderRadius: '8px',
    boxShadow: '0 4px 10px rgba(0, 0, 0, 0.1)',
  },
  header: {
    fontSize: '2rem',
    marginBottom: '20px',
    textAlign: 'center',
  },
  postsList: {
    listStyleType: 'none',
    padding: 0,
  },
  postItem: {
    marginBottom: '20px',
    padding: '10px',
    border: '1px solid #ddd',
    borderRadius: '4px',
    backgroundColor: '#fff',
  },
  postTitle: {
    fontSize: '1.5rem',
    color: '#333',
  },
  postContent: {
    fontSize: '1rem',
    color: '#666',
  },
  input: {
    width: '100%',
    padding: '10px',
    borderRadius: '4px',
    border: '1px solid #ccc',
    marginBottom: '10px',
  },
  textarea: {
    width: '100%',
    height: '150px',
    padding: '10px',
    borderRadius: '4px',
    border: '1px solid #ccc',
    marginBottom: '10px',
  },
  button: {
    backgroundColor: '#4CAF50',
    color: 'white',
    padding: '10px 15px',
    border: 'none',
    borderRadius: '4px',
    cursor: 'pointer',
    marginRight: '10px',
  },
};

export default MyPosts;
