import React, { useState } from 'react';
import { BrowserRouter as Router, Route, Routes, Navigate } from 'react-router-dom'; // Correct import for Navigate
import Navigation from './Components/Navigaton';
import PostList from './Components/PostList';
import PostDetail from './Components/PostDetails';
import CreatePost from './Components/CreatePost'; 
import Login from './Components/Auth/Login';
import Register from './Components/Auth/Register';

function App() {
  const [isLoggedIn, setIsLoggedIn] = useState(false); // Tracks login status

  // This function will update the login state
  const handleLogin = () => {
    setIsLoggedIn(true);
  };

  return (
    <Router>
      <div>
        <Navigation />

        <Routes>
          {/* If logged in, show the post list, otherwise redirect to login */}
          <Route path="/" element={isLoggedIn ? <PostList /> : <Navigate to="/login" />} />

          {/* Display post details if logged in, otherwise redirect */}
          <Route path="/posts/:id" element={isLoggedIn ? <PostDetail /> : <Navigate to="/login" />} />
          
          {/* Create Post */}
          <Route path="/create-post" element={isLoggedIn ? <CreatePost /> : <Navigate to="/login" />} />

          {/* Login and Register routes */}
          <Route path="/login" element={<Login onLogin={handleLogin} />} />
          <Route path="/register" element={<Register />} />
        </Routes>
      </div>
    </Router>
  );
}

export default App;
