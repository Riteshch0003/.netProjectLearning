import React from 'react';
import { Link } from 'react-router-dom';

const Navigation = () => {
  return (
    <nav>
      <ul>
        <li><Link to="/">Home</Link></li>
        <li><Link to="/login">Login</Link></li>
        <li><Link to="/Register">Register</Link></li>
        <li><Link to="/create-post">Create Post</Link></li>
      </ul>
    </nav>
  );
};

export default Navigation;
