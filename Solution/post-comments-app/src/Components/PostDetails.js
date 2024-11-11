import React, { useEffect, useState } from 'react';
import axios from 'axios';
import { useParams } from 'react-router-dom';

function PostDetail() {
  const { id } = useParams();
  const [post, setPost] = useState(null);
  const [commentText, setCommentText] = useState('');

  useEffect(() => {
    axios.get(`http://localhost:5041/api/PostComments/${id}`)
      .then(response => setPost(response.data))
      .catch(error => console.error(error));
  }, [id]);

  const handleCommentSubmit = async (e) => {
    e.preventDefault();
    const newComment = {
      postId: post.id,
      commentText,
      author: 'Anonymous',  // Add logic for author if needed
    };

    try {
      const response = await axios.post(`http://localhost:5041/api/PostComments`, newComment);
      setPost(prevPost => ({
        ...prevPost,
        comments: [...prevPost.comments, response.data]
      }));
      setCommentText('');
    } catch (error) {
      console.error(error);
    }
  };

  if (!post) return <div>Loading...</div>;

  return (
    <div>
      <h2>{post.title}</h2>
      <p>{post.body}</p>

      <h3>Comments</h3>
      <ul>
        {post.comments.map((comment, index) => (
          <li key={index}>{comment.commentText} - <em>{comment.author}</em></li>
        ))}
      </ul>

      <form onSubmit={handleCommentSubmit}>
        <textarea
          value={commentText}
          onChange={(e) => setCommentText(e.target.value)}
          placeholder="Add a comment"
        />
        <button type="submit">Submit Comment</button>
      </form>
    </div>
  );
}

export default PostDetail;
