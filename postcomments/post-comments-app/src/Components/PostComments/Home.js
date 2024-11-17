import React, { useEffect, useState } from 'react';

const Home = () => {
  const [posts, setPosts] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchPosts = async () => {
      try {
        const response = await fetch('http://localhost:5041/api/PostComments/all');

        if (!response.ok) {
          throw new Error(`Error ${response.status}: ${response.statusText}`);
        }

        const data = await response.json();
        setPosts(data.$values || []);
        setLoading(false);
      } catch (err) {
        setError(err);
        setLoading(false);
      }
    };

    fetchPosts();
  }, []);

  if (loading) {
    return <div style={styles.loading}>Loading posts...</div>;
  }

  if (error) {
    return <div style={styles.error}>Error: {error.message}</div>;
  }

  return (
    <div style={styles.homeContainer}>
      <h1 style={styles.pageTitle}>All Posts</h1>
      {posts.length === 0 ? (
        <p style={styles.noPosts}>No posts available.</p>
      ) : (
        <div style={styles.tableContainer}>
          <table style={styles.styledTable}>
            <thead>
              <tr>
                <th style={styles.tableHeader}>Post</th>
                <th style={styles.tableHeader}>Content</th>
              </tr>
            </thead>
            <tbody>
              {posts.map((post) => (
                <tr key={post.$id} style={styles.tableRow}>
                  <td style={styles.tableCell}>{post.title}</td>
                  <td style={styles.tableCell}>{post.content}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
};

// Inline styles
const styles = {
  homeContainer: {
    margin: '20px',
    padding: '20px',
    backgroundColor: '#f9f9f9',
    borderRadius: '10px',
    boxShadow: '0 4px 10px rgba(0, 0, 0, 0.1)',
    display: 'flex',
    flexDirection: 'column',
    alignItems: 'center',
  },
  pageTitle: {
    fontSize: '2.5rem',
    color: '#333',
    fontFamily: "'Arial', sans-serif",
    marginBottom: '20px',
    textAlign: 'center',
    fontWeight: 'bold',
  },
  noPosts: {
    color: '#888',
    fontSize: '1.2rem',
    marginTop: '20px',
  },
  tableContainer: {
    width: '80%',
    maxWidth: '1000px',
    backgroundColor: 'white',
    borderRadius: '8px',
    padding: '20px',
    boxShadow: '0 4px 10px rgba(0, 0, 0, 0.1)',
    borderLeft: '6px solid #4CAF50',
    borderRight: '6px solid #4CAF50',
    marginTop: '20px',
  },
  styledTable: {
    width: '100%',
    borderCollapse: 'collapse',
  },
  tableHeader: {
    backgroundColor: '#4CAF50',
    color: 'white',
    fontWeight: 'bold',
    fontSize: '1.2rem',
    padding: '15px',
    textAlign: 'left',
    textTransform: 'uppercase',
  },
  tableRow: {
    backgroundColor: '#fff',
    transition: 'background-color 0.3s ease',
  },
  tableCell: {
    padding: '15px',
    textAlign: 'left',
    fontSize: '1rem',
    color: '#555',
    borderBottom: '1px solid #ddd',
  },
  loading: {
    color: '#ff6347',
    fontSize: '1.5rem',
    fontWeight: 'bold',
    textAlign: 'center',
    marginTop: '20px',
  },
  error: {
    color: '#ff0000',
    fontSize: '1.5rem',
    fontWeight: 'bold',
    textAlign: 'center',
    marginTop: '20px',
  },
};

export default Home;
