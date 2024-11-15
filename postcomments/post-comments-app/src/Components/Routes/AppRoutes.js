import React from 'react';
import { Link, Routes, Route } from 'react-router-dom';
import Login from '../Auth/Login';
import Register from '../Auth/Register';
import Home from '../PostComments/Home';
import PostDetail from '../PostComments/PostDetails';
import PostList from '../PostComments/PostList';
import CreatePost from '../PostComments/CreatePost';

const AppRoutes = () => {
  return (
    <div className="flex">
      {/* MenuBar */}
      <div className="w-64 h-screen bg-gray-800 text-white fixed top-0 left-0 flex flex-col p-6">
        <h1 className="text-xl font-bold mb-6">Menu</h1>
        <ul className="space-y-4">
          <li>
            <Link to="/" className="text-gray-300 hover:text-teal-400 transition">
              Home
            </Link>
          </li>
          <li>
            <Link to="/login" className="text-gray-300 hover:text-teal-400 transition">
              Login
            </Link>
          </li>
          <li>
            <Link to="/register" className="text-gray-300 hover:text-teal-400 transition">
              Register
            </Link>
          </li>
           <li>
            <Link to="/createpost" className="text-gray-300 hover:text-teal-400 transition">
              CreatePost
            </Link>
          </li>
          <li>
            <Link to="/postdetails" className="text-gray-300 hover:text-teal-400 transition">
              PostDetail
            </Link>
          </li>
          <li>
            <Link to="/postlist" className="text-gray-300 hover:text-teal-400 transition">
              PostList
            </Link>
          </li>
        </ul>
      </div>

      {/* Main Content and Routes */}
      <div className="ml-64 flex-1 p-6">
        <Routes>
          <Route path="/" element={<Home/>} />
          <Route path="/login" element={<Login />} />
          <Route path="/register" element={<Register />} />
          <Route path="/createpost" element={<CreatePost />} />
          <Route path="/posts/:id" element={<PostDetail />} />
          <Route path="/postlist" element={<PostList/>} />
        </Routes>
      </div>
    </div>
  );
};

export default AppRoutes;
