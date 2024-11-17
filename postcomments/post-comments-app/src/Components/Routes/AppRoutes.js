import React from 'react';
import { Link, Routes, Route } from 'react-router-dom';
import Login from '../Auth/Login';
import Register from '../Auth/Register';
import Home from '../PostComments/Home';
import PostDetail from '../PostComments/PostDetails';
import CreatePost from '../PostComments/CreatePost';
import MyPosts from '../PostComments/PostList';





const AppRoutes = () => {
  return (
    <div className="flex">
     

      <div className="ml-64 flex-1 p-6">
        <Routes>
          <Route path="/" element={<Home/>} />
          <Route path="/login" element={<Login />} />
          <Route path="/register" element={<Register />} />
          <Route path="/createpost" element={<CreatePost />} />
          <Route path="/posts/:id" element={<PostDetail />} />
          <Route path="/postlist" element={<MyPosts/>} />
        </Routes>
      </div>
    </div>
  );
};

export default AppRoutes;
