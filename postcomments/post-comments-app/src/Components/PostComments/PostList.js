import React, { useEffect, useState } from 'react';
import axios from 'axios';

function EditPost() {
  const [posts, setPosts] = useState([]);
  const [error, setError] = useState(null);
  const [editingPostId, setEditingPostId] = useState(null);
  const [editTitle, setEditTitle] = useState("");
  const [editContent, setEditContent] = useState("");

  const userId = localStorage.getItem("userId")  

  useEffect(() => {
    if (!userId) {
      setError("User is not logged in or userId is invalid.");
      return;
    }

    axios
      .get(`http://localhost:5041/api/PostComments/user/${userId}`)
      .then((response) => {
        if (response.data && Array.isArray(response.data.$values)) {
          const postsData = response.data.$values.map((post) => {
            const { $id, comments, ...postData } = post;
            return postData;
          });
          setPosts(postsData);
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
    <div>
      <h1>Edit User Posts</h1>
      {error && <p style={{ color: "red" }}>{error}</p>}
      <ul>
        {posts.map((post) => (
          <li key={post.id}>
            {editingPostId === post.id ? (
              <div>
                <input
                  type="text"
                  value={editTitle}
                  onChange={(e) => setEditTitle(e.target.value)}
                  placeholder="Edit title"
                />
                <textarea
                  value={editContent}
                  onChange={(e) => setEditContent(e.target.value)}
                  placeholder="Edit content"
                  rows={4}
                  cols={50}
                ></textarea>
                <button onClick={handleSave}>Save</button>
                <button onClick={handleCancel}>Cancel</button>
              </div>
            ) : (
              <div>
                <h2>{post.title}</h2>
                <p>{post.content}</p>
                <button onClick={() => handleEdit(post)}>Edit Post</button>
              </div>
            )}
          </li>
        ))}
      </ul>
    </div>
  );
}

export default EditPost;
