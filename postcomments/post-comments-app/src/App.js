import React from 'react';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import Navigation from './Components/Navigaton';
import PostList from './Components/PostList';
import PostDetail from './Components/PostDetails';
import CreatePost from './Components/CreatePost'; 
import Login from './Components/Auth/Login';
import Register from './Components/Auth/Register';

function App() {
  return (
    <Router>
      <div>
        <Navigation />
        <Routes>
          <Route path="/" element={<PostList />} />
          {/* <Route path="/posts/:id" element={<PostDetail />} /> */}
          {/* <Route path="/create-post" element={<CreatePost />} /> */}
          {/* <Route path="/login" element={<Login />} /> */}
          {/* <Route path="/register" element={<Register />} /> */}
        </Routes>
      </div>
    </Router>
  );
}

export default App;
