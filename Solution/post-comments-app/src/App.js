import React from 'react';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import PostList from './Components/PostList';
import PostDetail from './Components/PostDetails';
import CreatePost from './Components/CreatePost'; // Import the CreatePost component

function App() {
  return (
    <Router>
      <div>
        <Routes>
          <Route path="/" element={<PostList />} />
          <Route path="/posts/:id" element={<PostDetail />} />
          <Route path="/create-post" element={<CreatePost />} /> {/* Add route for CreatePost */}
        </Routes>
      </div>
    </Router>
  );
}

export default App;
