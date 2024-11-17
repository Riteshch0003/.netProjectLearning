
import React from 'react';
import { Link } from 'react-router-dom';

const Navbar = () => {
    return (
      <nav className="bg-gray-800 text-white p-4">
        <ul className="flex space-x-4">
          <li><Link to="/" className="hover:text-gray-400">Home</Link></li>
          <li><Link to="/login" className="hover:text-gray-400">Login</Link></li>
          <li><Link to="/register" className="hover:text-gray-400">Register</Link></li>
          <li><Link to="/createpost" className="hover:text-gray-400">Create Post</Link></li>
          <li><Link to="/postlist" className="hover:text-gray-400">Post List</Link></li>
        </ul>
      </nav>
    );
  };
export default Navbar;
